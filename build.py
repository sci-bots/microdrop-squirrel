from __future__ import print_function
from argparse import ArgumentParser
import logging
import os
import sys
import subprocess as sp

from six.moves.urllib.request import urlretrieve
import path_helpers as ph
import whichcraft


def build(squirrel_version, delta=True):
    logging.info('Build MicroDrop launcher executable')

    cwd = os.getcwd()
    os.chdir('launcher')
    try:
        sp.check_call('msbuild.bat')
    finally:
        os.chdir(cwd)

    logging.info('Download `nuget v3.5.0` (to work around NuGet/Home#7188 and '
                 'NuGet/Home#5016).')
    urlretrieve('https://dist.nuget.org/win-x86-commandline/v3.5.0/nuget.exe', 'nuget.exe')

    logging.info('Create NuGet package.')
    # Strip version suffix (e.g., `rc1`, `a1`, etc.) since `nuget` does not
    # support them.
    sp.check_call(['./nuget.exe', 'pack', './Package.nuspec', '-Version',
                   squirrel_version])

    logging.info('Generate Squirrel release.')

    squirrel_com = whichcraft.which('Squirrel.com')
    if squirrel_com is None:
        # Squirrel.com was not found on system path.  Try to find version from
        # Conda package.
        squirrel_com = whichcraft.which('Squirrel.com',
                                        path=r'%s\Library\usr\bin\squirrel;' %
                                        os.environ['CONDA_PREFIX'])
        if squirrel_com is None:
            # Conda packaged Squirrel.com was not found.
            print('`Squirrel.com` was not found.', file=sys.stderr)
            sys.exit(-1)

    command = [squirrel_com, '--no-msi', '-i', 'launcher/microdrop.ico', '-g',
               'microdrop-installation-splash.gif', '--releasify',
               'MicroDrop.%s.nupkg' % squirrel_version]
    if not delta:
        command.insert(1, '--no-delta')
    logging.info('calling: `%s`', sp.list2cmdline(command))
    sp.check_call(command)
    logging.info('Done')


def download_microdrop_exe(url, output_dir):
    output_dir = ph.path(output_dir)
    url = ph.path(url)
    filename = url.name

    logging.info('Download MicroDrop executable release: `%s` -> '
                 '`microdrop.exe`', url)
    sp.check_call(['curl', '-O', '-L', url])

    logging.info('Extract `%s`', filename)
    sp.check_call(['7za', 'x', filename])

    # Extraction completed.  Remove archive file.
    ph.path(filename).unlink()

    release_dir = ph.path(filename.namebase)
    release_dir.rename(output_dir)


if __name__ == '__main__':
    logging.basicConfig(format='[%(asctime)s] %(message)s', level=logging.INFO,
                        datefmt=r'%Y-%m-%d %H:%M:%S')

    parser = ArgumentParser()

    parser.add_argument('microdrop_exe_source', type=ph.path)
    parser.add_argument('version', help='Override Squirrel app version')
    parser.add_argument('--no-delta', help='Skip generation of Squirrel delta '
                        'release', action='store_true')
    parser.add_argument('-f', '--force', help='Force overwrite of output '
                        'directory.', action='store_true')

    args = parser.parse_args()

    squirrel_app_dir = ph.path('launcher/bin/Release/app')

    if any([args.microdrop_exe_source.startswith(p)
            for p in ('http://','https://')]):
        remote = True
    else:
        remote = False

    if squirrel_app_dir.exists() and (remote or
                                      (args.microdrop_exe_source.realpath() !=
                                       squirrel_app_dir.realpath())):
        if args.force:
            try:
                squirrel_app_dir.unlink()
            except:
                squirrel_app_dir.rmtree()
        else:
            parser.error('Output directory `%s` already exists.  Use '
                         '`--force` to overwrite.' % squirrel_app_dir)

    if remote:
        # Download release from GitHub.
        download_microdrop_exe(args.microdrop_exe_source, squirrel_app_dir)
    elif args.microdrop_exe_source.realpath() != squirrel_app_dir.realpath():
        # MicroDrop executable release directory specified.  Copy it to the
        # Squirrel app tree.
        args.microdrop_exe_source.copytree(squirrel_app_dir)

    build(squirrel_version=args.version, delta=not args.no_delta)
