@echo off
setlocal enabledelayedexpansion

cd /d %~dp0

echo Cleaning build artifacts and temporary files...

rem Define directories to clean
set "CLEAN_DIRS=.vs bin obj publish logs tmp"

rem Clean directories recursively
for /r %%d in (%CLEAN_DIRS%) do (
    if exist "%%d" (
        echo Removing directory: "%%d"
        rd /s /q "%%d"
    )
)

rem Clean .nettrace files
if exist "*.nettrace" (
    echo Removing .nettrace files...
    del /q *.nettrace
)

echo.
echo Clean completed successfully.
pause
