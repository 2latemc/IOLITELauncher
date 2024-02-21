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
            _instance ??= Instance.Get;
            return Path.Combine(_instance.SettingsManager.SettingsData.EnginePath, ".lock");
        }
    }

    public bool ForceRemoveLockFile() {
        try {
            if (File.Exists(LockPath))
                File.Delete(LockPath);
            return true;
        }
        catch(Exception e) {
            MessageBox.Show("Could not delete lock file with e " + e.Message);
            return false;
        }
    }

    public bool ContainsLock() {
        _instance ??= Instance.Get;

        return !String.IsNullOrEmpty(GetLockProject());
    }

    public bool CreateLock(LockFile lockFile) {
        if (ContainsLock()) {
            MessageBox.Show("Could not create .lock file: Project already contains a lock file");
            return false;
        }

        try {
            var json = JsonConvert.SerializeObject(lockFile, Formatting.Indented);
            File.WriteAllText(LockPath, json);
            return true;
        }
        catch (Exception e) {
            MessageBox.Show("Could not write .lock file " + e.Message);
            return false;
        }
    }

    private string? GetLockProject() {
        if (!File.Exists(LockPath)) {
            return null;
        }

        try {
            string lockText = File.ReadAllText(LockPath);
            return JsonConvert.DeserializeObject<LockFile>(lockText).ProjectPath;
        }
        catch (Exception e) {
            MessageBox.Show("Could not check for .lock file with e: " + e.Message);
            return null;
        }
    }
}

public struct LockFile {
    public string ProjectPath;

    public LockFile(string projectPath) {
        ProjectPath = projectPath;
    }
}