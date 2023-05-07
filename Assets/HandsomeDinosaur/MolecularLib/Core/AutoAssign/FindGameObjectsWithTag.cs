using System;

namespace MolecularLib.AutoAssign
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    [JetBrains.Annotations.MeansImplicitUse(JetBrains.Annotations.ImplicitUseKindFlags.Assign), JetBrains.Annotations.UsedImplicitly]
    public class FindGameObjectsWithTag : Attribute
    {
        public readonly string Tag;
        
        public FindGameObjectsWithTag(string tag)
        {
            Tag = tag;
        }
    }
}