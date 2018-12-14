@ECHO OFF
CLS

PUSHD %~dp0

:: CS
SET SRC=..\..\..\..\SolARWrapper\out\csharp
SET DST=Wrapper

RMDIR /S /Q %DST%
MKDIR %DST%

COPY "%SRC%\*.cs" %DST%

:: DLL
SET VER=%1
IF "%VER%" EQU "" (
SET VER=release
)

SET EXT=dll
::SET EXT=*

SET DST=.

PUSHD Plugins

DEL *.dll *.lib *.pdb *.exp

:: ???
COPY "%BCOMDEVROOT:/=\%\thirdParties\opencv\3.4.3\lib\x86_64\shared\%VER%\opencv_world343.%EXT%" %DST%

:: Modules
COPY "%BCOMDEVROOT:/=\%\bcomBuild\SolARModuleFBOW\0.5.0\lib\x86_64\shared\%VER%\SolARModuleFBOW.%EXT%" %DST%
COPY "%BCOMDEVROOT:/=\%\bcomBuild\SolARModuleNonFreeOpenCV\0.5.0\lib\x86_64\shared\%VER%\SolARModuleNonFreeOpenCV.%EXT%" %DST%
COPY "%BCOMDEVROOT:/=\%\bcomBuild\SolARModuleOpenCV\0.5.0\lib\x86_64\shared\%VER%\SolARModuleOpenCV.%EXT%" %DST%
COPY "%BCOMDEVROOT:/=\%\bcomBuild\SolARModuleOpenGL\0.5.0\lib\x86_64\shared\%VER%\SolARModuleOpenGL.%EXT%" %DST%
COPY "%BCOMDEVROOT:/=\%\bcomBuild\SolARModuleTools\0.5.0\lib\x86_64\shared\%VER%\SolARModuleTools.%EXT%" %DST%

COPY "%BCOMDEVROOT:/=\%\thirdParties\boost\1.68.0\lib\x86_64\shared\%VER%\boost_context.%EXT%" %DST%
COPY "%BCOMDEVROOT:/=\%\thirdParties\boost\1.68.0\lib\x86_64\shared\%VER%\boost_date_time.%EXT%" %DST%
COPY "%BCOMDEVROOT:/=\%\thirdParties\boost\1.68.0\lib\x86_64\shared\%VER%\boost_fiber.%EXT%" %DST%
COPY "%BCOMDEVROOT:/=\%\thirdParties\boost\1.68.0\lib\x86_64\shared\%VER%\boost_filesystem.%EXT%" %DST%
COPY "%BCOMDEVROOT:/=\%\thirdParties\boost\1.68.0\lib\x86_64\shared\%VER%\boost_log.%EXT%" %DST%
COPY "%BCOMDEVROOT:/=\%\thirdParties\boost\1.68.0\lib\x86_64\shared\%VER%\boost_system.%EXT%" %DST%
COPY "%BCOMDEVROOT:/=\%\thirdParties\boost\1.68.0\lib\x86_64\shared\%VER%\boost_thread.%EXT%" %DST%
::COPY "%BCOMDEVROOT:/=\%\thirdParties\boost\1.68.0\lib\x86_64\shared\%VER%\*.%EXT%" %DST%

COPY "%BCOMDEVROOT:/=\%\thirdParties\xpcf\2.1.0\lib\x86_64\shared\%VER%\xpcf.%EXT%" %DST%

COPY "%BCOMDEVROOT:/=\%\bcomBuild\SolARFramework\0.5.0\lib\x86_64\shared\%VER%\SolARFramework.%EXT%" %DST%

COPY "%BCOMDEVROOT:/=\%\..\SolARFramework\build\%VER%\SolARWrapper\SolARWrapper.%EXT%" %DST%

POPD

POPD
