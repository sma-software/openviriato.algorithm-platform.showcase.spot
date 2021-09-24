using SMA.AlgorithmPlatform.AlgorithmInterface.AIDM;
using SMA.Algorithms.Utils.TimeWindow;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Extensions {
    public static class TimeWindowExtensions {
        public static TimeWindow ToTimeWindow(this ITimeWindow timeWindow) {
            return new TimeWindow(timeWindow.FromTime, timeWindow.ToTime);
        }
    }
}