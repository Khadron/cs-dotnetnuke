using System;

namespace DotNetNuke.Data
{
    [AttributeUsage( AttributeTargets.Parameter )]
    public sealed class NonCommandParameterAttribute : Attribute
    {
    }
}