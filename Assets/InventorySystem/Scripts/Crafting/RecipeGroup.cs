/*  Copyright 2020 Gabriel Pasquale Rodrigues Scavone
 *
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 * 
 * 
 *  
 *  This is a group of recipes, a recipe database
 * 
 */ 

using System.Collections.Generic;
using UnityEngine;

namespace UniversalInventorySystem
{
    [AddComponentMenu("UniversalInventorySystem/RecipeGroup"), CreateAssetMenu(fileName = "RecipeGroup", menuName = "UniversalInventorySystem/RecipeGroup", order = 133), System.Serializable]
    public class RecipeGroup : ScriptableObject
    {
        public List<Recipe> recipesList = new List<Recipe>();
        public List<PatternRecipe> receipePatternsList = new List<PatternRecipe>();
        [Space]
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
}