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
 *  This is the scriptable object for pattern recipes.  
 */
 
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

