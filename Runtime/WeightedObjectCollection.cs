﻿using System.Collections.Generic;
using UnityEngine;

namespace WeightedObjects
{
    public enum RandomType { WeightedRandom, RandomExhaustive, Ordered }

    [System.Serializable]
    public class WeightedObjectCollection<T>
    {
        [SerializeField]
        WeightedObject<T>[] weightedObjects = new WeightedObject<T>[] { };

        public RandomType randomType = RandomType.WeightedRandom;

        public int Length => weightedObjects.Length;

        //Runtime Use
        // Order
        int currIndex = 0;
        // Random
        List<int> randomPool = new List<int>();
        //

        public T GetRandom()
        {
            if(weightedObjects.Length == 0)
            {
                Debug.LogWarning("No Objects in list.");
                return default(T);
            }

            if(randomType == RandomType.WeightedRandom)
            {
                return RandomExtensions.GetRandomWeightedIndex<T>(weightedObjects);
            }
            else if(randomType == RandomType.Ordered)
            {
                if (currIndex >= weightedObjects.Length)
                {
                    currIndex = 0;
                }
                T returnedObject = weightedObjects[currIndex].Contents;
                currIndex++;
                return returnedObject;
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
                    Debug.Log(selIndex);
                    RebuildRandomPool();
                }
                else
                {
                    int ranIndex = UnityEngine.Random.Range(0, randomPool.Count);
                    selIndex = randomPool[ranIndex];
                    
                    if (selIndex >= weightedObjects.Length)
                    {
                        RebuildRandomPool();
                        selIndex = randomPool[0];
                    }
                    Debug.Log(selIndex);
                }

                randomPool.Remove(selIndex);

                var selectedEntry = weightedObjects[selIndex];

                return selectedEntry.Contents;
            }
            else
            {
                return default(T);
            }

            void RebuildRandomPool()
            {
                randomPool = new List<int>();
                for (int i = 0; i < weightedObjects.Length; i++)
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
                Debug.Log("Reset");
            }
        }
    }
}
