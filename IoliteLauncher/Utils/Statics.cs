using System.Collections.Generic;

namespace IoLiteLauncher.Utils;

public class Statics {
    public static readonly string ExecutableName = "Iolite.exe";
    public static readonly string DownloadUrl = "https://iolite-engine.com/api/download_windows";
    public static readonly string MetadataFileName = "app_metadata.json";
    public static readonly string ProjectDataStructureName = "projectDataStructure.json";
    public static readonly string WarningFileName = "WARNING_README.txt";

    public static List<string> TemplatePaths = new List<string>() {
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

    public static List<string> AffectedFileNames = new List<string>() {
        MetadataFileName,
        ProjectDataStructureName,
        "plugins.json"
    };
}