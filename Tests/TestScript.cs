using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using WeightedObjects;

public class TestScript
{
    [Test]
    public void RandomWeightedNonRepeat()
    {
        // Use the Assert class to test conditions

        var newCollection = new WeightedObjectCollection<int>();

        var testAmount = 20;

        newCollection.randomType = RandomType.WeightedRandom;
        newCollection.canRepeat = false;

        for (int i = 0; i < testAmount; i++)
        {
            newCollection.Add(i, Random.Range(0,100));
        }

        var ranSelections = new List<int>();

        for (int i = 0; i < testAmount; i++)
        {
            var ran = newCollection.GetRandom(0);

            CollectionAssert.DoesNotContain(ranSelections, ran);

            ranSelections.Add(ran);
        }
    }

    // A Test behaves as an ordinary method
    [Test]
    public void RandomExhaustiveSelection()
    {
        // Use the Assert class to test conditions

        var newCollection = new WeightedObjectCollection<int>();
        
        var cycles = 10;

        var testAmount = 200;

        newCollection.randomType = RandomType.RandomExhaustive;
        for (int i = 0; i < testAmount; i++)
        {
            newCollection.Add(i, 1);
        }

        for (int x = 0; x < cycles; x++)
        {
            var ranSelections = new List<int>();

            for (int i = 0; i < testAmount; i++)
            {
                var ran = newCollection.GetRandom(0);

                CollectionAssert.DoesNotContain(ranSelections, ran);

                ranSelections.Add(ran);
            }
        }
    }

    //// A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    //// `yield return null;` to skip a frame.
    //[UnityTest]
    //public IEnumerator Test2()
    //{
    //    // Use the Assert class to test conditions.
    //    // Use yield to skip a frame.
    //    yield return null;
    //}
}
