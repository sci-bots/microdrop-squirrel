# MicroDrop-Squirrel #

Tool to bundle a [MicroDrop executable release][md-exe] as a
[Squirrel][squirrel] Windows app.

<!-- vim-markdown-toc GFM -->

* [Build](#build)
    * [Prerequisites](#prerequisites)
        * [Conda environment](#conda-environment)
    * [Build release](#build-release)
        * [What does the build do?](#what-does-the-build-do)
    * [Version convention](#version-convention)
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

A Squirrel MicroDrop release may be built within [an activated development
Conda environment](#conda-environment) by running the following command:

```sh
python build.py <micrdrop-exe release location> <NuGet MicroDrop release version> [-f]
```

where:

 - `<micrdrop-exe release location>` is either: **(a)** a remote [MicroDrop
   executable release][md-exe] URL, e.g.,
   https://github.com/sci-bots/microdrop-exe/releases/download/v2.34.1rc17/microdrop-2.34.1rc17.exe;
   or **(b)** a local `microdrop-exe/dist` directory (see [`microdrop-exe`
   **README**][exe-build]).
 - `<NuGet MicroDrop release version>` is [a valid NuGet package version
   string][nuget-version], e.g., `2.34.1-rc17alpha0`.  _Note that, for example,
   release candidate NuGet versions require a **”-”** (i.e., hyphen) character
   between the main version and the release candidate tag._
 - `-f` is an optional flag to force overwriting of existing output files

### What does the build do?

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

Example contents of output `Releases` directory after several builds (new
builds are appended to `RELEASES` text file):

```
Releases/
  MicroDrop-2.28.0-rc0alpha0-full.nupkg
  MicroDrop-2.28.1-rc0alpha0-full.nupkg
  MicroDrop-2.28.2-rc0alpha0-full.nupkg
  MicroDrop-2.28.3-rc0alpha0-full.nupkg
  MicroDrop-2.28.3-rc2alpha0-full.nupkg
  MicroDrop-2.28.3-rc4alpha0-full.nupkg
  MicroDrop-2.30.0-rc1alpha0-full.nupkg
  MicroDrop-2.30.0-rc3alpha0-full.nupkg
  MicroDrop-2.30.0-rc4alpha0-full.nupkg
  MicroDrop-2.32.0-rc1alpha0-full.nupkg
  MicroDrop-2.32.0-rc2alpha0-full.nupkg
  MicroDrop-2.32.0-rc3alpha0-full.nupkg
  MicroDrop-2.32.0-full.nupkg
  MicroDrop-2.34.1-rc16alpha0-delta.nupkg
  MicroDrop-2.34.1-rc16alpha0-full.nupkg
  RELEASES
  Setup.exe
```

[exe-build]: https://github.com/sci-bots/microdrop-exe#build
[nuget-version]: https://docs.microsoft.com/en-us/nuget/reference/package-versioning#pre-release-versions

## Version convention

When assigning a version to a Squirrel MicroDrop release, we currently use the
following conventions:

 - Pre-release Squirrel MicroDrop builds:
   ```
   microdrop-<patched micrdrop-exe version>alpha<i>
   ```
   - For example, for `microdrop-exe` version `microdrop-2.34.1rc16` _(note the
     hyphen added between the main version and the release candidate tag to
     comply with the [NuGet version scheme][nuget-version])_:
     ```
     microdrop-2.34.1-rc16alpha0
     microdrop-2.34.1-rc16alpha1
     ...
     ```
 - Squirrel MicroDrop release builds:
   ```
   microdrop-<patched micrdrop-exe version>
   ```
   - For example:
     * For `microdrop-exe` version `microdrop-2.34.1rc16`:
       ```
       microdrop-2.34.1-rc16
       ```
     * For `microdrop-exe` version `microdrop-2.34.1`:
       ```
       microdrop-2.34.1
       ```

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
