using System;
using System.IO;
using System.Windows;
using Newtonsoft.Json;

namespace IoLiteLauncher.Backend;

public abstract class Serializer {
    public static bool ToFile<T>(string path, T obj) {
        try {
            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string serialized = JsonConvert.SerializeObject(obj, Formatting.Indented);
            using StreamWriter streamWriter = new StreamWriter(path, false);
            streamWriter.Write(serialized);
        }
        catch (Exception e) {
            MessageBox.Show(e.Message);
            return false;
        }
        return true;
    }
}