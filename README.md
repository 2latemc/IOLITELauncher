# IOLITE - Voxel Game Engine Launcher
# Unofficial Launcher for the [IOLITE - Voxel Game Engine](https://iolite-engine.com/)

*This launcher is in early alpha. Make sure to backup all your projects before working with this.*

## Installation:
- Download the [latest release](https://github.com/2latemc/ioliteLauncher/releases/latest) of the launcher & launch the setup
- Make sure you have a **fresh** install of the engine. Otherwise things will break.
- Make sure you are running on the release [V.0.4.8](https://media.missing-deadlines.com/iolite/builds/release/iolite-v0.4.8.exe) of the Engine as things might not work with later engine versions (Currently (20.02.2024) this is the latest release)
- **Never** open the Iolite.exe manually, always open a proejct through the launcher
- If you select an Engine install with template project, those will be converted in their own project stored at the first element of your project path list.

## How it works:
The launcher copies the per project information to a by the user specified directory.

**Important:** Adding new data sources while the project is opened will cause the newly added data source to not be moved back into the projects folder, make sure to only add new folders in the project root when the project is not opened. You can however add new assets etc. to data source while a project is opened.

## Migrating existing projects
1. Make a new directory inside one of your in launcher selected project paths (e.g. C:\Users\2late\Documents\IoliteProjects)
2. Select all data sourches from your engine directory (e.g. C:\Files\IOLITE) and move them into the newly created folder.
! Data sourches are the folders such as the "default" one. If you are not sure which one to move, check the app_metadata.json located inside of the engine Directory !
3. Move the app_metadata.json file into the newly created folder
4. If your launcher has the correct projects path selected in the settings your project should show up in the list

## Uninstalling
Run the UninstallIoliteLauncher.exe file located in the Applications directory.
Make sure to also uninstall the engine & projects if you no longer need them.

## Self compiling
1. Publish the solution using your fav. Code Editor (Such as JetbrainsRider / Visual Studio)
2. Make sure you copy over the DefaultProject Directory located in Setup/DefaultProject into %appdata%\IoliteLauncher (Path should look something like this: C:\Users\2late\AppData\Roaming\IoliteLauncher\DefaultProject)

## Todos:
- ~~Project Creation~~
- ~~Project Opening~~
- ~~ProjectDeletion~~
- Project Renaming
- ~~Fix horrible UI~~
- Per project plugin support

Launcher settings saved at %appdata%\IoliteLauncher
