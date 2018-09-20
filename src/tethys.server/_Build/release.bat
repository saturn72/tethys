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
SET compressedFileName=%fileName%.zip
SET source=%CD%\bin\release\netcoreapp2.1\%runtime%\publish\*
SET destDir=%CD%\bin\release\versions\
SET tmpFilePath=%destDir%tmp.zip

ECHO Create versions directory, if not exists
powershell New-Item -ItemType directory -Path %destDir% -Force

ECHO Compress %source% (as zip)to %destDir% 
powershell Compress-Archive -Path %source% -DestinationPath %tmpFilePath% -Force

ECHO Rename zip file to %compressedFileName%
powershell Remove-Item -Path %destDir%%compressedFileName% -Force

powershell Rename-Item -Path %tmpFilePath% -NewName %compressedFileName% -Force

ECHO DONE! artifacts may be found in %destDir%