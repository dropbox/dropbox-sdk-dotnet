from __future__ import unicode_literals

import argparse
import imp
import os

try:
    from csproj import make_csproj_file
    from csharp import _CSharpGenerator
except ImportError:
    # The babel generate calls imp.load_source on this file, which precludes
    # referencing csproj as if it was part of a module, so we have to jump
    # through this hoop...
    csproj = os.path.join(os.path.dirname(__file__), 'csproj.py')
    csproj_module = imp.load_source('csproj_module', csproj)
    make_csproj_file = csproj_module.make_csproj_file
    csharp = os.path.join(os.path.dirname(__file__), 'csharp.py')
    csharp_module = imp.load_source('csharp_module', csharp)
    _CSharpGenerator = csharp_module._CSharpGenerator

cmdline_desc = """\
Generate .NET project for Dropbox Api.
"""

_cmdline_parser = argparse.ArgumentParser(description=cmdline_desc)
_cmdline_parser.add_argument(
    '-p',
    '--private',
    action='store_true',
    help='Generate private build.',
)

class DropboxCSharpGenerator(_CSharpGenerator):
    DEFAULT_NAMESPACE = 'Dropbox.Api'
    DEFAULT_APP_NAME = 'Dropbox'

    cmdline_parser = _cmdline_parser

    def __init__(self, *args, **kwargs):
        super(DropboxCSharpGenerator, self).__init__(self.DEFAULT_NAMESPACE, self.DEFAULT_APP_NAME, *args, **kwargs)

    def _generate(self, api):
        self._generate_dropbox_auth_exception(api)
        self._generate_csproj()
        self._copy_files('dropbox')

        if self.args.private:
            self._generate_xml_doc(api)
            self._copy_files('private')

    def _generate_xml_doc(self, api):
        """
        Generates an xml documentation file containing the namespace level
        documentation for the API specification being generated.

        Args:
            api (babelapi.api.Api): The API specification.
        """
        with self.output_to_relative_path('namespace_summaries.xml'):
            self.emit('<?xml version="1.0"?>')
            with self.xml_block('doc'):
                with self.xml_block('assembly'):
                    self.emit_xml('_NamespaceSummaries_', 'name')
                with self.xml_block('members'):
                    with self.xml_block('member', name='N:{0}'.format(self.DEFAULT_NAMESPACE)):
                        self.emit_summary('Contains the dropbox client - '
                                '<see cref="T:{0}.{1}Client"/>.'.format(self.DEFAULT_NAMESPACE, self.DEFAULT_APP_NAME))
                    for namespace in api.namespaces.itervalues():
                        ns_name = self._public_name(namespace.name)
                        with self.xml_block('member', name='N:{0}.{1}'.format(
                                self.DEFAULT_NAMESPACE, ns_name)):
                            doc = namespace.doc or ('Contains the types used by the routes declared in '
                                                    '<see cref="T:{0}.{1}.Routes.{1}Routes" />.'.format(
                                                        self.DEFAULT_NAMESPACE, ns_name))
                            self.emit_summary(doc)
                        with self.xml_block('member', name='N:{0}.{1}.Routes'.format(
                                self.DEFAULT_NAMESPACE, ns_name)):
                            self.emit_summary('Contains the routes for the <see cref="N:{0}.{1}" /> '
                                    'namespace.'.format(self.DEFAULT_NAMESPACE, ns_name))

    def _generate_csproj(self):
        """
        Generates two csproj files.

        One is a portable assembly - this is the assembly that is intended to
        be distributed; the other is a regular desktop .Net assembly that is
        used to generate documentation - the documentation tool SandCastle
        cannot reliably generate documentation from a portable assembly.
        """
        files = [f for f in self._generated_files if f.endswith('.cs')]
        modes = ['Portable', 'Portable40']
        if self.args.private:
            # Only generate SandCastle csproj for private build.
            modes.append('Doc')

        for mode in ('Portable', 'Portable40', 'Doc'):
            with self.output_to_relative_path('{0}.{1}.csproj'.format(self.DEFAULT_NAMESPACE, mode)):
                self.emit_raw(make_csproj_file(files, mode=mode, is_private=self.args.private))

    def _generate_dropbox_auth_exception(self, api):
        """
        Generates the auth exception class

        Args:
            api (babelapi.api.Api): The API specification.
        """
        ns = api.namespaces['auth']
        auth_error = self._public_name(ns.data_type_by_name['AuthError'].name)
        ns_name = self._public_name(ns.name)

        with self.output_to_relative_path('AuthException.cs'):
            self.auto_generated()
            with self.namespace():
                self.emit('using sys = System;')
                self.emit()
                self.emit('using {0}.{1};'.format(self.DEFAULT_NAMESPACE, ns_name))
                self.emit()

                with self.doc_comment():
                    self.emit_summary('An HTTP exception that is caused by the server reporting an authentication problem.')
                with self.class_('AuthException', access='public sealed partial',
                                 inherits=['StructuredException<{0}>'.format(auth_error)]):
                    with self.doc_comment():
                        self.emit_summary('Initializes a new instance of the <see cref="AuthException"/> class.')
                    with self.cs_block(before='public AuthException()'):
                        pass
                    self.emit()
                    with self.doc_comment():
                        self.emit_summary('Decode from given json.')
                    with self.cs_block(before='internal static AuthException Decode(string json)'):
                        self.emit('return StructuredException<{0}>.Decode<AuthException>(json, {0}.Decoder);'.format(auth_error))

del _CSharpGenerator
