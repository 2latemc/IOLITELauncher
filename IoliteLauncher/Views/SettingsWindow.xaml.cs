﻿using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using IoLiteLauncher.Backend;

namespace IoliteLauncher.Views;

public partial class SettingsWindow : Window {
    private ObservableCollection<string> _projectPathEntries = null!;
    public ObservableCollection<string> ProjectPathEntries {
        get => _projectPathEntries;
        set => _projectPathEntries = value;
    }

    private Instance _instance;
    public SettingsWindow(SettingsData? overrideData = null) {
        DataContext = this;
        _instance = Instance.Get;

        ProjectPathEntries = new ObservableCollection<string>();


        InitializeComponent();

        ProjectPathEntries.Add( Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "IoliteProjects"));

        if (overrideData != null) {
            ProjectPathEntries.Clear();
            foreach (string path in overrideData.ProjectsPaths) {
                ProjectPathEntries.Add(path);
            }

            EngineBox.Text = overrideData.EnginePath;
        }
    }

    private void AddProjectPathElementButton(object sender, RoutedEventArgs e) {
        ProjectPathEntries.Add(ProjectPathBox.Text);
    }

    private void RemoveElementButton(object sender, RoutedEventArgs e) {
        if (ProjectPathList.SelectedItem == null) {
            MessageBox.Show("You need to select an element first.");
            return;
        }
        ProjectPathEntries.Remove((string)ProjectPathList.SelectedItem);
    }

    private void Submit(object sender, RoutedEventArgs e) {
        if (ProjectPathEntries.Count <= 0) {
            MessageBox.Show(
                "You need to add a project path to submit (you might have forgotten to press the add button after typing the path");
            return;
        }

        if (!_instance.SettingsManager.IsValidProjectsPath(ProjectPathEntries.ToList())) {
            MessageBox.Show("Please enter valid project paths!");
            return;
        }

        if (!_instance.SettingsManager.IsValidEnginePath(EngineBox.Text)) {
            MessageBox.Show("Enter valid engine path");
            return;
        }

        _instance.SettingsManager.SettingsData = new SettingsData() {
            ProjectsPaths = ProjectPathEntries.ToList(),
            EnginePath = EngineBox.Text,
        };
        _instance.SettingsManager.Save();

        MainWindow mainWindow = new MainWindow();
        mainWindow.Show();
        Close();
    }
}