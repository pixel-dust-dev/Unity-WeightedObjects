using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WeightedObjects
{
    public static class RandomExtensions
    {
        public static int GetRandomWeightedIndex(float[] weights, int seed = 0)
        {
            if (weights == null || weights.Length == 0) return -1;

            float w;
            float t = 0;
            int i;
            for (i = 0; i < weights.Length; i++)
            {
                w = weights[i];

                if (float.IsPositiveInfinity(w))
                {
                    return i;
                }
                else if (w >= 0f && !float.IsNaN(w))
                {
                    t += weights[i];
                }
            }

            if(seed != 0)
            {
                UnityEngine.Random.InitState(seed);
            }

            float r = Random.value;
            float s = 0f;

            for (i = 0; i < weights.Length; i++)
            {
                w = weights[i];
                if (float.IsNaN(w) || w <= 0f) continue;

                s += w / t;
                if (s >= r) return i;
            }

            return -1;
        }

        public static WeightedObject<T> GetRandomWeightedIndex<T>(List<WeightedObject<T>> weightedObject, int seed = 0)
        {
            var weights = weightedObject.Select(x => x.Weight).ToArray();

            var index = GetRandomWeightedIndex(weights, seed);

            if(index == -1)
            {
                return null;
            }

            return weightedObject[index];
        }
    }
}
