echo Building Managed-OSVR.sln. If this fails, try running under the visual studio command prompt, or adding devenv.exe to your path.
msbuild Managed-OSVR.proj /t:ClientKit
rmdir /S /Q NuGetPackaging\osvr-core-snapshots
rmdir /S /Q NuGetPackaging\lib
mkdir NuGetPackaging\lib\net20
mkdir NuGetPackaging\lib\net45
xcopy osvr-core-snapshots NuGetPackaging\osvr-core-snapshots /E /I
copy build\bin\net20\Release\OSVR.ClientKit.dll NuGetPackaging\lib\net20
copy build\bin\net20\Release\OSVR.ClientKit.pdb NuGetPackaging\lib\net20
copy build\bin\net45\Release\OSVR.ClientKit.dll NuGetPackaging\lib\net45
copy build\bin\net45\Release\OSVR.ClientKit.pdb NuGetPackaging\lib\net45
cd NuGetPackaging
echo Packing the NuGet package. This will emit some warnings about assemblies that are not in 'lib'. These can be ignored.
nuget pack
start .
cd ..
