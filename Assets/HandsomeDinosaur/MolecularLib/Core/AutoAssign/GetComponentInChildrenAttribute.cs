using System;

namespace MolecularLib.AutoAssign
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    [JetBrains.Annotations.MeansImplicitUse(JetBrains.Annotations.ImplicitUseKindFlags.Assign), JetBrains.Annotations.UsedImplicitly]
    public class GetComponentInChildrenAttribute : Attribute
    {
        public readonly Type ComponentType;

        public GetComponentInChildrenAttribute(Type componentType = null)
        {
            ComponentType = componentType;
        }
    }
}