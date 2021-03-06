#!/usr/bin/env python
from __future__ import absolute_import, division, print_function

import glob
import os
import shutil
import subprocess


def main():
    """The entry point for the program."""
    generator_path = os.path.abspath('generator')
    spec_path = os.path.abspath('spec')
    specs = glob.glob(spec_path + '/*.stone')  # Arbitrary sorting
    specs.sort()

    repo_path = os.path.abspath('dropbox-sdk-dotnet')
    print('Generating Stone types')
    try:
        shutil.rmtree(os.path.join(repo_path, 'Dropbox.Api', 'Generated'))
    except OSError:
        pass
    try:
        subprocess.check_output(
            (['python', '-m', 'stone.cli', '--filter-by-route-attr', 'alpah_group=null', '-a:all', generator_path + '/csharp.stoneg.py'] +
             [os.path.join(repo_path, 'Dropbox.Api')] + specs))
    except subprocess.CalledProcessError as e:
        print(e.output)

if __name__ == '__main__':
    main()
