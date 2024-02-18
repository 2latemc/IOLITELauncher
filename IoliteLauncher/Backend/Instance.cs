using System.Collections.Generic;
using System.Windows.Documents.DocumentStructures;

namespace IoLiteLauncher.Backend;

public class Instance {

    private static Instance? _instance;

    public SettingsManager SettingsManager;
    public LockManager LockManager;

    private Instance() {
        SettingsManager = new SettingsManager();
        LockManager = new LockManager(this);
    }

    public static Instance Get {
        get {
            if(_instance == null) _instance = new Instance();
            return _instance;
        }
    }

    public void Init() {
        SettingsManager.Load();
    }

    public void Shutdown() {
        SettingsManager.Save();
    }
}