using System;

namespace MakeKits.Workshop;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
public sealed class WorkshopAttribute(string id) : Attribute
{
    /// <summary>
    /// The unique identifier for the workshop plugin.
    /// </summary>
    public string Id { get; set; } = id;
}
