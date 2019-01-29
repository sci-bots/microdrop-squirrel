# MicroDrop-Squirrel #

Tool to bundle a [MicroDrop executable release][md-exe] as a
[Squirrel][squirrel] Windows app.

<!-- vim-markdown-toc GFM -->

* [Build](#build)
    * [Prerequisites](#prerequisites)
        * [Conda environment](#conda-environment)
    * [Build release](#build-release)
* [Publish](#publish)

<!-- vim-markdown-toc -->

Build
=====

## Prerequisites

 - Visual Studio. **Tested with [Visual Studio 2017
Community][vs2017]**.
 - Conda environment (see [below](#conda-environment))

### Conda environment

Create a Conda environment with required packages:

```sh
conda env create -n <env name> --file environment.yaml
```

## Build release

From within [an activated development Conda environment](#conda-environment)
run:

```sh
python build.py <micrdrop-exe release URL> <NuGet MicroDrop release version> [-f]
```

where:

 - `<micrdrop-exe release URL>` is the URL of a [MicroDrop executable
   release][md-exe], e.g.,
   https://github.com/sci-bots/microdrop-exe/releases/download/v2.34.1rc17/microdrop-2.34.1rc17.exe
 - `<NuGet MicroDrop release version>` is [a valid NuGet package version
   string][nuget-version], e.g., `2.34.1-rc17alpha0`
 - `-f` is an optional flag to force overwriting of existing output files

The `build.py` script referenced above performs the following steps:

 1. Download the specified [MicroDrop executable release][md-exe].
 2. Package the contents of the [MicroDrop executable release][md-exe] into a
    NuGet package (required by Squirrel).
 3. Generate Squirrel release, e.g., `MicroDrop.2.34.1-rc16alpha0.nupkg`.
 4. Update the `Releases` directory:
  1. Update the `Releases/RELEASES` file with details of the new release.
  2. Copy the Squirrel release into the `Releases` and generate a _"delta"_
     package *(unless `--no-delta` flag was used)*, e.g.,
     `MicroDrop-2.34.1-rc16alpha0-delta.nupkg`.
  3. Update/create `Releases/Setup.exe` as installer for new release.

Publish
=======

Once one or more releases have been built, publish the contents of the
`Releases` directory at a `http://` URL.

When the `Setup.exe` file is launched, the URL used for updates is resolved in
the following order:

 1. `update-url.rc` file located in same directory as installed app `.exe`
 2. `MICRODROP_UPDATE_URL` environment variable.
 3. Default of `https://sci-bots.com/microdrop-releases`.

[md-exe]: https://github.com/sci-bots/microdrop-exe/releases
[squirrel]: https://github.com/Squirrel/Squirrel.Windows
[nuget-3.5]: https://dist.nuget.org/win-x86-commandline/v3.5.0/nuget.exe
[vs2017]: https://visualstudio.microsoft.com/downloads/
