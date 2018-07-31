ECHO OFF
SET runtime=win10-x64
SET version=%1
SET fileName=%2
SET versionSuffix="--version-suffix" %version%

ECHO Publish Tethys
IF "%version%"=="" SET versionSuffix=%
dotnet publish -c release -r %runtime% --verbosity diagnostic %versionSuffix%

ECHO Create Zip
IF "%fileName%"=="" SET fileName=%version%
IF "%fileName%"=="" SET fileName=tethys_%runtime%
SET %fileName%=%fileName%.zip
SET source=%CD%\bin\release\netcoreapp2.1\%runtime%\publish\*
SET destDir=%CD%\bin\release\versions\

ECHO Create versions directory, if not exists
powershell New-Item -ItemType directory -Path %destDir% -Force

ECHO Compress (as zip)to %destDir% 
powershell Compress-Archive -Path %source% -DestinationPath %destDir%%fileName% -Force

ECHO DONE! artifacts may be found in %destDir%