using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace MolecularLib
{
    public static class TypeLibrary
    {
        public static bool IsInitialized { get; private set; } = false;
        
        private static IEnumerable<Type> _allNonUnityAssembliesTypes;
        private static IEnumerable<Type> _allAssembliesTypes;
        private static IDictionary<string, Assembly> _allAssemblies;

        public static IEnumerable<Type> AllAssembliesTypes
        {
            get
            {
                if (_allAssembliesTypes is null) Bootstrap();
                return _allAssembliesTypes;
            }
            private set => _allAssembliesTypes = value;
        }

        public static IEnumerable<Type> AllNonUnityAssembliesTypes
        {
            get
            {
                if (_allNonUnityAssembliesTypes is null) Bootstrap();
                return _allNonUnityAssembliesTypes;
            }
            private set => _allNonUnityAssembliesTypes = value;
        }

        public static IDictionary<string, Assembly> AllAssemblies
        {
            get
            {
                if (_allAssemblies is null) Bootstrap();
                return _allAssemblies;
            }
            private set => _allAssemblies = value;
        }

        /*public static IEnumerable<Type> AllAssembliesTypes
        {
            get => throw new Exception("The USE_TYPE_LIBRARY precompiler symbol is disabled, therefore this feature is no enabled. To enable it add USE_TYPE_LIBRARY to the script define symbols in unity project settings");
            set => throw new Exception("The USE_TYPE_LIBRARY precompiler symbol is disabled, therefore this feature is no enabled. To enable it add USE_TYPE_LIBRARY to the script define symbols in unity project settings");
        }
        public static IEnumerable<Type> AllNonUnityAssembliesTypes
        {
            get => throw new Exception("The USE_TYPE_LIBRARY precompiler symbol is disabled, therefore this feature is no enabled. To enable it add USE_TYPE_LIBRARY to the script define symbols in unity project settings");
            set => throw new Exception("The USE_TYPE_LIBRARY precompiler symbol is disabled, therefore this feature is no enabled. To enable it add USE_TYPE_LIBRARY to the script define symbols in unity project settings");
        }*/

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void Bootstrap()
        {
            if (IsInitialized) return;
            
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            
            AllAssembliesTypes = assemblies
                .Select(a => a.GetTypes())
                .Aggregate(new List<Type>() as IEnumerable<Type>,(a, s) => a.Concat(s));

            AllNonUnityAssembliesTypes = assemblies
                .Where(a => !IsAssemblyFromUnity(a.FullName))
                .Select(a => a.GetTypes())
                .Aggregate(new List<Type>() as IEnumerable<Type>,(a, s) => a.Concat(s));

            var seenAssemblies = new HashSet<string>();
            AllAssemblies = assemblies
                .Select(a => (assembly: a, name: a.GetName().Name))
                .Where(a => seenAssemblies.Add(a.name))
                .ToDictionary(a => a.name, a => a.assembly);
            
            /*PlayerAssembliesTypes = CompilationPipeline
                .GetAssemblies(AssembliesType.Player).Select(a => a.)
                .Aggregate(new List<Type>() as IEnumerable<Type>,(a, s) => a.Concat(s));*/

            bool IsAssemblyFromUnity(string assemblyName)
            {
                return assemblyName.Split('.')[0] == "Unity"
                       || assemblyName.Contains("UnityEngine")
                       || assemblyName.Contains("UnityEditor")
                       || assemblyName.Contains("UnityEngineInternal")
                       || assemblyName.Contains("UnityEditorInternal");
            }

            IsInitialized = true;
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        public static void BootstrapEditor()
        {
            Bootstrap();
        }
        #endif
    }
}
