using System;
using System.Collections.Generic;
using System.IO;

namespace IoLiteLauncher.Utils;

public abstract class Statics {
    public static readonly string ExecutableName = "Iolite.exe";
    public static readonly string DownloadUrl = "https://media.missing-deadlines.com/iolite/builds/release/iolite-v0.4.8.exe";
    public static readonly string MetadataFileName = "app_metadata.json";
    public static readonly string ProjectDataStructureName = "projectDataStructure.json";
    public static readonly string WarningFileName = "WARNING_README.txt";
    public static readonly string GithubIssuesUrl = "https://github.com/2latemc/ioliteLauncher/issues";
    public static readonly string DefaultProjectTemplateFolderName = "DefaultProject";

    public static string AppDataPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "IoliteLauncher");

    public static readonly List<string> TemplatePaths = new List<string>() {
        "default",
        "sample_base",
        "sample_flappy",
        "sample_heightmap",
        "sample_minecraft",
        "sample_minecraft_map",
        "sample_pathfinding",
        "sample_physics",
        "sample_spectrum",
        "trapped_below",
    };

    public static readonly List<string> AffectedFileNames = new List<string>() {
        MetadataFileName,
        ProjectDataStructureName,
        "plugins.json"
    };
}