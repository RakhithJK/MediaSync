[System.Reflection.Assembly]::Load("WindowsBase,Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35")
$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition
$releaseDir = (get-item $scriptPath ).parent.FullName + "\SynologyPictureSync\bin\Release";

$zipFile = $releaseDir + "\MediaSync.zip"
  
If (Test-Path $zipFile){
	Remove-Item $zipFile
}

$ZipPackage=[System.IO.Packaging.ZipPackage]::Open($zipFile,  [System.IO.FileMode]"OpenOrCreate", [System.IO.FileAccess]"ReadWrite")
$files = @("/MediaSync.exe", "/Newtonsoft.Json.dll")

ForEach ($file In $files)
{
   $partName=New-Object System.Uri($file, [System.UriKind]"Relative")
   $part=$ZipPackage.CreatePart($partName, "application/zip", [System.IO.Packaging.CompressionOption]"Maximum")
   $bytes=[System.IO.File]::ReadAllBytes($releaseDir + $file)
   $stream=$part.GetStream()
   $stream.Write($bytes, 0, $bytes.Length)
   $stream.Close()
}

$ZipPackage.Close()
