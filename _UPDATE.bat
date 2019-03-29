::Usage: script.bat [debug|release(default)]

@ECHO OFF
CLS

PUSHD %~dp0

SET VERSION=0.5.1

:: CS
ECHO Deploy wrapper scripts

SET SRC="%BCOMDEVROOT%\..\SolARFramework\sources\SolARFramework\SolARWrapper\out\csharp"
SET DST="Wrapper"

RMDIR /S /Q %DST%
MKDIR %DST%
ROBOCOPY %SRC% %DST% /MIR

:: DLL
ECHO Deploy wrapper DLL

SET MODE=%1
IF "%MODE%" EQU "" (
SET MODE=release
)
ECHO MODE = %MODE%

::SET EXT=dll
SET EXT=*

SET DST="Plugins"

RMDIR /S /Q %DST%
MKDIR %DST%

:: SolAR
1>NUL COPY "%BCOMDEVROOT%\..\SolARFramework\build\%MODE%\SolARWrapper\SolARWrapper.%EXT%" %DST%
1>NUL COPY "%BCOMDEVROOT%\bcomBuild\SolARFramework\%VERSION%\lib\x86_64\shared\%MODE%\SolARFramework.%EXT%" %DST%
1>NUL COPY "%BCOMDEVROOT%\thirdParties\xpcf\2.1.0\lib\x86_64\shared\%MODE%\xpcf.%EXT%" %DST%
1>NUL COPY "%BCOMDEVROOT%\thirdParties\opencv\3.4.3\lib\x86_64\shared\%MODE%\opencv_world343.%EXT%" %DST%

:: Modules
1>NUL COPY "%BCOMDEVROOT%\bcomBuild\SolARModuleFBOW\%VERSION%\lib\x86_64\shared\%MODE%\SolARModuleFBOW.%EXT%" %DST%
1>NUL COPY "%BCOMDEVROOT%\bcomBuild\SolARModuleNonFreeOpenCV\%VERSION%\lib\x86_64\shared\%MODE%\SolARModuleNonFreeOpenCV.%EXT%" %DST%
1>NUL COPY "%BCOMDEVROOT%\bcomBuild\SolARModuleOpenCV\%VERSION%\lib\x86_64\shared\%MODE%\SolARModuleOpenCV.%EXT%" %DST%
1>NUL COPY "%BCOMDEVROOT%\bcomBuild\SolARModuleOpenGL\%VERSION%\lib\x86_64\shared\%MODE%\SolARModuleOpenGL.%EXT%" %DST%
1>NUL COPY "%BCOMDEVROOT%\bcomBuild\SolARModuleTools\%VERSION%\lib\x86_64\shared\%MODE%\SolARModuleTools.%EXT%" %DST%

:: Boost
1>NUL COPY "%BCOMDEVROOT%\thirdParties\boost\1.68.0\lib\x86_64\shared\%MODE%\boost_context.%EXT%" %DST%
1>NUL COPY "%BCOMDEVROOT%\thirdParties\boost\1.68.0\lib\x86_64\shared\%MODE%\boost_date_time.%EXT%" %DST%
1>NUL COPY "%BCOMDEVROOT%\thirdParties\boost\1.68.0\lib\x86_64\shared\%MODE%\boost_fiber.%EXT%" %DST%
1>NUL COPY "%BCOMDEVROOT%\thirdParties\boost\1.68.0\lib\x86_64\shared\%MODE%\boost_filesystem.%EXT%" %DST%
1>NUL COPY "%BCOMDEVROOT%\thirdParties\boost\1.68.0\lib\x86_64\shared\%MODE%\boost_log.%EXT%" %DST%
1>NUL COPY "%BCOMDEVROOT%\thirdParties\boost\1.68.0\lib\x86_64\shared\%MODE%\boost_system.%EXT%" %DST%
1>NUL COPY "%BCOMDEVROOT%\thirdParties\boost\1.68.0\lib\x86_64\shared\%MODE%\boost_thread.%EXT%" %DST%

POPD
