using IoliteLauncher.Backend.Core;

namespace IoLiteLauncher.Backend;

public class Instance {

    private static Instance? _instance;

    public readonly ProjectsManager ProjectsManager;
    public readonly SettingsManager SettingsManager;
    public readonly LockManager LockManager;

    private Instance() {
        _instance ??= this;

        SettingsManager = new SettingsManager();
        LockManager = new LockManager(this);
        ProjectsManager = new ProjectsManager();
    }

    public static Instance Get {
        get {
            if (_instance == null) _instance = new Instance();
            Instance nonNullableInstance = _instance;
            return nonNullableInstance;
        }
    }

    public void Init() {
        SettingsManager.Load();
    }

    public void Shutdown() {
        _instance?.ProjectsManager.CloseProject();
        SettingsManager.Save();
    }
}