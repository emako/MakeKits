using System;

namespace MakeKits.Workshop.WPF;

public static class TypeExtensions
{
    public static T CreateInstance<T>(this Type t, params object[] paramArray)
    {
        return (T)Activator.CreateInstance(t, paramArray)!;
    }
}
