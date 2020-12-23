import argparse
import imp
import os

from csharp import _CSharpGenerator

cmdline_desc = """\
Generate .NET project for Dropbox Api.
"""

_cmdline_parser = argparse.ArgumentParser(description=cmdline_desc)


class DropboxCSharpGenerator(_CSharpGenerator):
    DEFAULT_NAMESPACE = 'Dropbox.Api'
    DEFAULT_APP_NAME = 'Dropbox'

    cmdline_parser = _cmdline_parser

    def __init__(self, *args, **kwargs):
        super().__init__(self.DEFAULT_NAMESPACE, self.DEFAULT_APP_NAME, *args, **kwargs)

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
                    with self.xml_block('member', name='N:{}'.format(self.DEFAULT_NAMESPACE)):
                        self.emit_summary('Contains the dropbox client - '
                                          '<see cref="T:{}.{}Client"/>.'.format(self.DEFAULT_NAMESPACE,
                                                                                  self.DEFAULT_APP_NAME))
                    for namespace in api.namespaces.values():
                        ns_name = self._public_name(namespace.name)
                        with self.xml_block('member', name='N:{}.{}'.format(
                                self.DEFAULT_NAMESPACE, ns_name)):
                            doc = namespace.doc or ('Contains the types used by the routes declared in '
                                                    '<see cref="T:{0}.{1}.Routes.{1}Routes" />.'.format(
                                                        self.DEFAULT_NAMESPACE, ns_name))
                            self.emit_summary(doc)
                        with self.xml_block('member', name='N:{}.{}.Routes'.format(
                                self.DEFAULT_NAMESPACE, ns_name)):
                            self.emit_summary('Contains the routes for the <see cref="N:{}.{}" /> '
                                              'namespace.'.format(self.DEFAULT_NAMESPACE, ns_name))

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

        with self.output_to_relative_path('{}.cs'.format(exception_type)):
            self.auto_generated()
            with self.namespace():
                self.emit('using sys = System;')
                self.emit()
                self.emit('using {}.{};'.format(self.DEFAULT_NAMESPACE, ns_name))
                self.emit()

                with self.doc_comment():
                    self.emit_summary(doc_string)
                with self.class_(exception_type, access='public sealed partial',
                                 inherits=['StructuredException<{}>'.format(error_name)]):
                    with self.doc_comment():
                        self.emit_summary('Decode from given json.')
                    with self.cs_block(before='internal static {0} Decode(string json, sys.Func<{0}> exceptionFunc)'
                            .format(exception_type)):
                        self.emit('return StructuredException<{0}>.Decode<{1}>(json, {0}.Decoder, exceptionFunc);'
                                  .format(error_name, exception_type))

del _CSharpGenerator
