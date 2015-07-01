setlocal
pushd %~dp0
set msBuildDir=%WINDIR%\Microsoft.NET\Framework\v4.0.30319

call %msBuildDir%\msbuild.exe  ClientKit\ClientKit.csproj /p:Configuration=Release20 /p:OutputPath=..\Managed-OSVR-Unity /p:NativeOutputSuffix32=x86 /p:NativeOutputSuffix64=x86_64%*
endlocal
