from __future__ import print_function, unicode_literals

import imp
import os
import os.path
import sys

from babelapi.compiler import Compiler, GeneratorException
from babelapi.babel.tower import InvalidSpec, TowerOfBabel

SPECS = [
#    'dfb.babel',
#    'feed_chat.babel',
    'files.babel',
#    'resellers.babel',
    'users.babel',
    ]

SPEC_DIR = r'c:\src\babel-test\spec'
OUTPUT_DIR = r'c:\src\babel-test\babel\Dropbox.Api'
SOURCE_DIR = r'c:\src\babel-test\babel\generator'

if __name__ == '__main__':
    specs = []
    for spec in SPECS:
        spec_path = os.path.join(SPEC_DIR, spec)
        with open(spec_path) as f:
            specs.append((spec_path, f.read()))

    tower = TowerOfBabel(specs)
    try:
        api = tower.parse()
    except InvalidSpec as e:
        print('%s:%s: error: %s' % (e.path, e.lineno, e.msg), file=sys.stderr)
        sys.exit(1)

    if api is None:
        print('You must fix the parsing errors for code generation to continue',
              file = sys.stderr)
        sys.exit(1)

    generator = os.path.join(SOURCE_DIR, 'csharp.babelg.py')
    if not Compiler.is_babel_generator(generator):
        print("%s: error: Generator '%s' must have a .babelg.py extension." %
              generator, file=sys.stderr)
        sys.exit(1)
    else:
        try:
            generator_module = imp.load_source('user_generator', generator)
        except:
            print('%s: error: Importing generator module raised an exception:' %
                  generator, file=sys.stderr)
            raise

    c = Compiler(api, generator_module, OUTPUT_DIR)
    try:
        c.build()
    except GeneratorException as e:
        print('%s: error: %s raised an exception:\n%s' %
              (generator, e.generator_name, e.traceback),
              file=sys.stderr)
        sys.exit(1)