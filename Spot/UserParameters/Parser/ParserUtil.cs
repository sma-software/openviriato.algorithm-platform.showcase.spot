using System.Globalization;
using System.IO;
using SMA.Apps.Utils.Answers;
using SMA.Apps.Utils.Collections.Generic;
using SMA.Apps.Utils.Collections.Generic.Extensions;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.UserParameters.Parser {
    public static class ParserUtil {
        public static Result<IImmutableList<string>> ReadAllLineSafely(string fileName, string settingName) {
            try {
                if (fileName == null) {
                    return Result.FromString(string.Format(CultureInfo.InvariantCulture, "No file name given for {0}.", settingName));
                }

                var fileInfo = new FileInfo(fileName);
                if (!fileInfo.Exists) {
                    return Result<IImmutableList<string>>.FromString(string.Format(CultureInfo.InvariantCulture, "File given for {0} does not exist. Given file name: {1}. Is this the right file name?", settingName, fileName));
                }

                return File.ReadAllLines(fileName).ToImmutableList();
            } catch (IOException e) {
                return Result<IImmutableList<string>>.FromString(string.Format(CultureInfo.InvariantCulture, "Reading of file {0} for {1} failed. See log file for more details. Message: {2}", fileName, settingName, e.Message));
            }
        }
    }
}