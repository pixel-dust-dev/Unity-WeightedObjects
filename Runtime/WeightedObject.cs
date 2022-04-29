using System;
using UnityEngine;

namespace WeightedObjects
{
    [System.Serializable]
    public class WeightedObject<T>
    {
        [SerializeField]
        float weight = 1;
        public float Weight => weight;

        [SerializeField]
        T contents;
        public T Contents => contents;

#if UNITY_EDITOR
        //Used to display a little flash of color in editor
        float lastPing = Mathf.Infinity;
        public float LastPing => lastPing;
        public void Ping()
        {
            this.lastPing = (float)UnityEditor.EditorApplication.timeSinceStartup;
        }
#endif
    }
}
