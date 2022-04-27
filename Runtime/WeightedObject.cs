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

    [System.Serializable]
    public class WeightedObject<T>
    {
        [SerializeField]
        float weight = 1;
        public float Weight => weight;

        [SerializeField]
        T contents;
        public T Contents => contents;
    }
}
