using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace IoLiteLauncher.Backend;

public class SettingsManager {
    public Settings Settings = new Settings();

    private string settingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "IOLauncher", "settings.json");
    public void Save() {
        bool success = Serializer.ToFile(settingsPath, Settings);
        if (success) {
            Debug.WriteLine("Settings saved");
        }
        else {
            Debug.WriteLine("Could not save settings.");
        }
    }
}

public struct Settings {
    public List<string> projectsPath;
    public string enginePath;
}