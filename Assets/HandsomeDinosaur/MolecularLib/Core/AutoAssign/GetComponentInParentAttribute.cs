using System;

namespace MolecularLib.AutoAssign
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    [JetBrains.Annotations.MeansImplicitUse(JetBrains.Annotations.ImplicitUseKindFlags.Assign), JetBrains.Annotations.UsedImplicitly]
    public class GetComponentInParentAttribute : Attribute
    {
        public readonly Type ComponentType;
        
        public GetComponentInParentAttribute(Type componentType)
        {
            ComponentType = componentType;
        }
    }
}