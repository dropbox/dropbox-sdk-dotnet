from __future__ import unicode_literals

import argparse
import imp
import os

try:
    from csproj import make_csproj_file
    from csharp import _CSharpGenerator
except ImportError:
    # The stone generate calls imp.load_source on this file, which precludes
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


class DropboxCSharpGenerator(_CSharpGenerator):
    DEFAULT_NAMESPACE = 'Dropbox.Api'
    DEFAULT_APP_NAME = 'Dropbox'

    cmdline_parser = _cmdline_parser

    def __init__(self, *args, **kwargs):
        super(DropboxCSharpGenerator, self).__init__(self.DEFAULT_NAMESPACE, self.DEFAULT_APP_NAME, *args, **kwargs)

    def _generate(self, api):
        self.emit_summary('An HTTP exception that is caused by the server '
                          'reporting an authentication problem.')
        self._generate_dropbox_exception(api, 'auth', 'AuthError', 'AuthException',
                                         'An HTTP exception that is caused by the server '
                                         'reporting an authentication problem.')

        self._generate_dropbox_exception(api, 'auth', 'RateLimitError', 'RateLimitException',
                                         'An HTTP exception that is caused by the client '
                                         'being rate limited by the server.')

        self._generate_dropbox_exception(api, 'auth', 'AccessError', 'AccessException',
                                         'An HTTP exception that is caused by the account not'
                                         'not having access to the endpoint.')
        
        self._generate_dropbox_exception(api, 'common', 'PathRootError', 'PathRootException',
                                         'An HTTP exception that is caused by invalid'
                                         'Dropbox-Api-Path-Root header.')
                                         
        self._generate_xml_doc(api)

    def _generate_xml_doc(self, api):
        """
        Generates an xml documentation file containing the namespace level
        documentation for the API specification being generated.

        Args:
            api (stone.api.Api): The API specification.
        """
        with self.output_to_relative_path('namespace_summaries.xml'):
            self.emit('<?xml version="1.0"?>')
            with self.xml_block('doc'):
                with self.xml_block('assembly'):
                    self.emit_xml('_NamespaceSummaries_', 'name')
                with self.xml_block('members'):
                    with self.xml_block('member', name='N:{0}'.format(self.DEFAULT_NAMESPACE)):
                        self.emit_summary('Contains the dropbox client - '
                                          '<see cref="T:{0}.{1}Client"/>.'.format(self.DEFAULT_NAMESPACE,
                                                                                  self.DEFAULT_APP_NAME))
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
        modes = [
                 ('Net45', '')]

        for mode, suffix in modes:
            with self.output_to_relative_path(
                    '{0}{1}.csproj'.format(self.DEFAULT_NAMESPACE, suffix), folder=''):
                self.emit_raw(make_csproj_file(files, mode=mode))

    def _generate_dropbox_exception(self, api, namespace, error_type, exception_type,
                                    doc_string):
        """
        Generates a Dropbox global exception class

        Args:
            api (stone.api.Api): The API specification.
            namespace (str): The namespace name for the error type.
            error_type (str): The name of the error type.
            exception_type (str): The exception name in .NET.
            doc_string (str): The doc string for the exception.
        """
        ns = api.namespaces[namespace]
        error_name = self._public_name(ns.data_type_by_name[error_type].name)
        ns_name = self._public_name(ns.name)

        with self.output_to_relative_path('{0}.cs'.format(exception_type)):
            self.auto_generated()
            with self.namespace():
                self.emit('using sys = System;')
                self.emit()
                self.emit('using {0}.{1};'.format(self.DEFAULT_NAMESPACE, ns_name))
                self.emit()

                with self.doc_comment():
                    self.emit_summary(doc_string)
                with self.class_(exception_type, access='public sealed partial',
                                 inherits=['StructuredException<{0}>'.format(error_name)]):
                    with self.doc_comment():
                        self.emit_summary('Decode from given json.')
                    with self.cs_block(before='internal static {0} Decode(string json, sys.Func<{0}> exceptionFunc)'
                            .format(exception_type)):
                        self.emit('return StructuredException<{0}>.Decode<{1}>(json, {0}.Decoder, exceptionFunc);'
                                  .format(error_name, exception_type))

del _CSharpGenerator
