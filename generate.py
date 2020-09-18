#!/usr/bin/env python
from __future__ import absolute_import, division, print_function

import glob
import os
import shutil
import subprocess


def main():
    """The entry point for the program."""
    stone_path = os.path.abspath('stone')
    print(stone_path)
    generator_path = os.path.abspath('generator')
    print(generator_path)
    spec_path = os.path.abspath('spec')
    print(spec_path)
    specs = glob.glob(spec_path + '/*.stone')  # Arbitrary sorting
    specs.sort()
    print(specs)

    repo_path = os.path.abspath('dropbox-sdk-dotnet')
    print('Generating Stone types')
    try:
        shutil.rmtree(os.path.join(repo_path, 'Dropbox.Api', 'Generated'))
    except OSError:
        pass
    try:
        subprocess.check_output(
            (['python', '-m', 'stone.cli', '--filter-by-route-attr', 'alpah_group=null', '-a:all', generator_path + '/csharp.stoneg.py'] +
             [os.path.join(repo_path, 'Dropbox.Api')] + specs),
            cwd=stone_path)
    except subprocess.CalledProcessError as e:
        print(e.output)

if __name__ == '__main__':
    main()
