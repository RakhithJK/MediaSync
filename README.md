MediaSync
===================

I needed a way to sync my digital camera and iPhone photos and videos from my Windows machine to my NAS. I wanted to have a copy of those pictures and videos locally and on the NAS.

I have specific needs in how I like my photos organized on the NAS. I like it in the format `year\month\day\filename.jpg`.

This tray app solves this problem by:

- being a tray app.
- syncing one-way from your PC to your NAS.
- allowing you to specify:
 - the file types you want synced.
 - the directory structure to create on the NAS. 

###How Sync Works

- sits in your system tray
- installed to `%ProgramFiles(x86)%\MediaSync`
- waits for a sync command via right-click
- finds all items that are candidates for sync (by local directory and by file type)
- checks whether that file exists remotely already. 
 - if not, the file is copied.
 - if it exists already, it copies only if the file size is different.
- optionally removes the source files

###Installing

Requirements: 

- Windows 7 or greater.
- .NET 4.5

Run this PowerShell script.
https://github.com/philoushka/MediaSync/blob/master/Install/Install-MediaSync.ps1

###Configuration

There are few configuration items you'll need to set to get working.

Directories could be a drive letter or UNC like:

 -  `E:\`  
 -  `C:\Users\me\Pictures`
 -  `\\server\photos` 
 -  `\\192.168.0.5\photos`.

If you attempt to run a sync without the required items configured, you'll be warned. 

Configuration Items

- Source Directory - the directory on your PC that you want to sync files **from**. Files that are in subdirectories within will be synced. Default is your Pictures directory in your profile.
- Target Directory - the target directory you want your files to sync **to**. Subdirectories and path structure is another upcoming configuration option.
- Delete Items After Sync: whether you want to delete the source items from the source directory **after successful copy to the target directory**.
- Media File Types: a list of file extensions to include in the sync. This list is defaulted with `jpg, jpeg, png, mov, mp4, m4v, mpg, mpeg, avi`. Any file extension that matches this will be copied. This is case-insensitive.
- Warn Me Once Before Deleting: a confirmation nag to ensure that you actually did want to delete the items that have been successfully synced.

### Notes
  
1. If a file with the same name exists in the target directory, its size is checked. If the file size is:
    1. the same, the image is skipped. The file is considered a duplicate, and no sense overwriting the same file over and over again.
    2. different, then the image is copied with a new name. The new name will be like `file.jpg` to `file-1234567.jpg`. If that exists, the file will be skipped as a duplicate.
    
2.      