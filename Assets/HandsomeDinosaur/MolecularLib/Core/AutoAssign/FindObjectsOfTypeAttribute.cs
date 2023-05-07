using System;

namespace MolecularLib.AutoAssign
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    [JetBrains.Annotations.MeansImplicitUse(JetBrains.Annotations.ImplicitUseKindFlags.Assign), JetBrains.Annotations.UsedImplicitly]
    public class FindObjectsOfTypeAttribute : Attribute
    {
        public readonly Type Type;

        public FindObjectsOfTypeAttribute(Type type = null)
        {
            Type = type;
        }
    }
}