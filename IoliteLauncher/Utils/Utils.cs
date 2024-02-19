using System.IO;

namespace IoLiteLauncher.Utils;

public class Utils {
    public static bool IsMetaDataFile(string file) {
        return Path.GetFileName(file).ToLower().Equals(Statics.MetadataFileName);
    }
}