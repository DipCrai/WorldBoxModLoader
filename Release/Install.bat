@echo off
for /f "usebackq delims=" %%i in (
    `@"%systemroot%\system32\mshta.exe" "javascript:var objShellApp = new ActiveXObject('Shell.Application');var Folder = objShellApp.BrowseForFolder(0, 'SELECT WORLDBOX DIRECTORY',1, '::{20D04FE0-3AEA-1069-A2D8-08002B30309D}');try {new ActiveXObject('Scripting.FileSystemObject').GetStandardStream(1).Write(Folder.Self.Path)};catch (e){};close();" ^
    1^|more`
) do set sFolderName=%%i\worldbox_Data\Managed
if defined sFolderName (
    for %%f in (*.dll) do if "%%f" NEQ "SharpMonoInjector.dll" copy "%%f" %sFolderName%
)