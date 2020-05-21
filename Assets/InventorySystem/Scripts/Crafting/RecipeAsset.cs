using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[AddComponentMenu("UniversalInventorySystem/RecipeAsset"), CreateAssetMenu(fileName = "RecipeAsset", menuName = "UniversalInventorySystem/RecipeAsset", order = 10), System.Serializable]
public class RecipeAsset : ScriptableObject
{
    public List<Recipe> recipesList = new List<Recipe>();
    public List<PatternRecipe> receipePatternsList = new List<PatternRecipe>();

    public string strId;
    public int id;

    public Recipe GetRecipeAtIndex(int index) { return recipesList[index]; }

    public Recipe GetRecipeWithName(string _key)
    {
        foreach (Recipe i in recipesList)
        {
            if (i.key == _key) return i;
        }

        return null;
    }

    public Recipe GetRecipeWithID(int _id)
    {
        foreach (Recipe i in recipesList)
        {
            if (i.id == _id) return i;
        }

        return null;
    }

    public List<Recipe> OrderRecipeById()
    {
        return RecipeInsertionSort(recipesList);
    }

    static List<Recipe> RecipeInsertionSort(List<Recipe> inputArray)
    {
        for (int i = 0; i < inputArray.Count - 1; i++)
        {
            for (int j = i + 1; j > 0; j--)
            {
                if (inputArray[j - 1].id > inputArray[j].id)
                {
                    int temp = inputArray[j - 1].id;
                    inputArray[j - 1].id = inputArray[j].id;
                    inputArray[j].id = temp;
                }
            }
        }
        return inputArray;
    }

    public PatternRecipe GetRecipePatternAtIndex(int index) { return receipePatternsList[index]; }

    public PatternRecipe GetRecipePatternWithKey(string _key)
    {
        foreach (PatternRecipe i in receipePatternsList)
        {
            if (i.key == _key) return i;
        }

        return null;
    }

    public PatternRecipe GetRecipePatternWithID(int _id)
    {
        foreach (PatternRecipe i in receipePatternsList)
        {
            if (i.id == _id) return i;
        }

        return null;
    }

    public List<PatternRecipe> OrderRecipePatternById()
    {
        return RecipePatternInsertionSort(receipePatternsList);
    }

    static List<PatternRecipe> RecipePatternInsertionSort(List<PatternRecipe> inputArray)
    {
        for (int i = 0; i < inputArray.Count - 1; i++)
        {
            for (int j = i + 1; j > 0; j--)
            {
                if (inputArray[j - 1].id > inputArray[j].id)
                {
                    int temp = inputArray[j - 1].id;
                    inputArray[j - 1].id = inputArray[j].id;
                    inputArray[j].id = temp;
                }
            }
        }
        return inputArray;
    }
}
