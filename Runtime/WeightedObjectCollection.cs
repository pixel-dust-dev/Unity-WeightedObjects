﻿using System.Collections;
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

        public T GetRandom()
        {
            WeightedObject<T> weightedSelection = null;

            if(lastCount != Length)
            {
                lastCount = Length;
                validWeightedObjects = weightedObjects.ToList();
                RebuildRandomPool();
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
                weightedSelection = RandomExtensions.GetRandomWeightedIndex<T>(validWeightedObjects);
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
                var selIndex = -1;

                //First generation
                if(randomPool.Count == 0)
                {
                    RebuildRandomPool();
                }

                //If there is a final element in the pool
                if (randomPool.Count == 1)
                {
                    selIndex = randomPool[0];
                    RebuildRandomPool();
                }
                else
                {
                    int ranIndex = UnityEngine.Random.Range(0, randomPool.Count);
                    selIndex = randomPool[ranIndex];
                    
                    if (selIndex >= weightedObjects.Count)
                    {
                        RebuildRandomPool();
                        selIndex = randomPool[0];
                    }
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

            void RebuildRandomPool()
            {
                randomPool = new List<int>();
                for (int i = 0; i < weightedObjects.Count; i++)
                {
                    int number = (int)weightedObjects[i].Weight;
                    if(number < 1)
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
}
