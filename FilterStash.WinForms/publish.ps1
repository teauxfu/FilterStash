# Define the path to the folder
$folderPath = "..\PoE2FilterManager\bin\Release\net9.0-windows10.0.19041.0\win10-x64\publish\installer"

# Check if the folder exists and delete it if it does
#if (Test-Path $folderPath) {
#    Write-Host "Folder exists. Deleting $folderPath..."
#    Remove-Item -Path $folderPath -Recurse -Force
#} else {
#    Write-Host "Folder $folderPath does not exist."
#}
# Run the dotnet publish command
dotnet publish "..\FilterStash.WinForms\FilterStash.WinForms.csproj" -f net9.0-windows10.0.19041.0 -c Release --no-self-contained   -p:RuntimeIdentifierOverride=win10-x64 -p:WindowsPackageType=None

# Check if the publish was successful
if ($?) {
    # Run the Inno Setup Compiler (ISCC) with the .iss script
    & "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" "./package.iss"

    if($?)
    {
       # if the compile step was successful, upload it to a new github release using the gh cli 
       # Extract the version from Directory.Build.props
       [xml]$props = Get-Content "..\Directory.Build.props"
       $version = $props.Project.PropertyGroup.Version

       # if the compile step was successful, upload it to a new github release using the gh cli 
       gh release create "v$version" "bin\Release\win10installer\filterstash-v$version.exe" --title "FilterStash v$version" --prerelease --notes "See the changelog for details"
    }

} else {
    Write-Host "Dotnet publish failed, Inno Setup will not run."
}
