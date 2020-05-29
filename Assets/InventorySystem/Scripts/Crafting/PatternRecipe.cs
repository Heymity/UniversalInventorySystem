using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UniversalInventorySystem
{

    [AddComponentMenu("UniversalInventorySystem/PatternRecipe"), CreateAssetMenu(fileName = "PatternRecipe", menuName = "UniversalInventorySystem/PatternRecipe", order = 1), System.Serializable]
    public class PatternRecipe : ScriptableObject
    {
        public int numberOfFactors;
        public Item[] factors;

        public int numberOfProducts;
        public Item[] products;
        public int[] amountProducts;

        public Vector2Int gridSize;

        public Item[] pattern;
        public int[] amountPattern;

        public int id;
        public string key;
    }
}

