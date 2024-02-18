using System.Diagnostics;
using IoLiteLauncher.Backend;

namespace IoliteLauncher.Backend.Core;

public class ProjectsManager {
    private Instance _instance;
    private SettingsManager _settingsManager;

    public ProjectsManager() {
        _instance = Instance.Get;
        _settingsManager = _instance.SettingsManager;
    }

    public void CreateProject() {

    }

    public void DeleteProject() {

    }

    public void RenameProject() {

    }

    public void OpenProject(string path) {
        Process.Start(_settingsManager.ExecutablePath);
    }
}