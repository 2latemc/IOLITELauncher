using System.Collections.Generic;
using System.Windows.Documents.DocumentStructures;

namespace IoLiteLauncher.Backend;

public class Instance {

    private static Instance _instance;

    public static Instance Get {
        get {
            if(_instance == null) _instance = new Instance();
            return _instance;
        }
    }

    public static string enginePath;
    public static List<string> projectsPaths;

    public SettingsManager _settingsManager = new SettingsManager();

    public void Init() {
        _settingsManager.Save();
    }
}