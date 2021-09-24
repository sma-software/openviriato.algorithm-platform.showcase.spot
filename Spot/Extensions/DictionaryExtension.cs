using System.Collections.Generic;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Extensions {
    public static class DictionaryExtension {
        public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, List<TValue>> dictionary, TKey key, TValue value) {
            if (!dictionary.ContainsKey(key)) {
                dictionary[key] = new List<TValue>();
            }

            dictionary[key].Add(value);
        }
    }
}