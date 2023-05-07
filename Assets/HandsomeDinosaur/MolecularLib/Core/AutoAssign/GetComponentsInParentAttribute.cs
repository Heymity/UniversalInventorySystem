using System;

namespace MolecularLib.AutoAssign
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    [JetBrains.Annotations.MeansImplicitUse(JetBrains.Annotations.ImplicitUseKindFlags.Assign), JetBrains.Annotations.UsedImplicitly]
    public class GetComponentsInParentAttribute : Attribute
    {
        public readonly Type ComponentType;
        
        public GetComponentsInParentAttribute(Type componentType)
        {
            ComponentType = componentType;
        }
    }
}