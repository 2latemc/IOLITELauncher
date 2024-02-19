﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Documents;
using IoLiteLauncher.Backend;
using IoLiteLauncher.Utils;

namespace IoliteLauncher.Backend.Core;

public class ProjectsManager {
    private Instance _instance;
    private SettingsManager _settingsManager;

    public ProjectsManager() {
        _instance = Instance.Get;
        _settingsManager = _instance.SettingsManager;
        _lockManager = _instance.LockManager;
    }

    public void CreateProject() {

    }

    public void DeleteProject() {

    }

    public void RenameProject() {

    }

    public List<ProjectData> FetchProjects() {
        return new List<ProjectData>();
    }

    public struct ProjectData {
        public string Path;
        public string ProjectName;
    }

    private LockManager _lockManager;
    public void OpenProject(string path) {
        if (_lockManager.ContainsLock()) {
            var result =
                MessageBox.Show("Looks like another Project is currently open. Do you want to try force closing it?",
                    "", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes) {
                CloseProject();
            } else {
                return;
            }
        }


        Debug.WriteLine("Starting Engine..");
        Process.Start(_settingsManager.ExecutablePath);
    }

    private void CloseProject() {

    }


    public void MoveTemplateProjects() {
        for (int i = 0; i < Statics.TemplatePaths.Count; i++) {
            Statics.TemplatePaths[i] = Statics.TemplatePaths[i].ToLower();
        }

        bool allowed = false;
        foreach (var dir in Directory.GetDirectories(_settingsManager.SettingsData.EnginePath)) {
            DirectoryInfo dirInfo = new DirectoryInfo(dir);
            var dirName = dirInfo.Name;

            if (Statics.TemplatePaths.Contains(dirName.ToLower())) {
                try {

                    if (!allowed) {
                        var result = MessageBox.Show(
                            "In order to continue we need to move the Template projects. Would you like to continue?",
                            "", MessageBoxButton.YesNo);
                        if (result == MessageBoxResult.No) {
                            Application.Current.Shutdown();
                            return;
                        }
                        allowed = true;

                    }

                    string destiPath = Path.Combine(_settingsManager.SettingsData.ProjectsPaths[0], "TemplateProjects");
                    if (!Directory.Exists(destiPath)) Directory.CreateDirectory(destiPath);
                    if (allowed) {
                        foreach (var file in Directory.GetFiles(_settingsManager.SettingsData.EnginePath)) {
                            if (Path.GetFileName(file).ToLower() == Statics.MetadataFileName.ToLower()) {
                                File.Move(file, Path.Combine(destiPath, Path.GetFileName(file)));
                            }
                        }
                    }
                    Directory.Move(dir, Path.Combine(destiPath,dirName));
                }
                catch (Exception e) {
                    MessageBox.Show("Could not project. " + e.Message);
                }
            }
        }
    }

}