using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WeightedObjects
{
    public enum RandomType { WeightedRandom, RandomExhaustive, Ordered }

    [System.Serializable]
    public class WeightedObjectCollection<T>
    {
        [SerializeField]
        List<WeightedObject<T>> weightedObjects = new List<WeightedObject<T>> { };
        List<WeightedObject<T>> validWeightedObjects = new List<WeightedObject<T>>();

        public RandomType randomType = RandomType.WeightedRandom;
        public bool canRepeat = true;

        private int lastCount = -1;
        public int Length => weightedObjects.Count;

        //Runtime Use
        // Order
        int currIndex = 0;
        // Random
        List<int> randomPool = new List<int>();
        //

        public void Add(T newObj, int weight)
        {
            weightedObjects.Add(new WeightedObject<T>(newObj, weight));
        }

        int lastSeed = 0;
        public T GetRandom(int seed = 0)
        {
            WeightedObject<T> weightedSelection = null;

            if(lastCount != Length || lastSeed != seed)
            {
                lastSeed = seed;
                lastCount = Length;
                ResetState();
            }

            if (weightedObjects.Count == 0)
            {
                Debug.LogWarning("No Objects in list.");
                return default(T);
            }

            if(randomType == RandomType.WeightedRandom)
            {
                if(validWeightedObjects.Count == 0)
                {
                    validWeightedObjects = weightedObjects.ToList();
                }
                weightedSelection = RandomExtensions.GetRandomWeightedIndex<T>(validWeightedObjects, seed);
                if(!canRepeat)
                {
                    validWeightedObjects.Remove(weightedSelection);
                }
            }
            else if(randomType == RandomType.Ordered)
            {
                if (currIndex >= weightedObjects.Count)
                {
                    currIndex = 0;
                }
                weightedSelection = weightedObjects[currIndex];
                currIndex++;
            }
            else if(randomType == RandomType.RandomExhaustive)
            {
                var selIndex = 0;

                //First generation
                if(randomPool.Count == 0)
                {
                    ResetState();
                }

                //If there is a final element in the pool
                if(seed > 0)
                {
                    UnityEngine.Random.InitState(seed);
                }

                int ranIndex = UnityEngine.Random.Range(0, randomPool.Count);
                selIndex = randomPool[ranIndex];

                //In case we modified the collection somehow
                if (selIndex >= weightedObjects.Count)
                {
                    ResetState();
                    selIndex = randomPool[0];
                }

                randomPool.Remove(selIndex);

                weightedSelection = weightedObjects[selIndex];
            }

            if(weightedSelection != null)
            {
#if UNITY_EDITOR
                weightedSelection.Ping();
#endif
                return weightedSelection.Contents;
            }
            else
            {
                return default(T);
            }
        }

        public void ResetState()
        {
            validWeightedObjects = weightedObjects.ToList();
            RebuildRandomPool();
        }

        void RebuildRandomPool()
        {
            randomPool = new List<int>();
            for (int i = 0; i < weightedObjects.Count; i++)
            {
                int number = (int)weightedObjects[i].Weight;
                if (number < 1)
                {
                    number = 1;
                }
                for (int j = 0; j < number; j++)
                {
                    randomPool.Add(i);
                }
            }
        }
    }
}
