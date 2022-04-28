using UnityEngine;

namespace WeightedObjects
{
    [System.Serializable]
    public class WeightedObjectCollection<T>
    {
        [SerializeField]
        WeightedObject<T>[] weightedObjects = new WeightedObject<T>[] { };

        public int Length => weightedObjects.Length;

        public T GetRandom()
        {
            return RandomExtensions.GetRandomWeightedIndex<T>(weightedObjects);
        }
    }
}
