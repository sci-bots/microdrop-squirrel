# MicroDrop-Squirrel #

Tool to bundle a [MicroDrop executable release][md-exe] as a
[Squirrel][squirrel] Windows app.


Build
=====

 1. Extract a single downloaded [MicroDrop executable release][md-exe] to
    `launcher\bin\Release`.
 2. Rename `microdrop-...` to `app`
 3. Download [`nuget v3.5.0`][nuget-3.5] (to work around NuGet/Home#7188 and
    NuGet/Home#5016).
 4. Create NuGet package:

    ```sh
    nuget pack .\Package.nuspec -NoPackageAnalysis -Verbosity detailed -Version <release version>
    ```

    where `<release version>` matches the version of the downloaded MicroDrop
    release.
 5. Generate Squirrel release:

    ```sh
    Squirrel.com --no-msi --no-delta -i launcher/microdrop.ico -g microdrop-installation-splash.gif --releasify .\MicroDrop.<release version>.nupkg
    ```


[md-exe]: https://github.com/sci-bots/microdrop-exe/releases
[squirrel]: https://github.com/Squirrel/Squirrel.Windows
[nuget-3.5]: https://dist.nuget.org/win-x86-commandline/v3.5.0/nuget.exe
