using System;

namespace MolecularLib.AutoAssign
{
    [AttributeUsage(AttributeTargets.Class)]
    [JetBrains.Annotations.UsedImplicitly]
    public class UseAutoAssignAttribute : Attribute
    {
        //public readonly AutoAssignAt DefaultAutoAssignMoment;
        
        public UseAutoAssignAttribute(/*AutoAssignAt defaultAutoAssignMoment = AutoAssignAt.Awake*/)
        {
            //this.DefaultAutoAssignMoment = defaultAutoAssignMoment;
        }
    }
}