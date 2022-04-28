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
    }
}
