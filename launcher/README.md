# MicroDrop launcher

This (simple) C# application launches the executable at `app/MicroDrop.exe`.

As a C# application, it acts as a [Squirrel-aware][squirrel-aware] an adapter
for MicroDrop (a Py2Exe application).

------------------------------------------------------------------------

# Build

Run:

```sh
.\msbuild.bat
```

This will build the application and write the result to:

```
bin\Release
```

------------------------------------------------------------------------

[squirrel-aware]: https://github.com/Squirrel/Squirrel.Windows/blob/250fe4ce09035a682d90836f8c89097760638f66/docs/using/custom-squirrel-events.md#making-your-app-squirrel-aware
