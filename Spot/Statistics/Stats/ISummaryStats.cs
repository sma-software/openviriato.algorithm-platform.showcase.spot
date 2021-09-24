using NodaTime;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Statistics.Stats {
    public interface ISummaryStats {
        Duration TotalTravelTime { get; }
        int PassengersWithoutARoute { get; }
    }

    public class SummaryStats : ISummaryStats {
        public SummaryStats(Duration totalTravelTime, int passengersWithoutARoute) {
            TotalTravelTime = totalTravelTime;
            PassengersWithoutARoute = passengersWithoutARoute;
        }

        public Duration TotalTravelTime { get; }
        public int PassengersWithoutARoute { get; }
    }
}