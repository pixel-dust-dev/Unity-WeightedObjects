using System;
using UnityEngine;

namespace WeightedObjects
{
    [System.Serializable]
    public class WeightedObject
    {
        [SerializeField]
        float weight = 1;
        public float Weight => weight;

#if UNITY_EDITOR
        //Used to display a little flash of color in editor
        float lastPing = 0;
        public float LastPing => lastPing;
        public void Ping()
        {
            this.lastPing = (float)UnityEditor.EditorApplication.timeSinceStartup;
        }
#endif
        public void SetWeight(int newWeight)
        {
            this.weight = newWeight;
        }
    }

    [System.Serializable]
    public class WeightedObject<T> : WeightedObject
    {
        [SerializeField]
        T contents;
        public T Contents => contents;
    }
}
