using System;
using NodaTime;
using SMA.Algorithms.Utils.TimeWindow;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.Scenario {
    public class TimeConverter {
        public TimeConverter(LocalDateTime startTime, Duration cycleTime) {
            CycleTime = cycleTime;
            StartTime = startTime;
            TimeStep = Duration.FromSeconds(6);
        }

        public Duration TimeStep { get; }

        public LocalDateTime StartTime { get; }

        public Duration CycleTime { get; }
        public TimeWindow TimeWindowFirstCycleTime => new TimeWindow(StartTime, StartTime.Plus(CycleTime));

        public LocalDateTime FromModelTime(double modelTime) {
            return StartTime.Plus(ToDuration(modelTime));
        }

        public int ToModelTime(Duration duration) {
            return (int)(duration.ToTimeSpan().TotalSeconds / TimeStep.ToTimeSpan().TotalSeconds);
        }

        public Duration ToDuration(double modelTime) {
            return Duration.FromSeconds((long)Math.Round(TimeStep.ToTimeSpan().TotalSeconds * modelTime));
        }
    }
}