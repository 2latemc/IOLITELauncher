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
    public static bool IsMetaDataFile(string file) {
        return Path.GetFileName(file).ToLower().Equals(Statics.MetadataFileName);
    }

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

    public static (bool success, ProjectsManager.ProjectDataStorage? projectDataStorage) MoveDirectoryContents(string source, string destination) {
        var dirs = Directory.GetDirectories(source);
        var files = Directory.GetFiles(source);

        ProjectsManager.ProjectDataStorage storage = new ProjectsManager.ProjectDataStorage(new List<string>(), new List<string>(), source);
        try {
            foreach (string file in files) {
                File.Move(file, Path.Combine(destination, Path.GetFileName(file)));
                storage.AffectedFiles.Add(Path.GetFileName(file));
            }

            foreach (string dir in dirs) {
                var destiPath = Path.Combine(destination, Path.GetFileName(dir));
                Directory.Move(dir, destiPath);
                storage.AffectedDirs.Add(Path.GetFileName(dir));
            }

            return (true, storage);
        }
        catch (Exception e) {
            Debug.WriteLine(e);
            return (false, null);
        }
    }
}