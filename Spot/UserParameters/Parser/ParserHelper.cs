using System.Linq;
using SMA.Apps.Utils.Collections.Generic;
using SMA.Apps.Utils.Collections.Generic.Extensions;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.UserParameters.Parser {
    public class ParserHelper {
        private IImmutableList<string> _state;

        public ParserHelper(IImmutableList<string> splitToParse) {
            _state = splitToParse;
        }

        public string Next() {
            var next = _state.First();
            _state = _state.Skip(1).ToImmutableList();
            return next;
        }

        public ParserHelper Skip(int splitsToSkip) {
            _state = _state.Skip(splitsToSkip).ToImmutableList();
            return this;
        }

        public bool Any() {
            return _state.Any();
        }

        public IImmutableList<string> Take() {
            return _state;
        }

        public IImmutableList<string> Take(int splitsToTake) {
            var take = _state.Take(splitsToTake).ToImmutableList();
            _state = _state.Skip(splitsToTake).ToImmutableList();
            return take;
        }
    }
}