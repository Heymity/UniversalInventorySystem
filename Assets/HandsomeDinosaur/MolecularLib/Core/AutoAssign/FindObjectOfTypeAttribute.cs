using System;

namespace MolecularLib.AutoAssign
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    [JetBrains.Annotations.MeansImplicitUse(JetBrains.Annotations.ImplicitUseKindFlags.Assign), JetBrains.Annotations.UsedImplicitly]
    public class FindObjectOfTypeAttribute : Attribute
    {
        public readonly Type Type; 
        
        public FindObjectOfTypeAttribute(Type type = null)
        {
            Type = type;
        }
    }
}