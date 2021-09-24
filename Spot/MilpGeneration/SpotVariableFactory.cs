using NodaTime;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.PassengerOdRelations;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.Scenario;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.Trains;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Services;
using SMA.Algorithms.Utils.LPCreation.Model;
using SMA.Apps.Utils.Extensions;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.MilpGeneration {
    public class SpotVariableFactory {
        private const string Departure = "DEP";
        private const string Arrival = "ARR";
        private const string CycleShift = "CYC";
        private const string AdditionalRunningTime = "ADD";
        private const string RoutePartDuration = "RPD";
        private const string PassengerRouteDuration = "TRD";
        private const string SelectedRouteDuration = "SRD";
        private const string RouteSelection = "RSD";
        private const string RoutePartTransferDuration = "PTD";
        private readonly TimeConverter _timeConverter;
        private readonly Duration _cycleDuration;
        private readonly Duration _maximumTransferTime;

        public SpotVariableFactory(TimeConverter timeConverter, Duration cycleDuration, Duration maximumTransferTime) {
            _timeConverter = timeConverter;
            _cycleDuration = cycleDuration;
            _maximumTransferTime = maximumTransferTime;
        }

        public ContinuousVariable CreateDeparture(ISpotLineConstraint lineConstraint, ISpotPathNodeConstraint lpn) {
            var name = CreateName(Departure, lineConstraint.Code, lineConstraint.ID.ToInvariantString(), lpn.ID.ToInvariantString(), lpn.SequenceNumber.ToInvariantString());
            return new ContinuousVariable(name, 0, _timeConverter.ToModelTime(_cycleDuration - _timeConverter.TimeStep));
        }

        public ContinuousVariable CreateArrival(ISpotLineConstraint lineConstraint, ISpotPathNodeConstraint lpn) {
            var name = CreateName(Arrival, lineConstraint.Code, lineConstraint.ID.ToInvariantString(), lpn.ID.ToInvariantString(), lpn.SequenceNumber.ToInvariantString());
            return new ContinuousVariable(name, 0, _timeConverter.ToModelTime(_cycleDuration - _timeConverter.TimeStep));
        }

        public BinaryVariable CreateCycleTimeShift(ISpotPathNodeConstraint predecessorEventReferenceNodeConstraint, ISpotPathNodeConstraint successorEventReferenceNodeConstraint) {
            var name = CreateName(
                CycleShift,
                predecessorEventReferenceNodeConstraint.ID.ToInvariantString(),
                predecessorEventReferenceNodeConstraint.SequenceNumber.ToInvariantString(),
                successorEventReferenceNodeConstraint.ID.ToInvariantString(),
                successorEventReferenceNodeConstraint.SequenceNumber.ToInvariantString());
            return new BinaryVariable(name);
        }

        public ContinuousVariable CreateAdditionalRunningTime(ISpotLineConstraint lineConstraint, ISpotPathNodeConstraint predecessorNode) {
            var name = CreateName(
                AdditionalRunningTime,
                lineConstraint.ID.ToInvariantString(),
                predecessorNode.ID.ToInvariantString(),
                predecessorNode.SequenceNumber.ToInvariantString());
            return new ContinuousVariable(name);
        }

        public ContinuousVariable CreateRoutePartDuration(IPassengerTravelRoutePart part) {
            var name = CreateName(
                RoutePartDuration,
                part.SpotLineConstraint.Code,
                part.SpotLineConstraint.ID.ToInvariantString(),
                part.StartLinePathNodeConstraint.ID.ToInvariantString(),
                part.EndLinePathNodeConstraint.ID.ToInvariantString());

            var minimumTravelTimeOnRoutPartAsModelTime = _timeConverter.ToModelTime(TravelTimesCalculationServices.CalculateMinimumTravelTimeOnRoutePart(part));
            var upperBound = minimumTravelTimeOnRoutPartAsModelTime
                             + _timeConverter.ToModelTime(TravelTimesCalculationServices.CalculateMaximumNumberCycles(part) * _cycleDuration);
            return new ContinuousVariable(name, minimumTravelTimeOnRoutPartAsModelTime, upperBound);
        }

        public ContinuousVariable CreateRoutePartTransferDuration(ISpotLineConstraint arrivalLine, ISpotPathNodeConstraint arrivalNodeConstraint, ISpotLineConstraint departureLine, ISpotPathNodeConstraint departureNodeConstraint) {
            var name = CreateName(
                RoutePartTransferDuration,
                arrivalLine.Code,
                arrivalLine.ID.ToInvariantString(),
                arrivalNodeConstraint.ID.ToInvariantString(),
                departureLine.Code,
                departureLine.ID.ToInvariantString(),
                departureNodeConstraint.ID.ToInvariantString());
            return new ContinuousVariable(name, 0, _timeConverter.ToModelTime(_maximumTransferTime));
        }

        public ContinuousVariable CreateRouteTotalDuration(IPassengerTravelRoute travelRoute) {
            var name = CreateName(PassengerRouteDuration, travelRoute.ID.ToInvariantString());
            var lowerBound = _timeConverter.ToModelTime(TravelTimesCalculationServices.CalculateMinimumTravelTimeOnTravelRoute(travelRoute));
            var upperBound = _timeConverter.ToModelTime(TravelTimesCalculationServices.CalculateMaximumTravelTimeOnTravelRoute(travelRoute, _maximumTransferTime, _cycleDuration));
            return new ContinuousVariable(name, lowerBound, upperBound);
        }

        public ContinuousVariable CreateSelectedRouteTotalTravelDuration(IPassengerRelation relation) {
            var variableName = CreateName(SelectedRouteDuration, relation.OriginNode.ID.ToInvariantString(), relation.DestinationNode.ID.ToInvariantString());
            var lowerBound = _timeConverter.ToModelTime(TravelTimesCalculationServices.CalculateMinimumTravelTimeOnAnyRoute(relation));
            var upperBound = _timeConverter.ToModelTime(TravelTimesCalculationServices.CalculateMaximumTravelTimeOnAnyRoute(relation, _maximumTransferTime, _cycleDuration));
            return new ContinuousVariable(variableName, lowerBound, upperBound);
        }

        public BinaryVariable CreateRouteSelectionVariable(IPassengerTravelRoute travelRoute) {
            var name = CreateName(RouteSelection, travelRoute.ID.ToInvariantString());
            return new BinaryVariable(name);
        }

        private static string CreateName(string baseName, params string[] parts) {
            return string.Join("_", baseName, string.Join("_", parts));
        }
    }
}