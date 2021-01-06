using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

public static class EnumExtensions
{
    public static IEnumerable<TEnum> GetEnumValues<TEnum>(this TEnum value)
        where TEnum : Enum
        => Enum.GetValues(value.GetType()).Cast<TEnum>().OrderBy(x => x).ToArray();

    public static IEnumerable<TEnum> GetFlags<TEnum>(this TEnum value)
        where TEnum : Enum
        => GetFlags(value, Enum.GetValues(value.GetType()).Cast<TEnum>().ToArray());

    public static IEnumerable<TEnum> GetIndividualFlags<TEnum>(this TEnum value)
        where TEnum : Enum
        => GetFlags<TEnum>(value, GetFlagValues<TEnum>(value.GetType()).ToArray());

    public static string GetDescription(this Enum value)
        => value.GetAttribute<DescriptionAttribute>().Description;

    private static TAttribute GetAttribute<TAttribute>(this Enum value)
        where TAttribute : Attribute
    {
        var type = value.GetType();
        var name = Enum.GetName(type, value);
        return type.GetField(name).GetCustomAttribute<TAttribute>();
    }

    private static IEnumerable<TEnum> GetFlags<TEnum>(TEnum value, TEnum[] values)
        where TEnum : Enum
    {
        var bits = Convert.ToUInt64(value);
        var results = new List<TEnum>();
        for (int i = values.Length - 1; i >= 0; i--)
        {
            var mask = Convert.ToUInt64(values[i]);
            if (i == 0 && mask == 0L)
                break;
            if ((bits & mask) == mask)
            {
                results.Add(values[i]);
                bits -= mask;
            }
        }
        if (bits != 0L)
            return Enumerable.Empty<TEnum>();
        if (Convert.ToUInt64(value) != 0L)
            return results.Reverse<TEnum>();
        if (bits == Convert.ToUInt64(value) && values.Length > 0 && Convert.ToUInt64(values[0]) == 0L)
            return values.Take(1);
        return Enumerable.Empty<TEnum>();
    }

    private static IEnumerable<TEnum> GetFlagValues<TEnum>(Type enumType)
        where TEnum : Enum
    {
        ulong flag = 0x1;
        foreach (var value in Enum.GetValues(enumType).Cast<TEnum>())
        {
            ulong bits = Convert.ToUInt64(value);
            if (bits == 0L)
                //yield return value;
                continue; // skip the zero value
            while (flag < bits) flag <<= 1;
            if (flag == bits)
                yield return value;
        }
    }

    //public static Dictionary<TEnum, DisplayAttribute> GetOrderedValueMapping<TEnum>()
    //{
    //    var mapping = new List<(TEnum, DisplayAttribute)>();
    //    var enumValues = Enum.GetValues(typeof(TEnum));

    //    foreach (TEnum item in enumValues)
    //    {
    //        var display = item.GetDisplayAttribute();

    //        mapping.Add((item, display));
    //    }

    //    mapping = mapping.OrderBy(x => x.Item2?.GetOrder() ?? 0).ToList();

    //    return mapping.ToDictionary(x => x.Item1, y => y.Item2);
    //}

    //public static DisplayAttribute GetDisplayAttribute(this object item)
    //{
    //    var field = item.GetType().GetField(item.ToString());
    //    return (field?
    //        .GetCustomAttributes(typeof(DisplayAttribute), true)
    //        .FirstOrDefault() as DisplayAttribute);
    //}

}
