@ECHO OFF
pushd "%~dp0"
ECHO.
ECHO.
ECHO.
ECHO This script deletes all temporary build files in their
ECHO corresponding BIN and OBJ Folder contained in the following projects
ECHO.
ECHO SuggestBoxLib
ECHO.
ECHO Components\ServiceLocator
ECHO Components\Settings\Settings
ECHO Components\Settings\SettingsModel
ECHO.
ECHO Demos\SuggestBox\SuggestBoxDemo
ECHO Demos\SuggestBox\SuggestBoxTestLib
ECHO Demos\SuggestBox\ThemedSuggestBoxDemo
ECHO.
REM Ask the user if hes really sure to continue beyond this point XXXXXXXX
set /p choice=Are you sure to continue (Y/N)?
if not '%choice%'=='Y' Goto EndOfBatch
REM Script does not continue unless user types 'Y' in upper case letter
ECHO.
ECHO XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
ECHO.
ECHO XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX

RMDIR /S /Q .vs

ECHO.
ECHO Deleting BIN and OBJ Folders in SuggestLib folder
ECHO.
RMDIR /S /Q SuggestBoxLib\bin
RMDIR /S /Q SuggestBoxLib\obj

ECHO.

ECHO Deleting BIN and OBJ Folders in ServiceLocator folder
ECHO.
RMDIR /S /Q Components\ServiceLocator\bin
RMDIR /S /Q Components\ServiceLocator\obj

ECHO Deleting BIN and OBJ Folders in Settings\Settings folder
ECHO.
RMDIR /S /Q Components\Settings\Settings\bin
RMDIR /S /Q Components\Settings\Settings\obj

ECHO Deleting BIN and OBJ Folders in Settings\SettingsModel folder
ECHO.
RMDIR /S /Q Components\Settings\SettingsModel\bin
RMDIR /S /Q Components\Settings\SettingsModel\obj

ECHO.
ECHO Deleting BIN and OBJ Folders in Demos\SuggestBox\SuggestBoxDemo folder
ECHO.
RMDIR /S /Q Demos\SuggestBoxDemo\bin
RMDIR /S /Q Demos\SuggestBoxDemo\obj
ECHO.

ECHO.
ECHO Deleting BIN and OBJ Folders in Demos\SuggestBox\ThemedSuggestBoxDemo folder
ECHO.
RMDIR /S /Q Demos\ThemedSuggestBoxDemo\bin
RMDIR /S /Q Demos\ThemedSuggestBoxDemo\obj
ECHO.

ECHO.
ECHO Deleting BIN and OBJ Folders in Demos\SuggestBox\SuggestBoxTestLib folder
ECHO.
RMDIR /S /Q Demos\SuggestBoxTestLib\bin
RMDIR /S /Q Demos\SuggestBoxTestLib\obj
ECHO.

PAUSE

:EndOfBatch