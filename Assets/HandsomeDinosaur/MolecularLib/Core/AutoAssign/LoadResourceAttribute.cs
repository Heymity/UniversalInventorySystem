using System;

namespace MolecularLib.AutoAssign
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    [JetBrains.Annotations.MeansImplicitUse(JetBrains.Annotations.ImplicitUseKindFlags.Assign), JetBrains.Annotations.UsedImplicitly]
    public class LoadResourceAttribute : Attribute
    {
        public readonly string ResourcePath;

        public LoadResourceAttribute(string resourcePath)
        {
            ResourcePath = resourcePath;
        }
    }
}