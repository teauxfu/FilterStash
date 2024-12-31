# Define the path to the folder
$folderPath = "..\PoE2FilterManager\bin\installer"

# Check if the folder exists and delete it if it does
if (Test-Path $folderPath) {
    Write-Host "Folder exists. Deleting $folderPath..."
    Remove-Item -Path $folderPath -Recurse -Force
} else {
    Write-Host "Folder $folderPath does not exist."
}
# Run the dotnet publish command
dotnet publish "..\PoE2FilterManager\PoE2FilterManager.csproj" -f net9.0-windows10.0.19041.0 -c Release -p:RuntimeIdentifierOverride=win10-x64 -p:WindowsPackageType=None

# Check if the publish was successful
if ($?) {
    # Run the Inno Setup Compiler (ISCC) with the .iss script
    & "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" "./package.iss"
} else {
    Write-Host "Dotnet publish failed, Inno Setup will not run."
}
