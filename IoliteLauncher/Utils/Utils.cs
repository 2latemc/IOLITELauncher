using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

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

    public static void MoveDirectory(string source, string destination) {
        foreach (var file in Directory.EnumerateFiles(source))
        {
            var dest = Path.Combine(destination, Path.GetDirectoryName(source), Path.GetFileName(file));
            File.Move(file, dest);
        }

        foreach (string directory in Directory.GetDirectories(destination)) {
            var dest = Path.Combine(destination, Path.GetDirectoryName(source) ,Path.GetDirectoryName(directory));
            Directory.Move(directory, dest);
        }
    }
}