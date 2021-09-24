using System.Linq;
using NodaTime;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.PassengerOdRelations;
using SMA.Algorithms.Utils.TimeWindow;
using SMA.Apps.Utils.Collections.Generic.Extensions;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Services {
    public static class TravelTimesCalculationServices {
        public static Duration CalculateMinimumTravelTimeOnRoutePart(IPassengerTravelRoutePart part) {
            var totalMinimumRunTime = part
                .VisitedNodes
                .Skip(1)
                .Select(n => n.MinRunTime.Value)
                .Sum();

            if (part.VisitedNodes.Count == 2) {
                return totalMinimumRunTime;
            } else {
                var totalMinimumStopTime = part
                    .VisitedNodes
                    .SkipLast(1)
                    .Skip(1)
                    .Select(n => n.MinStopTime)
                    .Sum();
                return totalMinimumRunTime + totalMinimumStopTime;
            }
        }

        public static Duration CalculateMinimumTravelTimeOnTravelRoute(IPassengerTravelRoute travelRoute) {
            return travelRoute.TravelRouteParts.Select(CalculateMinimumTravelTimeOnRoutePart).Sum();
        }

        public static Duration CalculateMinimumTravelTimeOnAnyRoute(IPassengerRelation relation) {
            var minimumTravelTimesOnAnyRoute = relation
                .TravelRoutes
                .Select(TravelTimesCalculationServices.CalculateMinimumTravelTimeOnTravelRoute)
                .Min();
            return minimumTravelTimesOnAnyRoute;
        }

        public static int CalculateMaximumNumberCycles(IPassengerTravelRoutePart part) {
            var numberOfStops = part.VisitedNodes.Count() - 2;
            var numberOfRuns = part.VisitedNodes.Count - 1;
            return numberOfRuns + numberOfStops;
        }

        public static Duration CalculateMaximumTravelTimeOnTravelRoute(IPassengerTravelRoute travelRoute, Duration maximalTransferDuration, Duration cycleDuration) {
            var totalMaximalTransfersDuration = travelRoute.NumberTransfers * maximalTransferDuration;
            var totalMaximalTravelRoutePartsDuration = travelRoute.TravelRouteParts.Select(CalculateMinimumTravelTimeOnRoutePart).Sum() + cycleDuration.Times(travelRoute.TravelRouteParts.Select(CalculateMaximumNumberCycles).Sum());
            return totalMaximalTransfersDuration + totalMaximalTravelRoutePartsDuration;
        }

        public static Duration CalculateMaximumTravelTimeOnAnyRoute(IPassengerRelation relation, Duration maximumTransferTime, Duration cycleDuration) {
            var maximumTravelTimeOnAnyRoute = relation
                .TravelRoutes
                .Select(route => TravelTimesCalculationServices.CalculateMaximumTravelTimeOnTravelRoute(route, maximumTransferTime, cycleDuration))
                .Max();
            return maximumTravelTimeOnAnyRoute;
        }
    }
}