using System;

namespace MolecularLib.AutoAssign
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    [JetBrains.Annotations.MeansImplicitUse(JetBrains.Annotations.ImplicitUseKindFlags.Assign), JetBrains.Annotations.UsedImplicitly]
    public class FindWithTagAttribute : Attribute
    {
        public readonly string Tag;   
        
        public FindWithTagAttribute(string tag)
        {
            Tag = tag;
        }
    }
}