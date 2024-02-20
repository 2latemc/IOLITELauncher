using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using IoliteLauncher.Backend.Core;

namespace IoLiteLauncher.Utils;

public class Utils {
    public static bool DoesDirContainFile(string path, string fileName) {
        try {
            foreach (var file in Directory.GetFiles(path)) {
                if (Path.GetFileName(file) == fileName) return true;
            }

            return false;
        }
        catch (Exception e) {
            MessageBox.Show("DoesDirContainFile threw an Exception: " + e.Message);
            return false;
        }
    }

    public static bool DoesDirContainFiles(string path, List<string> fileNames) {
        foreach (string fileName in fileNames) {
            if (DoesDirContainFile(path, fileName)) return true;
        }

        return false;
    }

    public static async Task WaitTask(int delayInSeconds) {
        await Task.Delay(delayInSeconds * 1000);
        return;
    }

    public static (bool success, ProjectsManager.ProjectDataStorage? projectDataStorage) MoveDirectoryContents(
        string source, string destination, bool copy = false) {
        var dirs = Directory.GetDirectories(source);
        var files = Directory.GetFiles(source);

        ProjectsManager.ProjectDataStorage storage =
            new ProjectsManager.ProjectDataStorage(new List<string>(), new List<string>(), source);
        try {
            foreach (string file in files) {
                if (!copy)
                    File.Move(file, Path.Combine(destination, Path.GetFileName(file)));
                else {
                    File.Copy(file, Path.Combine(destination, Path.GetFileName(file)));
                }

                storage.AffectedFiles.Add(Path.GetFileName(file));
            }

            foreach (string dir in dirs) {
                var destiPath = Path.Combine(destination, Path.GetFileName(dir));
                if (!copy)
                    Directory.Move(dir, destiPath);
                else {
                    CopyDirectory(dir, destiPath, true);
                }
                storage.AffectedDirs.Add(Path.GetFileName(dir));
            }

            return (true, storage);
        }
        catch (Exception e) {
            Debug.WriteLine(e);
            return (false, null);
        }
    }


    static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
    {
        // Get information about the source directory
        var dir = new DirectoryInfo(sourceDir);

        // Check if the source directory exists
        if (!dir.Exists)
            throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

        // Cache directories before we start copying
        DirectoryInfo[] dirs = dir.GetDirectories();

        // Create the destination directory
        Directory.CreateDirectory(destinationDir);

        // Get the files in the source directory and copy to the destination directory
        foreach (FileInfo file in dir.GetFiles())
        {
            string targetFilePath = Path.Combine(destinationDir, file.Name);
            file.CopyTo(targetFilePath);
        }

        // If recursive and copying subdirectories, recursively call this method
        if (recursive)
        {
            foreach (DirectoryInfo subDir in dirs)
            {
                string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                CopyDirectory(subDir.FullName, newDestinationDir, true);
            }
        }
    }
}

public static class ProcessExtensions
{
    public static bool IsRunning(this Process process) {
        if (process == null) return false;

        try
        {
            Process.GetProcessById(process.Id);
        }
        catch (ArgumentException)
        {
            return false;
        }
        return true;
    }
}