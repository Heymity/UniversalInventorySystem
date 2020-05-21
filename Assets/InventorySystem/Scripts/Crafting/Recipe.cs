using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

[AddComponentMenu("UniversalInventorySystem/Recipe"), CreateAssetMenu(fileName = "Recipe", menuName = "UniversalInventorySystem/Recipe", order = 1), System.Serializable]
public class Recipe : ScriptableObject
{
    public int numberOfFactors;
    public Item[] factors;

    public int numberOfProducts;
    public Item[] products;

    public int id;
    public string key;
}
