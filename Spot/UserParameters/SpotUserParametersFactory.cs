using System.Globalization;
using NodaTime;
using SMA.AlgorithmPlatform.AlgorithmInterface;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Extensions;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.UserParameters {
    public class SpotUserParametersFactory {
        private readonly IAlgorithmInterface _algorithmInterface;

        public SpotUserParametersFactory(IAlgorithmInterface algorithmInterface) {
            _algorithmInterface = algorithmInterface;
        }

        public ISpotUserParameters CreateParameters() {
            var filePathToRoutesAsCsvFile = _algorithmInterface.GetStringAlgorithmParameter("filePathToRoutesAsCsv");
            var maximalNumberOfTransfers = _algorithmInterface.GetIntAlgorithmParameter("maximalNumberOfTransfers").Value;
            var defaultMinimumTransferTimeInTenthOfMinutes = _algorithmInterface.GetFloatingPointAlgorithmParameter("defaultMinimumTransferTime");
            var defaultMinimumTransferTime = Duration.FromSeconds(defaultMinimumTransferTimeInTenthOfMinutes.Mantissa * 6);
            var cycleTimeWindow = _algorithmInterface.GetTimeWindowAlgorithmParameter("cycleTimeWindow").ToTimeWindow();
            var numberOfCycles = _algorithmInterface.GetIntAlgorithmParameter("numberOfCycles").Value;
            var solverTimeout = _algorithmInterface.GetIntAlgorithmParameter("solverTimeout");
            var additionalRunningTimeInPercent = _algorithmInterface.GetIntAlgorithmParameter("maximalAdditionalRunningTime").Value;
            var additionalRunTimeFactor = ((double)additionalRunningTimeInPercent / 100);
            var algorithmTrains = _algorithmInterface.GetAlgorithmTrainsParameter("templateTrainsFromScenario");
            _algorithmInterface.NotifyUser("Spot", string.Format(CultureInfo.InvariantCulture, "Instance contains {0} template trains", algorithmTrains.Count));

            return new SpotUserParameters(solverTimeout, maximalNumberOfTransfers, defaultMinimumTransferTime, numberOfCycles, filePathToRoutesAsCsvFile, cycleTimeWindow, additionalRunTimeFactor, algorithmTrains);
        }
    }
}