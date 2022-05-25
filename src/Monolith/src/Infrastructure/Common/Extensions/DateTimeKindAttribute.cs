using System;
using System.Linq;
using System.Reflection;

namespace Nuyken.VeGasCo.Backend.Infrastructure.Common.Extensions;

/// <summary>
///     Sets the DateTime.Kind value on DateTime and DateTime? members retrieved by Entity Framework. Sets Kind to
///     DateTimeKind.Utc by default.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class DateTimeKindAttribute : Attribute
{
    /// <summary>Specifies the DateTime.Kind value to set on the returned DateTime value.</summary>
    /// <param name="kind">The DateTime.Kind value to set on the returned DateTime value.</param>
    public DateTimeKindAttribute(DateTimeKind kind)
    {
        Kind = kind;
    }

    /// <summary>The DateTime.Kind value to set into the returned value.</summary>
    public DateTimeKind Kind { get; }

    /// <summary>Event handler to connect to the ObjectContext.ObjectMaterialized event.</summary>
    /// <param name="entity">The entity (POCO class) being materialized.</param>
    /// <param name="defaultKind">[Optional] The Kind property to set on all DateTime objects by default.</param>
    public static void Apply(object entity, DateTimeKind? defaultKind = null)
    {
        if (entity is null) return;

        // Get the PropertyInfos for all of the DateTime and DateTime? properties on the entity
        var properties = entity.GetType().GetProperties()
            .Where(x => x.PropertyType == typeof(DateTime) || x.PropertyType == typeof(DateTime?));

        // For each DateTime or DateTime? property on the entity...
        foreach (var propInfo in properties)
        {
            // Initialization
            var kind = defaultKind;

            // Get the kind value from the [DateTimekind] attribute if it's present
            var kindAttr = propInfo.GetCustomAttribute<DateTimeKindAttribute>();
            kind = kindAttr?.Kind;

            // Set the Kind property
            if (kind is not null)
            {
                var dt = propInfo.PropertyType == typeof(DateTime?)
                    ? (DateTime?) propInfo.GetValue(entity)
                    : (DateTime) propInfo.GetValue(entity);

                if (dt is not null) propInfo.SetValue(entity, DateTime.SpecifyKind(dt.Value, kind.Value));
            }
        }
    }
}