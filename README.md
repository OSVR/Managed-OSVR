# Managed-OSVR
> Maintained at <https://github.com/OSVR/Managed-OSVR>
>
> For details, see <http://osvr.github.io>
>
> For support, see <http://support.osvr.com>

## .NET Binding for OSVR - "Managed-OSVR"
The solution contains the ClientKit wrapper (in both .NET 2.0 and 4.5 versions) and ported examples based on the C++ examples from the core.

## Bundled binary snapshot
Windows native DLLs are bundled in the `osvr-core-snapshots` directory. The are presently from the snapshot identified as:

> `OSVR-Core-Snapshot-v0.1-786-gc9f3bcb`

## Build options
Instead of just "Debug" and "Release" configurations, there are "Debug45", "Release45", "Debug20", and "Release20" configurations - the suffix indicates the version of the .NET Framework being compiled against. The assembly information of a compiled binary indicates which framework was used, and distinct build directories are used as well.

## MSBuild Requirements and Properties
Some MSBuild tweaking was involved in getting the multi-framework targeting and native library copying working. Though the build can produce a .NET 2.0 assembly, you'll need at least 4.0 and the corresponding MSBuild to execute these builds.

For command-line builds, the following properties (centralized in [`MultiFramework.targets`](MultiFramework.targets) and [`CopyNativeLibraries.targets`](CopyNativeLibraries.targets) may be interesting:

- `Configuration` - A standard MSBuild property, but with the distinct meanings discussed above in Build Options, automatically setting `TargetFrameworkVersion`
- `BuildRoot` - The parent directory that will contain `bin` and `obj` - defaults to a directory named `build` in the repository root.
- `NativeRoot` - Defaults to the bundled `osvr-core-snapshots` directory, but you can point it to a similarly-laid-out alternate directory. If you look in the corresponding targets file, you'll see that you can override the locating of the native libraries additionally/instead at each level of granularity, to accommodate any sort of build environment.


## License

This project: Licensed under the Apache License, Version 2.0.