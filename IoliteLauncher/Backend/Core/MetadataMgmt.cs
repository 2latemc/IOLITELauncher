using System.Collections.Generic;
using System.IO;
using IoLiteLauncher.Backend;
using IoLiteLauncher.Utils;

namespace IoliteLauncher.Backend.Core;

public class MetadataMgmt {


    public class Project {
        public string application_name = "Project";
        public string organization_name = "Organization";
        public List<string> data_sources = new List<string>();
        public string initial_world = "default";
        public string active_data_source = "deprecated"; // This is deprecated and not needed but still needed for loading.
        public string initial_game_state = "Editing";
        public string version_string = "development";
    }

    public static Project? GetProjectMetaDataAtPath(string metadataFilePath) {
        return Serializer.FromFile<MetadataMgmt.Project>(metadataFilePath);
    }

    public static bool IsMetaDataFile(string file) {
        return Path.GetFileName(file).ToLower().Equals(Statics.MetadataFileName);
    }

}