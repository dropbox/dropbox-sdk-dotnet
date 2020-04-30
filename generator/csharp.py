from __future__ import unicode_literals

import itertools
import os
import re
import shutil

from collections import defaultdict, namedtuple
from contextlib import contextmanager

from stone.ir.data_types import (
    Float32,
    Float64,
    Int32,
    Int64,
    UInt32,
    UInt64,
    is_bytes_type,
    is_boolean_type,
    is_user_defined_type,
    is_list_type,
    is_nullable_type,
    is_numeric_type,
    is_string_type,
    is_struct_type,
    is_tag_ref,
    is_timestamp_type,
    is_union_type,
    is_void_type,
)
from stone.backend import CodeBackend


def memo_one(fn):
    """
    Memorize a single argument instance method.
    """
    cache = {}

    def wrapper(self, arg):
        value = cache.get(arg)
        if value is not None:
            return value
        value = fn(self, arg)
        cache[arg] = value
        return value
    return wrapper

ConstructorArg = namedtuple('ConstructorArg', ('type', 'name', 'arg', 'doc'))


class _CSharpGenerator(CodeBackend):
    _CAMEL_CASE_RE = re.compile('((?<=[a-z0-9])[A-Z]|(?!^)[A-Z](?=[a-z]))')
    _CSHARP_KEYWORDS = frozenset({
        'abstract', 'add', 'alias', 'as', 'ascending', 'async', 'await',
        'base', 'bool', 'break', 'byte', 'case', 'catch', 'char', 'checked',
        'class', 'const', 'continue', 'decimal', 'default', 'delegate',
        'descending', 'do', 'double', 'dynamic', 'else', 'enum', 'event',
        'explicit', 'extern', 'false', 'finally', 'fixed', 'float', 'for',
        'foreach', 'from', 'get', 'global', 'goto', 'group', 'if', 'implicit',
        'in', 'int', 'interface', 'internal', 'into', 'is', 'join', 'let',
        'lock', 'long', 'namespace', 'new', 'null', 'object', 'operator',
        'orderby', 'out', 'override', 'params', 'partial', 'private',
        'protected', 'public', 'readonly', 'ref', 'remove', 'return', 'sbyte',
        'sealed', 'select', 'set', 'short', 'sizeof', 'stackalloc', 'static',
        'string', 'struct', 'switch', 'this', 'throw', 'true', 'try', 'typeof',
        'uint', 'ulong', 'unchecked', 'unsafe', 'ushort', 'using', 'value',
        'var', 'virtual', 'void', 'volatile', 'where', 'while', 'yield',
    })

    def __init__(self, namespace_name, app_name, *args, **kwargs):
        """
        Initializes a new instance of CSharpGenerator.
        Args:
            namespace_name: The namespace name for all generated files.
            app_name: The app name which will be used as prefix for all client classes.

            *args:
            **kwargs:

        Returns:
        """
        super(_CSharpGenerator, self).__init__(*args, **kwargs)
        self._prefixes = []
        self._prefix = ''
        self._name_list = []
        self._prevent_collisions = set()
        self._generated_files = []
        self._tag_context = None

        self._namespace_name = namespace_name
        self._app_name = app_name

    def generate(self, api):
        self._generate_route_auth_map(api)

        for namespace in api.namespaces.itervalues():
            self._compute_related_types(namespace)
            self._generate_namespace(namespace)

        self._generate_client(api, '{0}Client'.format(self._app_name), 'user')
        self._generate_client(api, '{0}TeamClient'.format(self._app_name), 'team')
        self._generate_client(api, '{0}AppClient'.format(self._app_name), 'app')

        self._generate(api)

    def _generate(self, api, generated_files):
        """
        Override by derived generator to handle project specific logic.

        Args:
            generated_files:

        Returns:

        """
        raise NotImplementedError

    @contextmanager
    def cs_block(self, **kwargs):
        """
        Context manager for an allman style block, which is more common
        style for c#
        """
        kwargs['allman'] = True
        with self.block(**kwargs):
            yield

    @contextmanager
    def region(self, label):
        """
        Context manager for a c# region. All code emitted within the context
        is within the region.

        Args:
            label (Union[str, unicode]): The region label
        """
        self.emit('#region {0}'.format(label))
        self.emit()
        yield
        self.emit()
        self.emit('#endregion')

    def if_(self, condition):
        """
        Context manager for an `if` statement. All code emitted within the context
        is within the if statement.

        Args:
            condition (Union[str, unicode]): The if condition
        """

        return self.cs_block(before='if ({0})'.format(condition))

    def else_(self):
        """
        Context manager for an else statement. All code emitted within the context
        is within the else statement.
        """
        return self.cs_block(before='else')

    def else_if(self, condition):
        """
        Context manager for an `else if` statement. All code emitted within the
        context is within the `else if` statement.

        Args:
            condition (Union[str, unicode]): The else if condition.
        """
        return self.cs_block(before='else if ({0})'.format(condition))

    def namespace(self, name=None):
        """
        Context manager for a `namespace` statement. All code emitted within the
        context is within the namespace.
        """
        ns_name = '{0}.{1}'.format(self._namespace_name, name) if name else self._namespace_name

        return self.cs_block(before='namespace {0}'.format(ns_name))

    def class_(self, name, inherits=None, access=''):
        """
        Context manager for a class. All code emitted within the context is part of
        the class.

        Args:
            name (Union[str, unicode]): The name of the class.
            inherits (str|iterable): The base types for the class, if any. If
                this is a string it is added to the code verbatim, if an
                iterable, then joined with ', '
            access (Union[str, unicode]): The access modifierd of the class.
        """

        elements = []
        if access:
            elements.append(access)
        elements.append('class')
        elements.append(name)
        if inherits:
            elements.append(':')
            if isinstance(inherits, basestring):
                elements.append(inherits)
            else:
                elements.append(', '.join(inherits))
        return self.cs_block(before=' '.join(elements))

    def using(self, declaration):
        """
        Context manager for a `using` block. All code emitted within the context
        is within the using block.

        Args:
            declaration (Union[str, unicode]): The using declaration.
        """
        return self.cs_block(before='using ({0})'.format(declaration))

    def emit(self, text=''):
        """
        Wraps the regular generator emit() method.

        This is used by the prefix() and doc_comment() methods to prepend a
        fixed string to each line emitted within those contexts.

        Args:
            text (Union[str, unicode]): The text to emit
        """
        if text and self._prefix:
            super(_CSharpGenerator, self).emit(self._prefix + text)
        else:
            super(_CSharpGenerator, self).emit(text)

    def output_to_relative_path(self, filename, folder='Generated'):
        """
        Wraps the regular generator output_to_relative_path() method.

        This is used to keep track of the set of all files that are generated.

        Args:
            filename (Union[str, unicode]): The name of the file to generate.
            folder (unicode): The folder for output files.
        """

        filename = os.path.join(folder, filename)
        self._generated_files.append(filename)
        return super(_CSharpGenerator, self).output_to_relative_path(filename)

    @contextmanager
    def prefix(self, prefix):
        """
        Context manager that prepends the supplied prefix to every line of text
        that is emitted within the context.

        Args:
            prefix (Union[str, unicode]): The prefix to prepend to every line.
        """
        self._prefixes.append(prefix)
        self._prefix = ''.join(self._prefixes)
        yield
        self._prefixes.pop()
        self._prefix = ''.join(self._prefixes)

    @contextmanager
    def doc_comment(self, data_type=None, is_constructor=False):
        """
        Context manager that treats all lines of text emitted within the
        context as part of a doc comment (i.e. prefixed with '///').

        Args:
            data_type (stone.data_type.DataType): The type for which this
                documentation is being generated. This helps resolve references
                in the _tag_handler method.
            is_constructor (bool): Indicated whether this doc comment if for
                a constructor - also used when resolving references.
        """
        self._tag_context = (data_type, is_constructor)
        with self.prefix('/// '):
            yield
        self._tag_context = None

    def auto_generated(self):
        """
        Generates a standard comment for the head of every file. This prevents
        StyleCop from measuring the contents of the file.
        """
        with self.prefix('// '):
            self.emit('<auto-generated>')
            self.emit('Auto-generated by StoneAPI, do not modify.')
            self.emit('</auto-generated>')
        self.emit()

    def emit_wrapped_text(self, s, **kwargs):
        """
        Wraps the regular generator emit_wrapped_text() method.

        This does three things.
        1. It ensures consistend prefix behavior with the modified emit method
        2. It sets a default width of 95
        3. It calls self.process_doc on the input string if the process keyword
            is present

        Args:
            s (Union[str, unicode]): The string to emit and wrap.
            process (callable): The function to handle tags in the emitted text.
        """
        kwargs['prefix'] = self._prefix + kwargs.get('prefix', '')
        if 'width' not in kwargs:
            kwargs['width'] = 95
        if 'process' in kwargs:
            process = kwargs.pop('process')
            s = self.process_doc(s, process)

        super(_CSharpGenerator, self).emit_wrapped_text(s, **kwargs)

    @contextmanager
    def switch(self, expression):
        """
        Context manager for a `switch` statement.

        Args:
            expression (Union[str, unicode]): The expression to switch on.
        """
        self.emit('switch ({0})'.format(expression))
        self.emit('{')
        yield
        self.emit('}')

    @contextmanager
    def case(self, constant=None, needs_break=True):
        """
        Context manager for a `case` statement.

        Args:
            constant (Union[str, unicode]): If this is not provided, then this is generated as
                the default case.
            need_break (bool): Indicates whether a break statement should
                automatically be appended with the case statement ends.
        """
        with self.indent():
            self.emit('case {0}:'.format(constant) if constant else 'default:')
            with self.indent():
                yield
                if needs_break:
                    self.emit('break;')

    @contextmanager
    def _local_names(self, names):
        """
        This context manager is used to help resolve names if there are
        collisions between struct or union members and top level type names
        within the namespace.

        Args:
            names (iterable of str): The local names.
        """
        self._name_list.append(list(names))
        self._prevent_collisions = set(itertools.chain(*self._name_list))
        yield
        self._name_list.pop()
        self._prevent_collisions = set(itertools.chain(*self._name_list))

    def emit_xml(self, doc, tag, **attrs):
        """
        Emits an xml element.

        Args:
            doc (Union[str, unicode]): The contents of the xml element, if this is `None` then
                the element is emitted in self closed form
            tag (Union[str, unicode]): The xml element tag name.
            attrs (dict): The attributes (if any) for the element
        """
        tag_start = '<' + tag
        if attrs:
            tag_start += ' ' + ' '.join('{0}="{1}"'.format(k, v) for k,v in attrs.iteritems())

        if doc is None:
            self.emit(tag_start + ' />')
        else:
            self.emit_wrapped_text('{0}>{1}</{2}>'.format(tag_start, doc, tag),
                                   process=self._get_tag_handler(self._ns))

    @contextmanager
    def xml_block(self, tag, **attrs):
        """
        Context manager that includes all emitted code within an xml element

        Args:
            tag (Union[str, unicode]): The xml element tag name
            attrs (dict): The xml element attributes, if any.
        """
        if attrs:
            attributes = ' '.join('{0}="{1}"'.format(k, v) for k,v in attrs.iteritems())
            self.emit('<{0} {1}>'.format(tag, attributes))
        else:
            self.emit('<{0}>'.format(tag))
        if self._prefixes:
            yield
        else:
            with self.indent():
                yield
        self.emit('</{0}>'.format(tag))

    @contextmanager
    def encoder_block(self, class_name):
        """
        Context manager that emit the private decoder class

        Args:
            class_name (Union[str, unicode]): The class name for this decoder.
            inherit (Union[str, unicode]): The base type for this decoder.
        """
        self.emit()
        with self.region('Encoder class'):
            with self.doc_comment():
                self.emit_summary('Encoder for  <see cref="{0}" />.'.format(class_name))
            with self.class_(class_name + 'Encoder', inherits=['enc.StructEncoder<{0}>'.format(class_name)],
                             access='private'):
                with self.doc_comment():
                    self.emit_summary('Encode fields of given value.')
                    self.emit_xml('The value.', 'param', name='value')
                    self.emit_xml('The writer.', 'param', name='writer')
                with self.cs_block(before='public override void EncodeFields({0} value, enc.IJsonWriter writer)'.format(class_name)):
                    yield

    @contextmanager
    def decoder_block(self, class_name, inherit, is_void):
        """
        Context manager that emit the private decoder class

        Args:
            class_name (Union[str, unicode]): The class name for this decoder.
            inherit (Union[str, unicode]): The base type for this decoder.
        """
        self.emit()
        with self.region('Decoder class'):
            with self.doc_comment():
                self.emit_summary('Decoder for  <see cref="{0}" />.'.format(class_name))
            with self.class_(class_name + 'Decoder', inherits=['enc.{0}<{1}>'.format(inherit, class_name)],
                             access='private'):
                with self.doc_comment():
                    self.emit_summary('Create a new instance of type <see cref="{0}" />.'.format(class_name))
                    self.emit_xml('The struct instance.', 'returns')
                with self.cs_block(before='protected override {0} Create()'.format(class_name)):
                    if is_void:
                        self.emit('return {0}.Instance;'.format(class_name))
                    else:
                        self.emit('return new {0}();'.format(class_name))
                self.emit()
                yield

    @contextmanager
    def decoder_decode_fields_block(self, class_name):
        """
        Context manager that emit the DecodeFields override.

        Args:
            class_name (Union[str, unicode]): The class name for this decoder.
        """
        with self.doc_comment():
            self.emit_summary('Decode fields without ensuring start and end object.')
            self.emit_xml('The json reader.', 'param', name='reader')
            self.emit_xml('The decoded object.', 'returns')
        with self.cs_block(before='public override {0} DecodeFields(enc.IJsonReader reader)'.format(class_name)):
            yield

    @contextmanager
    def decoder_set_field_block(self, class_name):
        """
        Context manager that emit the SetField override block for struct decoder.

        Args:
            class_name (Union[str, unicode]): The class name for this decoder.
        """
        with self.doc_comment():
            self.emit_summary('Set given field.')
            self.emit_xml('The field value.', 'param', name='value')
            self.emit_xml('The field name.', 'param', name='fieldName')
            self.emit_xml('The json reader.', 'param', name='reader')
        with self.cs_block(
            before='protected override void SetField({0} value, string fieldName, enc.IJsonReader reader)'
            .format(class_name)):
            with self.switch('fieldName'):
                yield
                with self.case(needs_break=True):
                    self.emit('reader.Skip();')

    @contextmanager
    def decoder_tag_block(self, class_name):
        """
        Context manager that emit the Decode(tag) override block for union decoder.

        Args:
            class_name (Union[str, unicode]): The class name for this decoder.
        """
        with self.doc_comment():
            self.emit_summary('Decode based on given tag.')
            self.emit_xml('The tag.', 'param', name='tag')
            self.emit_xml('The json reader.', 'param', name='reader')
            self.emit_xml('The decoded object.', 'returns')
        with self.cs_block(before='protected override {0} Decode(string tag, enc.IJsonReader reader)'.format(class_name)):
            yield

    def emit_summary(self, doc=""):
        """
        Emits the supplied documentation as a summary element.

        Args:
            doc (Union[str, unicode]): The documentation to emit, if this is multi-line, then
                each line is wrapped in a `para` element.
        """
        lines = doc.splitlines()
        if len(lines) > 0:
            with self.xml_block('summary'):
                for line in lines:
                    self.emit_xml(line, 'para')
        else:
            self.emit_xml(doc, 'summary')

    def emit_ctor_summary(self, class_name):
        self.emit_summary('Initializes a new instance of the <see cref="{0}" /> '
                          'class.'.format(class_name))

    def _get_tag_handler(self, ns):
        def tag_handler(tag, value):
            """
            Passed as to the process_doc() method to handle tags that are found in
            the documentation string

            Args:
                tag (Union[str, unicode]): The tag type, one of 'field|link|route|type|val'
                value (Union[str, unicode]): The value of the tag.
            """
            if tag == 'field':
                if '.' in value:
                    parts = map(self._public_name, value.split('.'))
                    return '<see cref="{0}.{1}.{2}" />'.format(self._namespace_name, ns, '.'.join(parts))
                elif self._tag_context:
                    data_type, is_constructor = self._tag_context
                    if is_constructor:
                        return '<paramref name="{0}" />'.format(self._arg_name(value, is_doc=True))
                    else:
                        return '<see cref="{0}" />'.format(self._public_name(value))
                else:
                    return '<paramref name="{0}" />'.format(self._arg_name(value, is_doc=True))
            elif tag == 'link':
                parts = value.split(' ')
                uri = parts[-1]
                text = ' '.join(parts[:-1])
                return '<a href="{0}">{1}</a>'.format(uri, text)
            elif tag == 'route':
                if '.' in value:
                    ns_name, route_name = value.split('.')
                    ns_name = self._public_name(ns_name)
                else:
                    ns_name, route_name = ns, value
                
                if ':' in route_name:
                    route_name, version_str = route_name.split(':')
                    version = int(version_str)
                else:
                    route_name, version = route_name, 1
                
                route_name = self._public_route_name(route_name, version)
                auth_type = self._route_auth_map[(ns_name, route_name)]
                links = []
                for auth in auth_type:
                    links.append('<see cref="{0}.{1}.Routes.{1}{2}Routes.{3}Async" />'.format(
                        self._namespace_name, ns_name, self._public_name(auth), route_name))
                return " ".join(links)
            elif tag == 'type':
                return '<see cref="{0}" />'.format(self._public_name(value))
            elif tag == 'val':
                return '<c>{0}</c>'.format(value.strip('`'))
            else:
                assert False, 'Unknown tag: {0}:{1}'.format(tag, value)

        return tag_handler

    def _typename(self, data_type, void=None, is_property=False, is_response=False, include_namespace=False):
        """
        Generates a C# type from a data_type

        The translations for the primitive types are the exact equivalent
        C# value types. For composite types, the type name is represented using
        CamelCase. The list type is handled slightly differently for the
        property and constructor cases where it is an IList or IEnumerable
        respectively.j

        Args:
            data_type (stone.data_type.DataType): The type to translate.
            void (Union[str, unicode]): If supplied, this is the value to return if data_type
                is void.
            is_property (bool): Indicates whether the type translation is for
                a property type. Lists have different types expressed for
                properties than in other places.
            is_response (bool): Indicates whether the type translation is for
                a response type. Lists need to concrete list class in order to
                be created by Apm.
            include_namespace (bool): Indicates wheather the type translation includes
                namespace. Sometimes this needs to be true to avoild colliding with property name.
        """
        if is_nullable_type(data_type):
            nullable = True
            data_type = data_type.data_type
        else:
            nullable = False

        name = data_type.name
        if is_user_defined_type(data_type):
            public = self._public_name(name)
            type_ns = self._public_name(data_type.namespace.name)
            if type_ns != self._ns or public in self._prevent_collisions or include_namespace:
                return 'global::{0}.{1}.{2}'.format(self._namespace_name, type_ns, public)
            else:
                return public
        elif is_list_type(data_type):
            if is_property:
                return 'col.IList<{0}>'.format(self._typename(data_type.data_type, is_property=True))
            elif is_response:
                return 'col.List<{0}>'.format(self._typename(data_type.data_type, is_response=True))
            else:
                return 'col.IEnumerable<{0}>'.format(self._typename(data_type.data_type))
        elif is_string_type(data_type):
            return 'string'
        elif is_bytes_type(data_type):
            return 'byte[]'
        else:
            suffix = '?' if nullable else ''

            if is_boolean_type(data_type):
                typename = 'bool'
            elif isinstance(data_type, Int32):
                typename = 'int'
            elif isinstance(data_type, UInt32):
                typename = 'uint'
            elif isinstance(data_type, Int64):
                typename = 'long'
            elif isinstance(data_type, UInt64):
                typename = 'ulong'
            elif isinstance(data_type, Float32):
                typename = 'float'
            elif isinstance(data_type, Float64):
                typename = 'double'
            elif is_timestamp_type(data_type):
                typename = 'sys.DateTime'
            elif is_void_type(data_type):
                return void or 'void'
            else:
                assert False, 'Unknown data type %r' % data_type

            return typename + suffix

    @staticmethod
    def _process_literal(literal):
        """
        Translate literal values used in defaults

        Args:
            literal: The literal value.
        """
        if isinstance(literal, bool):
            return 'true' if literal else 'false'
        elif isinstance(literal, str) or isinstance(literal, unicode):
            return '\"{0}\"'.format(literal)
        return literal

    @staticmethod
    def _type_literal_suffix(data_type):
        """
        Returns the suffix needed to make a numeric literal values type explicit.

        Args:
            data_type (stone.data_type.DataType): The type in question.
        """
        if not is_numeric_type(data_type) or isinstance(data_type, Int32):
            return ''
        elif isinstance(data_type, UInt32):
            return 'U'
        elif isinstance(data_type, Int64):
            return 'L'
        elif isinstance(data_type, UInt64):
            return 'UL'
        elif isinstance(data_type, Float32):
            return 'F'
        elif isinstance(data_type, Float64):
            return 'D'
        else:
            assert False, 'Unknown numeric data type %r' % data_type

    @staticmethod
    def _could_be_null(data_type):
        """
        Returns true if 'data_type' could be null, i.e. if it is not a value type

        Args:
            data_type (stone.data_type.DataType): The type in question.
        """
        return is_user_defined_type(data_type) or is_string_type(data_type) or is_list_type(data_type)

    @staticmethod
    def _verbatim_string(string):
        """
        Creates a C# verbatim string (way easier than dealing with escapes)

        Args:
            string (Union[str, unicode]): The string to represent.
        """
        return '@"{0}"'.format(string.replace('"', '""'))

    def _process_composite_default(self, field, include_null_check=True):
        """
        Generate code to initialize a default value for a composite field.

        Note: This is not implemented for fields that are structs.

        Args:
            field: (stone.data_type.Field): The field to initialize.
            include_null_check (bool): Indicates whether a check for an
                argument being null should be emitted.
        """
        if is_struct_type(field.data_type):
            raise NotImplementedError()
        elif is_union_type(field.data_type):
            self._process_union_default(field, include_null_check)
        else:
            assert False, 'field is neither struct nor union: {0}.'.format(field)

    def _process_union_default(self, field, include_null_check):
        """
        Generate code to initialize a default value for a field that is a union.

        Note: This only works for union fields that don't have arguments.

        Args:
            field: (stone.data_type.Field): The field to initialize.
            include_null_check (bool): Indicates whether a check for an
                argument being null should be emitted.
        """
        assert is_tag_ref(field.default), (
            'Default union value is not a tag ref: {0}'.format(field.default))

        union = field.default.union_data_type
        default = field.default.tag_name

        arg_name = (self._arg_name(field.name) if include_null_check else
                    'this.{0}'.format(self._public_name(field.name)))

        assign_default = '{0} = {1}.{2}.Instance;'.format(
            arg_name, self._typename(union, include_namespace=True),
            self._public_name(default))

        if include_null_check:
            with self.if_('{0} == null'.format(arg_name)):
                self.emit(assign_default)
        else:
            self.emit(assign_default)

    def _check_constraints(self, name, data_type, has_null_check):
        """
        Emits code to checks the validity of a field when constructing an
        object.

        Args:
            name (Union[str, unicode]): The field name.
            data_type (stone.data_type.DataType): The type of the field
            has_null_check (bool): Indicates whether prior code has already
                generated a null check for this field - this happens if a
                composite field has a default.
        """
        if is_nullable_type(data_type):
            nullable = True
            data_type = data_type.data_type
        else:
            nullable = False

        checks = []
        if is_numeric_type(data_type):
            suffix = self._type_literal_suffix(data_type)
            if data_type.min_value is not None:
                checks.append(('{0} < {1}{2}'.format(name, data_type.min_value, suffix),
                               '"Value should be greater or equal than {0}"'.format(data_type.min_value)))
            if data_type.max_value is not None:
                checks.append(('{0} > {1}{2}'.format(name, data_type.max_value, suffix),
                               '"Value should be less of equal than {0}"'.format(data_type.max_value)))
        elif is_string_type(data_type):
            if data_type.min_length is not None:
                checks.append(('{0}.Length < {1}'.format(name, data_type.min_length),
                               '"Length should be at least {0}"'.format(data_type.min_length)))
            if data_type.max_length is not None:
                checks.append(('{0}.Length > {1}'.format(name, data_type.max_length),
                               '"Length should be at most {0}"'.format(data_type.max_length)))
            if data_type.pattern is not None:
                # patterns must match entire input sequence:
                pattern = '\\A(?:{0})\\z'.format(data_type.pattern)
                checks.append(('!re.Regex.IsMatch({0}, {1})'.format(name, self._verbatim_string(pattern)),
                               self._verbatim_string("Value should match pattern '{0}'".format(pattern))))
        elif is_list_type(data_type):
            list_name = name + 'List'
            self.emit('var {0} = enc.Util.ToList({1});'.format(list_name, name))
            self.emit()

            if data_type.min_items is not None:
                checks.append(('{0}.Count < {1}'.format(list_name, data_type.min_items),
                               '"List should at at least {0} items"'.format(data_type.min_items)))
            if data_type.max_items is not None:
                checks.append(('{0}.Count > {1}'.format(list_name, data_type.max_items),
                               '"List should at at most {0} items"'.format(data_type.max_items)))

        has_checks = len(checks) > 0

        def apply_checks():
            for check, message in checks:
                with self.if_('{0}'.format(check)):
                    self.emit('throw new sys.ArgumentOutOfRangeException("{0}", {1});'.format(name, message))
        if nullable:
            if has_checks:
                with self.if_('{0} != null'.format(name)):
                    apply_checks()
        else:
            if self._could_be_null(data_type) and not has_null_check:
                with self.if_('{0} == null'.format(name)):
                    self.emit('throw new sys.ArgumentNullException("{0}");'.format(name))
                has_checks = True
            apply_checks()

        if has_checks:
            self.emit()

    def _arg_name(self, name, is_doc=False):
        """
        Creates an initial lowercase camelCase representation of name.

        This performs the following transformations.
            foo_bar -> fooBar
            fooBar -> fooBar
            FooBar -> fooBar

        Args:
            name (Union[str, unicode]): The name to transform
            is_doc (bool): If the arg name is in doc.
        """
        public = self._public_name(name)
        arg_name = public[0].lower() + public[1:]
        if arg_name in _CSharpGenerator._CSHARP_KEYWORDS and not is_doc:
            return '@' + arg_name
        return arg_name
    
    def _route_url(self, ns_name, route_name, route_version):
        """
        Get url path for given route.

        Args:
            ns_name (Union[str, unicode]): The name of the namespace.
            route_name (Union[str, unicode]): The name of the route.
            route_version (int): The version of the route.
        """

        route_url = '/{}/{}'.format(ns_name, route_name)

        if route_version != 1:
            route_url = "{}_v{}".format(route_url, route_version)

        return '"{}"'.format(route_url)

    def _public_route_name(self, name, version):
        """
        Creates an initial capitalize CamelCase representation of a route name with optional version suffix.

        This performs the following transformations.
            foo_bar -> FooBar
            foo_bar:2 -> FooBarV2

        Args:
            route (Union[str, unicode]): Name of the route.
            version (int): Version number
        """
        route_name = self._public_name(name)

        if version != 1:
            route_name = '{}V{}'.format(route_name, version)

        return route_name

    @memo_one
    def _segment_name(self, name):
        """
        Segments a name into a list of lowercase components.

        Names are segmented on '/' or '_' characters and also on CamelCase boundaries.

        Args:
            name (Union[str, unicode]): The name to segment.
        """
        name = name.replace('/', '_')
        name = _CSharpGenerator._CAMEL_CASE_RE.sub(r'_\1', name).lower()
        return name.split('_')

    @memo_one
    def _public_name(self, name):
        """
        Creates an initial capitalize CamelCase representation of name.

        This performs the following transformations.
            foo_bar -> FooBar
            fooBar -> FooBar
            FooBar -> FooBar

        Args:
            name (Union[str, unicode]): The name to transform
        """
        return ''.join(x.capitalize() for x in self._segment_name(name))
    
    @memo_one
    def _name_words(self, name):
        """
        Creates a space separated sequence of words from a name.

        This performs the following transformation.
            foo_bar -> 'foo bar'
            fooBar -> 'foo bar'
            FooBar -> 'foo bar'

        Args:
            name (Union[str, unicode]): The name to transform
        """
        return ' '.join(self._segment_name(name))

    def _generate_client(self, api, client_name, auth_type):
        """
        Generates a partial class for client name with given auth type only includes
        the route declarations and the route initialization, the rest of the
        class can be hand written separately.

        Args:
            api (stone.api.Api): The API specification.
            client_name (Union[str, unicode]): The name of the client. e.g. XXXClient, XXXTeamClient
            auth_type (Union[str, unicode]): The expected auth type for the client. e.g. User, Team
        """
        with self.output_to_relative_path('{0}.cs'.format(client_name)):
            self.auto_generated()
            with self.namespace():
                self.emit('using sys = System;')
                self.emit()
                self.emit('using {0}.Stone;'.format(self._namespace_name))

                def enumerate_ns():
                    for ns in api.namespaces.itervalues():
                        name = self._public_name(ns.name)
                        for auth, _ in self._get_routes(ns).iteritems():
                            if auth == auth_type:
                                yield name

                for ns_name in enumerate_ns():
                    self.emit('using {0}.{1}.Routes;'.format(self._namespace_name,
                                                             self._public_name(ns_name)))
                self.emit()

                auth_name = self._public_name(auth_type)
                with self.class_(client_name, access='public sealed partial'):
                    first = True
                    for ns_name in enumerate_ns():
                        if first:
                            first = False
                        else:
                            self.emit()

                        with self.doc_comment():
                            self.emit_summary('Gets the {0} routes.'.format(ns_name))
                        self.emit('public {0}{1}Routes {0} {{ get; private set; }}'
                                  .format(ns_name, auth_name))

                    self.emit()
                    with self.doc_comment():
                        self.emit_summary('Initializes the routes.')
                        self.emit_xml('The transport.', 'returns')
                    with self.cs_block(before='internal override void InitializeRoutes(ITransport transport)'):
                        for ns_name in enumerate_ns():
                            self.emit('this.{0} = new {0}{1}Routes(transport);'.format(ns_name, auth_name))

    def _compute_related_types(self, ns): 
        """
        This creates a map of supertype-subtype relationships.

        This is used to generate `seealso` documentation, because the
        specification type hierarchy is not always present in the generated
        code.
        """
        related_types = defaultdict(set)
        for data_type in ns.data_types:
            if not is_struct_type(data_type):
                continue

            struct_name = data_type.name

            if data_type.parent_type:
                related_types[data_type.parent_type.name].add(struct_name)
                related_types[struct_name].add(self._typename(data_type.parent_type))

            for field in data_type.all_fields:
                if not is_struct_type(field.data_type):
                    continue

                related_types[field.data_type.name].add(struct_name)

        self._related_types = related_types

    def _generate_route_auth_map(self, api):
        d = dict()

        for ns in api.namespaces.itervalues():
            for route in ns.routes:
                key = (self._public_name(ns.name), self._public_route_name(route.name, route.version))
                d[key] = self._get_auth_type(route)

        self._route_auth_map = d

    def _generate_namespace(self, ns):
        """
        Perform code generation for the namespace.

        This calls methods that generate classes for each data type and a class
        for all the routes.

        Args:
            ns (stone.api.ApiNamespace): The namespace to generate.
        """
        ns_name = self._public_name(ns.name)

        self._ns = ns_name
        for data_type in ns.data_types:
            self._generate_data_type(ns_name, data_type)
        if ns.routes:
            self._generate_routes(ns)

    def _generate_data_type(self, ns_name, data_type):
        """
        Generate the classes for a data type.

        This generates the framework of the code file and calls an appropriate 
        method for structs and unions to generate the type itself.

        Args:
            ns_name (Union[str, unicode]): The name of the namespace.
            data_type (stone.data_type.DataType): The type to generate.
        """
        assert is_user_defined_type(data_type)
        class_name = self._public_name(data_type.name)
        with self.output_to_relative_path(os.path.join(ns_name, class_name + ".cs")):
            # this stops stylecop from analyzing the file
            self.auto_generated()

            with self.namespace(ns_name):
                # place using statements inside the namespace to make aliasing bugs
                # explicit
                self.emit('using sys = System;')
                self.emit('using col = System.Collections.Generic;')
                self.emit('using re = System.Text.RegularExpressions;')
                self.emit()
                self.emit('using enc = {0}.Stone;'.format(self._namespace_name))
                self.emit()

                if is_struct_type(data_type):
                    self._generate_struct(data_type)
                elif is_union_type(data_type):
                    self._generate_union(data_type)
                else:
                    assert False, 'Unknown composite type: %r' % data_type

    def _emit_explicit_interface_suppress(self):
        """
        Generates a suppression attribute that prevents a useless CodeAnalysis warning

        The warning isn't useless in general, but just isn't relevant to our use case.
        """
        self.emit('[System.Diagnostics.CodeAnalysis.SuppressMessage('
                  '"Microsoft.Design", '
                  '"CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]')

    @staticmethod
    def _parse_data_type(data_type):
        """
        Parse the data data type and get its core type.

        """
        
        is_nullable = is_nullable_type(data_type)
        if is_nullable:
            data_type = data_type.data_type

        is_list = is_list_type(data_type)
        if is_list:
            data_type = data_type.data_type

        return data_type, is_nullable, is_list

    @staticmethod
    def _get_primitive_prefix(data_type):
        """
        Get encoder/decoder name prefix for primitive types.

        Args:
            data_type (stone.data_type.DataType): The type.
        """
        if is_string_type(data_type):
            return 'String'
        elif is_bytes_type(data_type):
            return 'Bytes'
        elif is_boolean_type(data_type):
            return 'Boolean'
        elif isinstance(data_type, Int32):
            return 'Int32'
        elif isinstance(data_type, UInt32):
            return 'UInt32'
        elif isinstance(data_type, Int64):
            return 'Int64'
        elif isinstance(data_type, UInt64):
            return 'UInt64'
        elif isinstance(data_type, Float32):
            return 'Float'
        elif isinstance(data_type, Float64):
            return 'Double'
        elif is_timestamp_type(data_type):
            return 'DateTime'
        elif is_void_type(data_type):
            return 'Empty'
        else:
            assert False, 'Unknown data type %r' % data_type

    @staticmethod
    def _get_primitive_instance_name(is_nullable):
        """
        Get encoder/decoder instance name.

        Args:
            is_nullable (bool): If the type is nullable.
        """

        return 'NullableInstance' if is_nullable else 'Instance'

    @staticmethod
    def _get_union_fields(union):
        """
        Get all fields including fields in parent union type.

        Args:
            union (stone.data_type.Union): The union in question.
        """

        fields = list(union.fields)
        if union.parent_type:
            fields.extend(union.parent_type.fields)

        return fields

    @staticmethod
    def _get_auth_type(route):
        auth_type = route.attrs.get('auth', 'user')
        if "," in auth_type:
            multi_auths = auth_type.replace(" ", "").split(",")
            return multi_auths
        if auth_type == 'noauth':
            return ['user']
        else:
            return [auth_type]

    @classmethod
    def _get_routes(cls, ns):
        ret = defaultdict(list)

        for r in ns.routes:
            for auth in cls._get_auth_type(r):
                ret[auth].append(r)

        return ret

    def _get_decoder(self, data_type):
        """
        Get decoder of type IDecoder<T> for given data type.

        Args:
            data_type (stone.data_type.DataType): The type.
        """

        data_type, is_nullable, is_list = self._parse_data_type(data_type)

        if is_list:
            return 'enc.Decoder.CreateListDecoder({0})'.format(self._get_decoder(data_type))
        elif is_user_defined_type(data_type):
            return '{0}.Decoder'.format(self._typename(data_type, include_namespace=True))
        else: 
            return 'enc.{0}Decoder.{1}'.format(
                self._get_primitive_prefix(data_type),
                self._get_primitive_instance_name(is_nullable))

    def _get_encoder(self, data_type):
        """
        Get encoder of type IEncoder<T> for given data type.

        Args:
            data_type (stone.data_type.DataType): The type.
        """

        data_type, is_nullable, is_list = self._parse_data_type(data_type)

        if is_list:
            return 'enc.Encoder.CreateListEncoder({0})'.format(self._get_encoder(data_type))
        elif is_user_defined_type(data_type):
            return '{0}.Encoder'.format(self._typename(data_type, include_namespace=True))
        else:
            return 'enc.{0}Encoder.{1}'.format(
                self._get_primitive_prefix(data_type),
                self._get_primitive_instance_name(is_nullable))

    def _emit_decoder(self, field, field_public_name=None):
        """
        Emits a decoder fragment for a struct field

        Args:
            field (stone.data_type.Field): The field to be decoded.
            field_public_name (Union[str, unicode]): Optional field public name.
        """
        field_public_name = field_public_name or self._public_name(field.name)
        data_type, is_nullable, is_list = self._parse_data_type(field.data_type)

        if is_list:
            value = 'ReadList<{0}>(reader, {1})'.format(self._typename(data_type, is_property=True), self._get_decoder(data_type))
        else:
            value = '{0}.Decode(reader)'.format(self._get_decoder(data_type))

        self.emit('value.{0} = {1};'.format(field_public_name, value))
    
    def _emit_encoder(self, field, field_public_name=None, inline_composite_type=False):
        """
        Emits an encoder fragment for a struct field.

        Args:
            field (stone.data_type.Field): The field to generate the encoder for.
            field_public_name (Union[str, unicode]): Optional field_public_name.
            inline_composite_type (bool): If True, composite type will be inline instead
            of encoded as field.
        """
        field_public_name = field_public_name or self._public_name(field.name)
        data_type, is_nullable, is_list = self._parse_data_type(field.data_type)
        
        if is_nullable:
            nullable = True
            if is_list:
                null_block = self.if_('value.{0}.Count > 0'.format(field_public_name))
            else:
                null_block = self.if_('value.{0} != null'.format(field_public_name))
            null_block.__enter__()
        else:
            nullable = False
            null_block = None

        try:
            method = 'WriteProperty'
            if is_list:
                method = 'WriteListProperty'
            elif is_user_defined_type(data_type):
                if is_string_type(data_type) and inline_composite_type:
                    # We inline struct fields when a union field is a struct.
                    self.emit('{0}.EncodeFields(value.Value, writer);'.format(self._get_encoder(data_type)))
                    return
            elif nullable and not is_string_type(data_type):
                field_public_name += '.Value'

            self.emit('{0}("{1}", value.{2}, writer, {3});'.format(
                method, field.name, field_public_name, self._get_encoder(data_type)))

        finally:
            if null_block:
                null_block.__exit__(None, None, None)
    
    def _emit_encoder_subtype(self, tag, subtype_name):
        with self.if_('value is {0}'.format(subtype_name)):
            self.emit('WriteProperty(".tag", "{0}", writer, enc.StringEncoder.Instance);'.format(tag))
            self.emit('{0}.Encoder.EncodeFields(({0})value, writer);'.format(subtype_name))
            self.emit('return;')

    def _make_struct_constructor_args(self, struct):
        """
        Creates a list of ConstructorArg instances for the fields of the 
        supplied struct. This prevents re-calculating the same information
        for each field in multiple places.

        Each entry in the returned list has the following elements
             - The C# type of the field
             - The name of the field suitable for use as an argument parameter
             - The argument declaration of the field for the constructor, this
                will include a default value where appropriate.
             - The doc string for the field.

        Args:
            struct (stone.data_type.Struct): The struct whose constructor 
                arguments are being enumerated.
        """
        constructor_args = []
        ns_name = self._public_name(struct.namespace.name)

        for field in struct.all_fields:
            fieldtype = self._typename(field.data_type)
            arg_name = self._arg_name(field.name)
            doc_name = arg_name[1:] if arg_name.startswith('@') else arg_name

            if field.has_default:
                if is_user_defined_type(field.data_type):
                    # we'll populate the real default when we check constraints
                    arg = '{0} {1} = null'.format(fieldtype, arg_name)
                else:
                    arg = '{0} {1} = {2}'.format(fieldtype, arg_name, self._process_literal(field.default))
            elif is_nullable_type(field.data_type):
                arg = '{0} {1} = null'.format(fieldtype, arg_name)
            else:
                arg = '{0} {1}'.format(fieldtype, arg_name)

            doc = field.doc or 'The {0}'.format(self._name_words(field.name))
            self._tag_context = (struct, True)
            doc = self.process_doc(doc, self._get_tag_handler(ns_name))
            self._tag_context = None
            doc = '<param name="{0}">{1}</param>'.format(doc_name, doc)

            constructor_args.append(ConstructorArg(fieldtype, arg_name, arg, doc))

        return constructor_args

    def _generate_encoder_decoder_instance(self, class_name):
        """
        Emits the encoder and decoder instance.

        Args:
            class_name (Union[str, unicode]): The C# class name of the struct.
        """

        self.emit('#pragma warning disable 108')
        self.emit()

        with self.doc_comment():
            self.emit_summary('The encoder instance.')
        self.emit('internal static enc.StructEncoder<{0}> Encoder = new {0}Encoder();'.format(class_name))
        self.emit()

        with self.doc_comment():
            self.emit_summary('The decoder instance.')
        self.emit('internal static enc.StructDecoder<{0}> Decoder = new {0}Decoder();'.format(class_name))
        self.emit()

    def _generate_struct_init_ctor(self, struct, class_name, parent_type, parent_type_fields):
        """
        Generates the initialization constructor for a struct.

        This constructor has arguments for all fields on the struct, and
        performs validation and default handling for fields.

        Args:
            struct (stone.data_type.Struct): The struct for which we are
                generating a constructor.
            class_name (Union[str, unicode]): The C# class name of the struct.
            parent_type (stone.data_type.Struct): The parent type of this
                struct, if any.
            parent_type_fields (set): A set containing the names of fields
                that are implemented by this struct's parent type hierarchy.
        """
        ctor_args = self._make_struct_constructor_args(struct)
        super_args = []
        if parent_type:
            super_args = self._make_struct_constructor_args(parent_type)

        with self.doc_comment(data_type=struct, is_constructor=True):
            self.emit_ctor_summary(class_name)
            for arg in ctor_args:
                self.emit_wrapped_text(arg.doc)

        ctor_access = 'protected' if struct.has_enumerated_subtypes() and ctor_args else 'public'
        self.generate_multiline_list(
            [item.arg for item in ctor_args],
            before='{0} {1}'.format(ctor_access, class_name),
            skip_last_sep=True
        )
        if super_args:
            with self.indent():
                self.emit(': base({0})'.format(', '.join([item.name for item in super_args])))

        with self.cs_block():
            for field in struct.all_fields:
                # Initialize fields and check that they meet their
                # constraints according to the specification.
                if field.name in parent_type_fields:
                    continue

                has_null_check = False
                if field.has_default and is_user_defined_type(field.data_type):
                    self._process_composite_default(field)
                    has_null_check = True
                self._check_constraints(self._arg_name(field.name), field.data_type, has_null_check)

            for field in struct.all_fields:
                if field.name in parent_type_fields:
                    continue

                field_public_name = self._public_name(field.name)
                field_arg_name = self._arg_name(field.name)
                if (is_list_type(field.data_type) or
                    (is_nullable_type(field.data_type) and is_list_type(field.data_type.data_type))):
                    self.emit('this.{0} = {1}List;'.format(field_public_name, field_arg_name))
                else:
                    self.emit('this.{0} = {1};'.format(field_public_name, field_arg_name))

    def _generate_struct_default_ctor(self, struct, class_name, parent_type_fields):
        """
        Generates the default constructor for a struct.

        This is only relevant if the struct has fields - otherwise the
        initializing constructor is also the default constructor.

        This intializes fields to their default values if any, so that when the
        struct is being decoded, if the fields are not present in the message
        they will have their default values.

        Args:
            struct (stone.data_type.Struct): The struct to generate a
                constructor for.
            class_name (Union[str, unicode]): The C# class name for the struct.
            parent_type_fields (set): A set containing the names of fields
                that are implemented by this struct's parent type hierarchy.
        """
        assert len(struct.all_fields), ('Only generate a default ctor when '
                                        'the struct {0} has fields'.format(struct.name))

        self.emit()
        with self.doc_comment():
            self.emit_ctor_summary(class_name)
            self.emit_xml('This is to construct an instance of the object when '
                          'deserializing.', 'remarks')

        self.emit('[sys.ComponentModel.EditorBrowsable(sys.ComponentModel.EditorBrowsableState.Never)]')
        with self.cs_block(before='public {0}()'.format(class_name)):
            # initialize fields to their default values, where necessary
            for field in struct.all_fields:
                if field.name in parent_type_fields:
                    continue
                if field.has_default:
                    if is_user_defined_type(field.data_type):
                        self._process_composite_default(field, include_null_check=False)
                    else:
                        self.emit('this.{0} = {1};'.format(
                            self._public_name(field.name), self._process_literal(field.default)))

    def _generate_struct_strunion_is_as(self, struct):
        """
        Generates the IsFoo AsFoo properties for the subtypes of this struct.

        This is only relevant if the struct has enumerated subtypes - it is a
        strunion.

        Args:
            struct (stone.data_type.Struct): The struct in question.
        """
        assert struct.has_enumerated_subtypes(), ('Only generate is/as '
                                                  'properties when the struct {0} has enumerated '
                                                  'subtypes'.format(struct.name))

        for subtype in struct.get_enumerated_subtypes():
            subtype_type = self._typename(subtype.data_type)
            subtype_name = self._public_name(subtype.name)
            self.emit()
            with self.doc_comment():
                self.emit_summary('Gets a value indicating whether this instance is {0}'.format(subtype_name))
            with self.cs_block(before='public bool Is{0}'.format(subtype_name)):
                with self.cs_block(before='get'):
                    self.emit('return this is {0};'.format(subtype_type))

            self.emit()
            with self.doc_comment():
                self.emit_summary('Gets this instance as a <see cref="{0}" />, or <c>null</c>.'.format(subtype_type))
            with self.cs_block(before='public {0} As{1}'.format(subtype_type, subtype_name)):
                with self.cs_block(before='get'):
                    self.emit('return this as {0};'.format(subtype_type))

    def _generate_struct_properties(self, struct, parent_type_fields):
        """
        Generates the properties for struct fields.

        Args:
            struct (stone.data_type.Struct): The struct in question.
            parent_type_fields (set): A set containing the names of fields
                that are implemented by this struct's parent type hierarchy.
        """
        for field in struct.all_fields:
            if field.name in parent_type_fields:
                continue
            self.emit()
            doc = field.doc or 'Gets the {0} of the {1}'.format(
                self._name_words(field.name), self._name_words(struct.name))
            with self.doc_comment(data_type=struct):
                self.emit_summary(doc)

            fieldtype = self._typename(field.data_type, is_property=True)
            self.emit('public {0} {1} {{ get; {2} set; }}'.format(fieldtype,
                                                                  self._public_name(field.name),
                                                                  'protected'))

    def _get_struct_tag(self, struct):
        if struct.parent_type and struct.parent_type.has_enumerated_subtypes():
            for subtype in struct.parent_type.get_enumerated_subtypes():
                if subtype.data_type is struct:
                    return subtype.name

    def _generate_struct_encoder(self, struct):
        """
        Generates the private encoder for the struct.

        Args:
            struct (stone.data_type.Struct): The struct in question.
        """
        class_name = self._public_name(struct.name)

        with self.encoder_block(class_name=class_name):
            if struct.has_enumerated_subtypes():
                for subtype in struct.get_enumerated_subtypes():
                    data_type = subtype.data_type
                    self._emit_encoder_subtype(self._get_struct_tag(data_type), self._typename(data_type))
                for field in struct.all_fields:
                    self._emit_encoder(field)
            else:
                for field in struct.all_fields:
                    self._emit_encoder(field)
 
    def _generate_struct_decoder(self, struct):
        """
        Generates the private decoder for the struct.

        """

        class_name = self._public_name(struct.name)
        self.emit()

        if struct.has_enumerated_subtypes():
            inherit = 'UnionDecoder'
        else:
            inherit = 'StructDecoder'

        with self.decoder_block(class_name=class_name, inherit=inherit, is_void=False):
            if struct.has_enumerated_subtypes():
                with self.decoder_tag_block(class_name=class_name):
                    with self.switch('tag'):
                        for subtype in struct.get_enumerated_subtypes():
                            with self.case('"{0}"'.format(subtype.name), needs_break=False):
                                subtype_typename = self._typename(subtype.data_type)
                                self.emit('return {0}.Decoder.DecodeFields(reader);'.format(subtype_typename))
                        if struct.is_catch_all():
                            with self.case(needs_break=False):
                                self.emit('return base.Decode(reader);')
                        else:
                            with self.case(needs_break=False):
                                self.emit('throw new sys.InvalidOperationException();')

            with self.decoder_set_field_block(class_name=class_name):
                for field in struct.all_fields:
                    with self.case('"{0}"'.format(field.name), needs_break=True):
                        self._emit_decoder(field)   

    def _generate_struct(self, struct):
        """
        Generates the class for a struct.

        This performs the following steps.
            - Emits class documentation
            - Emits the class declaration
            - Emits a constructor that takes arguments to initialize the fields
            - If there are fields, emits a default constructor which will be
                used by the deserialization process, and which initializes 
                fields to their default values.
            - If this struct has enumerated subtypes, then emit accessor
                properties for those subtypes
            - Emits properties for fields (not including fields in parent 
                types)
            - Emits the encoder and decoder implementations
        """
        with self.doc_comment(data_type=struct):
            self.emit_summary(struct.doc or 'The {0} object'.format(self._name_words(struct.name)))
            for related in sorted(self._related_types[struct.name]):
                self.emit_xml(None, 'seealso', cref=self._public_name(related))

        class_name = self._public_name(struct.name)

        if struct.parent_type:
            parent_type = struct.parent_type
            inherits = [self._typename(parent_type)]
            parent_type_fields = set(f.name for f in parent_type.all_fields)
        else:
            parent_type = None
            parent_type_fields = set()
            inherits = []

        with self.class_(class_name, inherits=inherits, access='public'):
            # Generate encoder and decoder
            self._generate_encoder_decoder_instance(class_name)

            # Generate the initializing constructor.
            self._generate_struct_init_ctor(struct, class_name, parent_type, parent_type_fields)

            # Generate a default constructor
            if len(struct.all_fields):
                # the default constructor is only needed if the struct has fields
                self._generate_struct_default_ctor(struct, class_name, parent_type_fields)

            if struct.has_enumerated_subtypes():
                # Generate properties for checking/getting the actual type
                self._generate_struct_strunion_is_as(struct)

            # Emit properties for all fields
            self._generate_struct_properties(struct, parent_type_fields)

            # Emit private encoder class.
            self._generate_struct_encoder(struct)

            # Emit private decoder class.
            self._generate_struct_decoder(struct)
    
    def _generate_union_is_as_properties(self, union):
        """
        Generates this IsFoo AsFoo properties for the union fields.

        These properties allow code to check and cast a union instances.

        Args:
            union (stone.data_type.Union): The union in question.
        """
        for field in self._get_union_fields(union):
            field_type = self._public_name(field.name)
            self.emit()
            with self.doc_comment():
                self.emit_summary('Gets a value indicating whether this instance is {0}'.format(field_type))
            with self.cs_block(before='public bool Is{0}'.format(field_type)):
                with self.cs_block(before='get'):
                    self.emit('return this is {0};'.format(field_type))

            self.emit()
            with self.doc_comment():
                self.emit_summary('Gets this instance as a {0}, or <c>null</c>.'.format(field_type))
            with self.cs_block(before='public {0} As{0}'.format(field_type)):
                with self.cs_block(before='get'):
                    self.emit('return this as {0};'.format(field_type))
    
    def _generate_union_encoder(self, union, class_name):
        """
        Generates private encoder class for a union.

        Args:
            union (stone.data_type.Union): The union in question.
            class_name (Union[str, unicode]): The C# class name of the union.
        """
        with self.encoder_block(class_name=class_name):
            for field in self._get_union_fields(union):
                self._emit_encoder_subtype(field.name, self._public_name(field.name))
            self.emit('throw new sys.InvalidOperationException();')

    def _generate_union_decoder(self, union, class_name):
        """
        Generates private decoder class for a union.

        Args:
            union (stone.data_type.Union): The union in question.
            class_name (Union[str, unicode]): The C# class name of the union.
        """

        with self.decoder_block(class_name=class_name, inherit='UnionDecoder', is_void=False):
            with self.decoder_tag_block(class_name=class_name):
                with self.switch('tag'):
                    for field in self._get_union_fields(union):
                        constant = None if union.catch_all_field == field else '"{0}"'.format(field.name)
                        with self.case(constant, needs_break=False):
                            self.emit('return {0}.Decoder.DecodeFields(reader);'.format(self._public_name(field.name)))
                    if not union.catch_all_field:
                        with self.indent():
                            self.emit('default:')
                            with self.indent():
                                self.emit('throw new sys.InvalidOperationException();')

    def _generate_union_field_void_type(self, field, field_type):
        """
        Generates the inner type for a union field that is void.

        This has a private constructor and a singleton static instance.

        Args:
            field (stone.data_type.UnionField): The union field in question.
            field_type (Union[str, unicode]): The C# type name of the union field.
        """
        # constructor
        with self.doc_comment():
            self.emit_ctor_summary(field_type)
        with self.cs_block(before='private {0}()'.format(field_type)):
            pass

        # singleton instance
        self.emit()
        with self.doc_comment():
            self.emit_summary('A singleton instance of {0}'.format(field_type))
        self.emit('public static readonly {0} Instance = new {0}();'.format(field_type))

        # Private encoder.
        with self.encoder_block(class_name=field_type):
            pass
        
        # Private decoder.
        with self.decoder_block(class_name=field_type, inherit='StructDecoder', is_void=True):
            pass

    def _generate_union_field_value_type(self, field, field_type):
        """
        Generates the inner type for a union field that has a value.

        This has a public constructor and a Value property.

        Args:
            field (stone.data_type.UnionField): The union field in question.
            field_type (Union[str, unicode]): The C# type name of the union field.
        """
        with self.doc_comment():
            self.emit_ctor_summary(field_type)
            self.emit('<param name="value">The value</param>')

        value_type = self._typename(field.data_type)
        with self.cs_block(
                before='public {0}({1} value)'.format(field_type, value_type)):
            if is_list_type(field.data_type):
                self.emit('this.Value = new col.List<{0}>(value);'.format(
                        self._typename(field.data_type.data_type)))
            else:
                self.emit('this.Value = value;')

        with self.doc_comment():
            self.emit_ctor_summary(field_type)
        with self.cs_block(before='private {0}()'.format(field_type)):
            pass

        self.emit()
        with self.doc_comment():
            self.emit_summary('Gets the value of this instance.')
        value_type = self._typename(field.data_type, is_property=True)
        self.emit('public {0} Value {{ get; private set; }}'.format(value_type))

        # Private encoder.
        with self.encoder_block(class_name=field_type):
            self._emit_encoder(field, 'Value', True)
        
        data_type = field.data_type
        if is_nullable_type(data_type):
            data_type = data_type.data_type

        # Private decoder.
        with self.decoder_block(class_name=field_type, inherit='StructDecoder', is_void=False):
            if is_struct_type(data_type) and not data_type.has_enumerated_subtypes():
                with self.decoder_decode_fields_block(class_name=field_type):
                    self.emit('return new {0}({1}.DecodeFields(reader));'.format(
                        field_type, self._get_decoder(data_type)))
            else:
                with self.decoder_set_field_block(class_name=field_type):
                    with self.case('"{0}"'.format(field.name), needs_break=True):
                        self._emit_decoder(field, 'Value')

    def _generate_union_field_type(self, field, class_name):
        """
        Generates the inner class for a union field.

        Args:
            field (stone.data_type.UnionField): The union field in question.
            class_name (Union[str, unicode]): The C# type name of the parent union.
        """
        field_type = self._public_name(field.name)
        self.emit()

        with self.doc_comment():
            self.emit_summary(field.doc or 'The {0} object'.format(self._name_words(field.name)))

        with self.class_(field_type, inherits=(class_name,), access='public sealed'):
            self._generate_encoder_decoder_instance(field_type)

            if is_void_type(field.data_type):
                self._generate_union_field_void_type(field, field_type)
            else:
                self._generate_union_field_value_type(field, field_type)

    def _generate_union(self, union):
        """
        Generates the class for a union.

        This performs the following steps
            - Creates a name context with the union field names, this protects
                against name collisions when resolving names.
            - Generates the class level documentation for the union class
            - Generates the class and its default constructor
            - Generates type helper ('Is<field>' and 'As<field>') properties
            - Generates encodable methos
            - Generates an inner type for each union field.

        Args:
            union (stone.data_type.Union): The union in question.
        """
        union_field_names = [self._public_name(f.name) for f in self._get_union_fields(union)]
        with self._local_names(union_field_names):
            with self.doc_comment():
                self.emit_summary(union.doc or 'The {0} object'.format(self._name_words(union.name)))
            class_name = self._public_name(union.name)
            with self.class_(class_name, access='public'):
                self._generate_encoder_decoder_instance(class_name)

                with self.doc_comment():
                    self.emit_ctor_summary(class_name)
                with self.cs_block(before='public {0}()'.format(class_name)):
                    pass

                # generate type helper properties
                self._generate_union_is_as_properties(union)

                # Emit private encoder class for the union.
                self._generate_union_encoder(union, class_name)

                # Emit private decoder class for the union.
                self._generate_union_decoder(union, class_name)

                # generate types for each union field
                for field in self._get_union_fields(union):
                    self._generate_union_field_type(field, class_name)

    def _generate_routes(self, ns):
        """
        Generates the class that encapsulates the routes in this namespace.

        This class has methods for each route and is constructed with an
        instance of the ITransport interface.

        Args:
            ns (stone.api.ApiNamespace): The namespace.
            routes (iterable of stone.api.ApiRoute): The routes in this
                namespace.
        """
        ns_name = self._public_name(ns.name)

        for auth_type, routes in self._get_routes(ns).iteritems():
            class_name = '{}{}{}'.format(ns_name, self._public_name(auth_type), 'Routes')
            with self.output_to_relative_path(os.path.join(ns_name, class_name + '.cs')):
                # this stops stylecop from analyzing the file
                self.auto_generated()
                with self.namespace('.'.join([ns_name, 'Routes'])):
                    self.emit('using sys = System;')
                    self.emit('using io = System.IO;')
                    self.emit('using col = System.Collections.Generic;')
                    self.emit('using t = System.Threading.Tasks;')
                    self.emit('using enc = {0}.Stone;'.format(self._namespace_name))
                    self.emit()

                    with self.doc_comment():
                        self.emit_summary('The routes for the <see cref="N:{0}.{1}"/> namespace'.format(
                            self._namespace_name, ns_name))
                    with self.class_(class_name, access='public'):
                        with self.doc_comment():
                            self.emit_ctor_summary(class_name)
                            self.emit_xml('The transport to use', 'param', name='transport')
                        with self.cs_block(before='internal {0}(enc.ITransport transport)'.format(
                                class_name)):
                            self.emit('this.Transport = transport;')

                        self.emit()
                        with self.doc_comment():
                            self.emit_summary('Gets the transport used for these routes')
                        self.emit('internal enc.ITransport Transport { get; private set; }')

                        for route in routes:
                            self._generate_route(ns, route, auth_type)

    def _generate_route(self, ns, route, auth_type):
        """
        Generates the methods that allow a route to be called.

        The route has at least one, maybe two, *Async() methods - there is only
        one method if the request type is void or has no fields, otherwise there
        are two, one with the request type explicitly and another with the 
        request type constructor arguments.

        For each *Async method there is a Begin* method with the same arguments - 
        plus callback and state arguments.

        There is one End* method generated.

        Args:
            ns (stone.api.ApiNamespace): The namespace of the route.
            route (stone.api.ApiRoute): The route in question.
        """
        public_name = self._public_route_name(route.name, route.version)
        async_name = '{0}Async'.format(public_name)
        route_host = route.attrs.get('host', 'api')
        route_style = route.attrs.get('style', 'rpc')
        # Have to check for noauth - we want noauth routes to be grouped in with user routes
        # Which is what would get passed in but it is noauth we want to override it here so noauth routes work
        original_auth_type = route.attrs.get('auth', 'user')
        if original_auth_type == 'noauth':
            auth_type = original_auth_type

        arg_type = self._typename(route.arg_data_type, void='enc.Empty')
        arg_is_void = is_void_type(route.arg_data_type)
        arg_name = (self._arg_name(route.arg_data_type.name) if
                    is_user_defined_type(route.arg_data_type) else 'request')
        result_type = self._typename(route.result_data_type, void='enc.Empty', is_response=True)
        result_is_void = is_void_type(route.result_data_type)
        error_type = self._typename(route.error_data_type, void='enc.Empty')
        error_is_void = is_void_type(route.error_data_type)

        if result_is_void:
            task_type = 't.Task'
            apm_result_type = 'void'
        elif route_style == 'download':
            task_type = 't.Task<enc.IDownloadResponse<{0}>>'.format(result_type)
            apm_result_type = 'enc.IDownloadResponse<{0}>'.format(result_type)
        else:
            task_type = 't.Task<{0}>'.format(result_type)
            apm_result_type = result_type

        ctor_args = []
        route_args = []
        if not arg_is_void:
            route_args.append("{0} {1}".format(arg_type, arg_name))
            if is_struct_type(route.arg_data_type):
                ctor_args = self._make_struct_constructor_args(route.arg_data_type)
        if route_style == 'upload':
            route_args.append("io.Stream body")
            if next((c.arg for c in ctor_args if '=' in c.arg), False):
                body_arg = 'io.Stream body = null'
            else:
                body_arg = 'io.Stream body'
            ctor_args.append(ConstructorArg('io.Stream', 'body', body_arg,
                                            '<param name="body">The document to upload</param>'))
       
        async_fn = 'public {0} {1}({2})'.format(task_type, async_name, ', '.join(route_args)) 

        apm_args = route_args + ['sys.AsyncCallback callback', 'object state = null']
        apm_fn = 'public sys.IAsyncResult Begin{0}({1})'.format(public_name, ', '.join(apm_args))

        type_args = (arg_type, result_type, error_type)

        self.emit()
        with self.doc_comment():
            self.emit_summary(route.doc or 'The {0} route'.format(self._name_words(route.name)))
            if not arg_is_void:
                self.emit_xml('The request parameters', 'param', name=arg_name)
            if route_style == 'upload':
                self.emit_xml('The content to upload.', 'param', name='body')
            if result_is_void:
                self.emit_xml('The task that represents the asynchronous send operation.',
                              'returns')
            else:
                self.emit_xml('The task that represents the asynchronous send operation. '
                              'The TResult parameter contains the response from the server.',
                              'returns')
            if not error_is_void:
                self.emit_xml('Thrown if there is an error processing the request; '
                              'This will contain a <see cref="{0}"/>.'.format(error_type),
                              'exception', cref='{1}.ApiException{{TError}}'.format(error_type, self._namespace_name))

        self._generate_obsolete_attribute(route.deprecated, suffix='Async')
        with self.cs_block(before=async_fn):
            args = ['enc.Empty.Instance' if arg_is_void else arg_name]
            if route_style == 'upload':
                args.append('body')
            args.extend([
                '"{0}"'.format(route_host),
                self._route_url(ns.name, route.name, route.version),
                '"{0}"'.format(auth_type),
                self._get_encoder(route.arg_data_type),
                self._get_decoder(route.result_data_type),
                self._get_decoder(route.error_data_type),
            ])

            self.emit('return this.Transport.Send{0}RequestAsync<{1}>({2});'.format(
                self._public_name(route_style),
                ', '.join(type_args),
                ', '.join(args)))

        self.emit()
        with self.doc_comment():
            self.emit_summary('Begins an asynchronous send to the {0} route.'.format(self._name_words(route.name)))
            if not arg_is_void:
                self.emit_xml('The request parameters.', 'param', name=arg_name)
            if route_style == 'upload':
                self.emit_xml('The content to upload.', 'param', name='body')
            self.emit_xml('The method to be called when the asynchronous send is completed.',
                          'param', name='callback')
            self.emit_xml('A user provided object that distinguished this send from other send '
                          'requests.', 'param', name='state')
            self.emit_xml('An object that represents the asynchronous send request.', 'returns')

        self._generate_obsolete_attribute(route.deprecated, prefix='Begin')
        with self.cs_block(before=apm_fn):
            async_args = []
            if not arg_is_void:
                async_args.append(arg_name)
            if route_style == 'upload':
                async_args.append('body')

            self.emit('var task = this.{0}({1});'.format(async_name, ', '.join(async_args)))
            self.emit()
            self.emit('return enc.Util.ToApm(task, callback, state);')

        if len(ctor_args) > (1 if route_style == 'upload' else 0):
            arg_list = [item.arg for item in ctor_args]
            arg_name_list = [item.name for item in ctor_args]

            self.emit()
            with self.doc_comment():
                self.emit_summary(route.doc or 'The {0} route'.format(self._name_words(route.name)))
                for arg in ctor_args:
                    self.emit_wrapped_text(arg.doc)
                if result_is_void:
                    self.emit_xml('The task that represents the asynchronous send operation.',
                                  'returns')
                else:
                    self.emit_xml('The task that represents the asynchronous send operation. '
                                  'The TResult parameter contains the response from the server.',
                                  'returns')
                if not error_is_void:
                    self.emit_xml('Thrown if there is an error processing the request; '
                                  'This will contain a <see cref="{0}"/>.'.format(error_type),
                                  'exception', cref='{1}.ApiException{{TError}}'.format(error_type, self._namespace_name))

            self._generate_obsolete_attribute(route.deprecated, suffix='Async')
            self.generate_multiline_list(
                arg_list,
                before='public {0} {1}'.format(task_type, async_name),
                skip_last_sep=True
            )
            with self.cs_block():
                self.generate_multiline_list(
                    arg_name_list[:-1] if route_style == 'upload' else arg_name_list,
                    before='var {0} = new {1}'.format(arg_name, arg_type),
                    after=';',
                    skip_last_sep=True
                )
                self.emit()
                async_args = [arg_name]
                if route_style == 'upload':
                    async_args.append('body')
                self.emit('return this.{0}({1});'.format(async_name, ', '.join(async_args)))

            self.emit()
            with self.doc_comment():
                self.emit_summary('Begins an asynchronous send to the {0} route.'.format(
                        self._name_words(route.name)))
                for arg in ctor_args:
                    self.emit_wrapped_text(arg.doc)
                self.emit_xml('The method to be called when the asynchronous send is completed.',
                              'param', name='callback')
                self.emit_xml('A user provided object that distinguished this send from other '
                              'send requests.', 'param', name='callbackState')
                self.emit_xml('An object that represents the asynchronous send request.',
                              'returns')

            if next((arg for arg in arg_list if '=' in arg), False):
                arg_list.append('sys.AsyncCallback callback = null')
            else:
                arg_list.append('sys.AsyncCallback callback')
            arg_list.append('object callbackState = null')

            self._generate_obsolete_attribute(route.deprecated, prefix='Begin')
            self.generate_multiline_list(
                    arg_list,
                    before='public sys.IAsyncResult Begin{0}'.format(public_name),
                    skip_last_sep=True)
            with self.cs_block():
                self.generate_multiline_list(
                    arg_name_list[:-1] if route_style == 'upload' else arg_name_list,
                    before='var {0} = new {1}'.format(arg_name, arg_type),
                    after=';',
                    skip_last_sep=True
                )
                self.emit()
                args = [arg_name]
                if route_style == 'upload':
                    args.append('body')
                args.extend(['callback', 'callbackState'])
                self.emit('return this.Begin{0}({1});'.format(public_name, ', '.join(args)))

        self.emit()
        with self.doc_comment():
            self.emit_summary('Waits for the pending asynchronous send to the {0} route to complete'.format(
                    self._name_words(route.name)))
            self.emit_xml('The reference to the pending asynchronous send request', 'param',
                          name='asyncResult')
            if not result_is_void:
                self.emit_xml('The response to the send request', 'returns')
            if not error_is_void:
                self.emit_xml('Thrown if there is an error processing the request; '
                              'This will contain a <see cref="{0}"/>.'.format(error_type),
                              'exception', cref='{1}.ApiException{{TError}}'.format(error_type, self._namespace_name))

        self._generate_obsolete_attribute(route.deprecated, prefix='End')
        with self.cs_block(before='public {0} End{1}(sys.IAsyncResult asyncResult)'.format(
                apm_result_type, public_name)):
            self.emit('var task = asyncResult as {0};'.format(task_type))
            with self.if_('task == null'):
                self.emit('throw new sys.InvalidOperationException();')
            if not result_is_void:
                self.emit()
                self.emit('return task.Result;')
    
    def _generate_obsolete_attribute(self, deprecated, prefix='', suffix=''):
        """
        Generate obsolete attribute for deprecated route.

        Args:
            deprecated (stone.api.DeprecationInfo): The route which deprecates.
            prefix (Union[str, unicode]): The prefix for the route function.
            suffix (Union[str, unicode]): The suffix for the route function.
        """
        if not deprecated:
            return
        
        if not deprecated.by:
            self.emit('[sys.Obsolete("This function is deprecated")]')
        else:
            self.emit('[sys.Obsolete("This function is deprecated, please use {0}{1}{2} instead.")]'
                    .format(prefix, self._public_name(deprecated.by.name), suffix))
