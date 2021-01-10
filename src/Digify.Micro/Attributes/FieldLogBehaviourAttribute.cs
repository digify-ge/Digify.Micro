using System;
using System.Collections.Generic;
using System.Text;

namespace Digify.Micro.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class FieldLogBehaviourAttribute : Attribute
    {
        public readonly FieldLogBehaviourTypeEnum Behaviuor;

        public FieldLogBehaviourAttribute(FieldLogBehaviourTypeEnum behaviuor = FieldLogBehaviourTypeEnum.LogFieldValue)
        {
            Behaviuor = behaviuor;
        }
    }

    public enum FieldLogBehaviourTypeEnum
    {
        LogFieldValue,
        IgnoreFieldValue,
        LogSensitiveField
    }
}
