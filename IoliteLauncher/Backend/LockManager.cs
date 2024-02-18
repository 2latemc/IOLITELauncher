using System;
using System.IO;
using System.Windows;
using Newtonsoft.Json;

namespace IoLiteLauncher.Backend;

public class LockManager {
    private Instance? _instance;

    public LockManager(Instance instance) {
        _instance = instance;
    }

    private string LockPath {
        get {
            if (_instance == null) _instance = Instance.Get;
            return Path.Combine(_instance.SettingsManager.SettingsData.EnginePath, ".lock");
        }
    }

    public bool ContainsLock() {
        if (_instance == null) _instance = Instance.Get;

        return !String.IsNullOrEmpty(GetLockProject());
    }

    public bool CreateLock(string projectPath) {
        if (ContainsLock()) {
            MessageBox.Show("Could not create .lock file: Project already contains a lock file");
            return false;
        }

        try {
            LogFile logFile = new LogFile(projectPath);
            var json = JsonConvert.SerializeObject(logFile);
            File.WriteAllText(projectPath, json);
            return true;
        }
        catch (Exception e) {
            MessageBox.Show("Could not write .lock file " + e.Message);
            return false;
        }
    }

    public string? GetLockProject() {
        if (!File.Exists(LockPath)) {
            return null;
        }

        try {
            string lockText = File.ReadAllText(LockPath);
            return JsonConvert.DeserializeObject<LogFile>(LockPath).ProjectPath;
        }
        catch (Exception e) {
            MessageBox.Show("Could not check for .lock file with e: " + e.Message);
            return null;
        }
    }
}

public struct LogFile {
    public string ProjectPath;

    public LogFile(string projectPath) {
        ProjectPath = projectPath;
    }
}