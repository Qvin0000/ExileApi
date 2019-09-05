using System;

namespace Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Method)]
    public class HideInReflectionAttribute : Attribute
    {
    }
}