using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("UniversalInventorySystem/PatternRecipe"), CreateAssetMenu(fileName = "PatternRecipe", menuName = "UniversalInventorySystem/PatternRecipe", order = 1), System.Serializable]
public class PatternRecipe : ScriptableObject
{
    public int numberOfFactors;
    public Item[] factors;

    public int numberOfProducts;
    public Item[] products;

    public Vector2Int gridSize;

    public Item[] pattern;

    public int id;
    public string key;
}
