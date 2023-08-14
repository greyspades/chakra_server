using System;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public class BypassInputFormatterAttribute : Attribute
{
}