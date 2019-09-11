using System;

namespace ExileCore.Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Method)]
    public class HideInReflectionAttribute : Attribute
    {
    }
}
