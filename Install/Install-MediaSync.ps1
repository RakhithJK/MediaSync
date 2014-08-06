#1. Download the latest archive
#2. Extract its contents into Program Files x86 \ MediaSync
#3. Create a shortcut into current user's Startup dir
#4. Run the exe
function Expand-ZipFile($file, $destination)
{
    $shell = new-object -com shell.application
    $zip = $shell.NameSpace($file)

  foreach($item in $zip.items()){
    $shell.Namespace($destination).CopyHere($item, 0x14)
  }
  
  If (Test-Path "$destination\[Content_Types].xml"){
	Remove-Item  "$destination\[Content_Types].xml"
	}	
}

function Create-MediaSyncStartupShortcut($targetPath)
{
  $WshShell = New-Object -comObject WScript.Shell
  $Startup =[System.Environment]::GetFolderPath("Startup");
  $Shortcut = $WshShell.CreateShortcut("$Startup\MediaSync.lnk")
  $Shortcut.TargetPath = $targetPath
  $Shortcut.Save()  
}

#1. Download
$mediaSyncDownloadUrl = "https://github.com/philoushka/MediaSync/releases/download/v1.1/MediaSync.zip"
$mediaSyncLocationDir = "${env:ProgramFiles(x86)}" + "\MediaSync\"
$mediaSyncZipLocalFullPath = $mediaSyncLocationDir + "MediaSync.zip"

#Create the target directory if it doesn't exist
if (!(Test-Path $mediaSyncLocationDir)){
	New-Item $mediaSyncLocationDir -type directory -Force
}
	
Write-Host "***Are you running As Administrator?****"
#download the zip
$wc = New-Object System.Net.WebClient
$wc.DownloadFile($mediaSyncDownloadUrl,$mediaSyncZipLocalFullPath)

#extract the zip
Expand-ZipFile $mediaSyncZipLocalFullPath $mediaSyncLocationDir

#create a shortcut for startup
Create-MediaSyncStartupShortcut($mediaSyncLocalFullPath)

#run the exe for the user
Invoke-Item  "$mediaSyncLocationDir\MediaSync.exe"
Write-Host "Done"
