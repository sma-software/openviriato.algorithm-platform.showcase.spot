using System.Globalization;
using SMA.Apps.Utils.Extensions;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.MilpGeneration {
    public static class SpotConstraintNameFactory {
        private const string RunningTimeEquation = "C01_RUN";
        private const string StoppingTimeEquationWithStop = "C02a_STOP";
        private const string StoppingTimeEquationWithoutStop = "C02b_STOP";
        private const string TravelRoutePartDurationEquation = "C03_TRPD";
        private const string MinimumTravelRouteDuration = "CO4_MIND";
        private const string MaximumTravelRouteDuration = "CO5_MAXD";
        private const string MinimumDurationSelectionTravelRoute = "C06_TRDS";
        private const string TravelRouteTotalDuration = "C07_TRD";
        private const string RequiredMinimumTransferDuration = "C08_RPTM";
        private const string TransferDurationBetweenSucceedingTravelParts = "C09_RPTD";
        private const string AdditionalRunningTimeBoundEquation = "C10_ADDR";

        private const char SeparatorChar = '_';

        public static string CreateRunningTimeEquation(int index) {
            return GetName(RunningTimeEquation, index);
        }

        public static string CreateAdditionalRunningTimeBoundEquation(int index) {
            return GetName(AdditionalRunningTimeBoundEquation, index);
        }

        public static string CreateStoppingTimeEquationWithStop(int index) {
            return GetName(StoppingTimeEquationWithStop, index);
        }

        public static string CreateStoppingTimeEquationWithoutStop(int index) {
            return GetName(StoppingTimeEquationWithoutStop, index);
        }

        public static string CreateTravelRoutePartDuration(int index) {
            return GetName(TravelRoutePartDurationEquation, index);
        }

        public static string CreateMinimumTravelRouteDuration(int index) {
            return GetName(MinimumTravelRouteDuration, index);
        }

        public static string CreateMaximumTravelRouteDuration(int index) {
            return GetName(MaximumTravelRouteDuration, index);
        }

        public static string CreateTravelRouteDurationSelectionEquation(int index) {
            return GetName(MinimumDurationSelectionTravelRoute, index);
        }

        public static string CreateRequiredTransferDurationBetweenSucceedingRouteParts(int index) {
            return GetName(RequiredMinimumTransferDuration, index);
        }

        public static string CreateTransferDurationBetweenSucceedingTravelRouteParts(int index) {
            return GetName(TransferDurationBetweenSucceedingTravelParts, index);
        }

        public static string CreateTravelRouteTotalDurationEquation(int index) {
            return GetName(TravelRouteTotalDuration, index);
        }

        private static string GetName(string name, int index) {
            return string.Join(SeparatorChar.ToString(CultureInfo.InvariantCulture), name, index.ToInvariantString());
        }
    }
}