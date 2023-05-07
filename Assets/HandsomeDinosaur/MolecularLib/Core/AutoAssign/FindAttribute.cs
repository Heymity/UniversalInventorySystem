using System;

namespace MolecularLib.AutoAssign
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    [JetBrains.Annotations.MeansImplicitUse(JetBrains.Annotations.ImplicitUseKindFlags.Assign), JetBrains.Annotations.UsedImplicitly]
    public class FindAttribute : Attribute
    {
        public readonly string Name;
        
        public FindAttribute(string name)
        {
            Name = name;
        }
    }
}