using System.Reflection;

namespace DocumentView.Framework;

/// <summary>
/// Assigns sequential IDs to static int fields marked with <see cref="AutoIdAttribute"/>.
/// Call from the ResourceId static constructor.
/// </summary>
public static class AutoIdAssigner
{
    public static void Assign(Type type, int startId = 1)
    {
        const BindingFlags flags =
            BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly;

        int next = startId;
        foreach (var fi in type.GetFields(flags))
        {
            if (fi.FieldType == typeof(int) && fi.IsDefined(typeof(AutoIdAttribute), false))
                fi.SetValue(null, next++);
        }
    }
}
