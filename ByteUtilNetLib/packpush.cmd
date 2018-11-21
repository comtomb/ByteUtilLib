del *.nupkg
y:\nuget\nuget pack ByteUtilNetLib.nuspec
for /f "delims=" %%a in ('dir /s /b *.nupkg') do set "PACKAGE=%%a"
y:\nuget\nuget push %PACKAGE% -src thomas_release