#!/usr/bin/env python
from __future__ import absolute_import, division, print_function, unicode_literals

import argparse
import errno
import glob
import os
import shutil
import subprocess
import sys

REMOTE_ORIGIN_URLS = (
    'git@github.com:dropbox/dropbox-sdk-dotnet.git',
    'ssh://git@github.com/dropbox/dropbox-sdk-dotnet.git',
    'https://github.com/dropbox/dropbox-sdk-dotnet.git',
)

cmdline_desc = """\
Updates a local checkout of the public .NET SDK on GitHub (dropbox-sdk-dotnet).
"""

_cmdline_parser = argparse.ArgumentParser(description=cmdline_desc)
_cmdline_parser.add_argument(
    '-v',
    '--verbose',
    action='store_true',
    help='Print debugging statements.',
)
_cmdline_parser.add_argument(
    'repo_path',
    type=str,
    help='Path to a checkout of the dropbox-sdk-dotnet repo.',
)

def check_remote_origin_url(repo_path, accepted_remote_origin_urls):
    """
    If the remote origin url of the specified repo is not found in
    accepted_remote_origin_urls, prints an error to stderr and exits.

    Args:
        repo_path (str): Path to a git repository.
        accepted_remote_origin_urls (List[str])
    """
    stdout = subprocess.check_output(
        ['git', 'config', '--local', '--get', 'remote.origin.url'],
        cwd=repo_path)
    remote_url = stdout.rstrip()
    if remote_url not in accepted_remote_origin_urls:
        print('error: The repo (%s) has an unexpected remote origin url:' %
              repo_path, file=sys.stderr)
        print('    %s' % remote_url)
        print('Expected either:')
        for url in accepted_remote_origin_urls:
            print('    %s' % url, file=sys.stderr)
        sys.exit(1)

def main():
    """The entry point for the program."""

    args = _cmdline_parser.parse_args()
    repo_path = args.repo_path
    verbose = args.verbose

    # Sanity check repository path.
    if not os.path.exists(repo_path):
        print('error: The repo folder (%s) does not exist.' % repo_path,
              file=sys.stderr)
        sys.exit(1)
    if not os.path.isdir(repo_path):
        print('error: The repo path (%s) must be a folder.' % repo_path,
              file=sys.stderr)
        sys.exit(1)

    # Check that repo path points to the top-level of the repo
    if not os.path.exists(os.path.join(repo_path, '.git')):
        print('error: The repo folder (%s) is not the top-level of the '
              'public SDK repo.' % repo_path)
        sys.exit(1)

    check_remote_origin_url(repo_path, REMOTE_ORIGIN_URLS)

    if verbose:
        print('Generating code')
    shutil.rmtree(os.path.join(repo_path, 'Dropbox.Api'))
    subprocess.check_output(
        (['python', '-m', 'babelapi.cli', 'generator/csharp.babelg.py'] +
         glob.glob('spec/*.babel') + [os.path.join(repo_path, 'Dropbox.Api')]))

    if verbose:
        print('Copying Dropbox.Api.sln, examples, license, and readme')
    shutil.copy('dropbox-sdk-dotnet/Dropbox.Api.sln', repo_path)
    shutil.copy('dropbox-sdk-dotnet/LICENSE', repo_path)
    shutil.copy('dropbox-sdk-dotnet/README.md', repo_path)

    if verbose:
        print('Removing examples and copying new ones')
    try:
        shutil.rmtree(os.path.join(repo_path, 'Examples'))
    except OSError as e:
        if e.errno != errno.ENOENT:
            raise
    shutil.copytree('dropbox-sdk-dotnet/Examples',
                    os.path.join(repo_path, 'Examples'))

if __name__ == '__main__':
    main()

