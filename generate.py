#!/usr/bin/env python
from __future__ import absolute_import, division, print_function

import glob
import os
import shutil
import subprocess


def main():
    """The entry point for the program."""
    
    stone_path = os.path.abspath('stone')
    repo_path = 'dropbox-sdk-dotnet'
    print('Generating Stone types')
    try:
        shutil.rmtree(os.path.join(repo_path, 'Dropbox.Api', 'Generated'))
    except OSError:
        pass
    try:
        subprocess.check_output(
            (['python', '-m', 'stone.cli', '--filter-by-route-attr', 'alpah_group=null', '-a:all', 'generator/csharp.stoneg.py'] +
             [os.path.join(repo_path, 'Dropbox.Api')] + glob.glob('spec/*.stone')),
            cwd=stone_path)
    except subprocess.CalledProcessError as e:
        print(e.output)

if __name__ == '__main__':
    main()
