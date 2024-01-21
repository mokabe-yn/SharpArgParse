# powershell.exe -ExecutionPolicy RemoteSigned .\thisfile.ps1

Set-Location $(Split-Path -Parent $MyInvocation.MyCommand.Path)

$dll = Get-ChildItem -File -Recurse -Include SharpArgParse.dll | 
    sort LastWriteTime |
    select -last 1

$vinfo = $dll.VersionInfo
$v = @($vinfo.FileMajorPart, 
       $vinfo.FileMinorPart, 
       $vinfo.FileBuildPart) -join "."

echo "target version is $v ($dll)"
$version_tmp = New-TemporaryFile
echo "SharpArgParse version $v" "" >> $version_tmp

$merger = Get-ChildItem -File -Recurse -Include MergeSharpSource.exe | 
    sort LastWriteTime |
    select -last 1

$dst = "SharpArgParse.cs"
Start-Process -FilePath $merger -Wait -NoNewWindow -ArgumentList @(
    "--embed-text", "$version_tmp",
    "--embed-text", "COPYING.MIT",
    "SharpArgParse",
    "-r",
    "--exclude", "bin",
    "--exclude", "obj",
    "-o", $dst)

Remove-Item $version_tmp
echo "output to $dst"
