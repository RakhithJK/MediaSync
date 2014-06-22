#download the binary
$mediaSyncDownloadUrl = "https://github.com/philoushka/MediaSync/releases/download/0.3/MediaSync.exe"
$mediaSyncLocationDir = "C:\Program Files (x86)\MediaSync\"
$file = "MediaSync.exe"
$mediaSyncLocalFullPath = $mediaSyncLocationDir + $file
#Create the target directory if it doesn't exist
if (!(Test-Path $mediaSyncLocationDir)){
	New-Item $mediaSyncLocationDir -type directory -Force
}
	
$wc = New-Object System.Net.WebClient
$wc.DownloadFile($mediaSyncDownloadUrl,$mediaSyncLocalFullPath )

#create a shortcut for startup
$WshShell = New-Object -comObject WScript.Shell
$Startup =[System.Environment]::GetFolderPath("Startup");
$Shortcut = $WshShell.CreateShortcut("$Startup\MediaSync.lnk")
$Shortcut.TargetPath = $mediaSyncLocalFullPath
$Shortcut.Save()

#run it
Invoke-Item $mediaSyncLocalFullPath