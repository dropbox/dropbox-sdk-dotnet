from __future__ import print_function, unicode_literals

import imp
import os
import os.path
import sys

from stone.compiler import Compiler, GeneratorException
from stone.stone.tower import InvalidSpec, TowerOfStone

SPECS = [
#    'dfb.stone',
#    'feed_chat.stone',
    'common.stone',
    'files.stone',
    'sharing.stone',
#    'resellers.stone',
    'users.stone',
    ]

SPEC_DIR = os.path.join('c:\\','Users','Dropbox','Dropbox (Dropbox)','ApiSpec')
OUTPUT_DIR = os.path.join('..\\','Dropbox.Api')
SOURCE_DIR = os.path.join('.\\')

if __name__ == '__main__':
    specs = []
    for spec in SPECS:
        spec_path = os.path.join(SPEC_DIR, spec)
        with open(spec_path) as f:
            specs.append((spec_path, f.read()))

    tower = TowerOfStone(specs)
    try:
        api = tower.parse()
    except InvalidSpec as e:
        print('%s:%s: error: %s' % (e.path, e.lineno, e.msg), file=sys.stderr)
        sys.exit(1)

    if api is None:
        print('You must fix the parsing errors for code generation to continue',
              file = sys.stderr)
        sys.exit(1)

    generator = os.path.join(SOURCE_DIR, 'csharp.stoneg.py')
    if not Compiler.is_stone_generator(generator):
        print("%s: error: Generator '%s' must have a .stoneg.py extension." %
              generator, file=sys.stderr)
        sys.exit(1)
    else:
        try:
            generator_module = imp.load_source('user_generator', generator)
        except:
            print('%s: error: Importing generator module raised an exception:' %
                  generator, file=sys.stderr)
            raise

    c = Compiler(api, generator_module, None, OUTPUT_DIR)
    try:
        c.build()
    except GeneratorException as e:
        print('%s: error: %s raised an exception:\n%s' %
              (generator, e.generator_name, e.traceback),
              file=sys.stderr)
        sys.exit(1)