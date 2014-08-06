#1. Download the latest archive
#2. Extract its contents into Program Files x86 \ MediaSync
#3. Create a shortcut into current user's Startup dir
#4. Run the exe

#1. Download
$mediaSyncDownloadUrl = "https://github.com/philoushka/MediaSync/releases/download/0.3/MediaSync.exe"
$mediaSyncLocationDir = "${env:ProgramFiles(x86)}" + "\MediaSync\\"
$zipFile ="MediaSync.zip" 
$mediaSyncZipLocalFullPath = $mediaSyncLocationDir + $zipFile

#Create the target directory if it doesn't exist
if (!(Test-Path $mediaSyncLocationDir)){
	New-Item $mediaSyncLocationDir -type directory -Force
}
	
#download the zip
$wc = New-Object System.Net.WebClient
$wc.DownloadFile($mediaSyncDownloadUrl,$mediaSyncZipLocalFullPath)

#download its dependants
Expand-ZipFile($mediaSyncZipLocalFullPath, $mediaSyncLocationDir)

#create a shortcut for startup
Create-MediaSyncStartupShortcut($mediaSyncLocalFullPath)

#run the exe for the user
Invoke-Item $mediaSyncLocationDir + "MediaSync.exe"

function Expand-ZipFile($file, $destination)
{
	$shell = new-object -com shell.application
	$zip = $shell.NameSpace($file)
  foreach($item in $zip.items()){
    $shell.Namespace($destination).copyhere($item)
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

