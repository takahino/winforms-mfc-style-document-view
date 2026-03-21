using System.Reflection;

namespace DocumentView.Framework;

/// <summary>
/// Reflects a resource.h-style class of <c>#define IDC_*</c> constants.
/// Bidirectional resolution: int (resource id value) ↔ string ("IDC_EDIT1").
/// </summary>
public sealed class ResourceIdResolver
{
    private readonly Dictionary<int, string> _idToName = [];
    private readonly Dictionary<string, int> _nameToId = [];

    public ResourceIdResolver(Type resourceIdType)
    {
        const BindingFlags flags =
            BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly;

        foreach (var fi in resourceIdType.GetFields(flags))
        {
            if (fi.FieldType != typeof(int)) continue;

            int id;
            AutoIdAttribute? autoId = null;
            if (fi.IsLiteral)
                id = (int)fi.GetRawConstantValue()!;
            else if ((autoId = fi.GetCustomAttribute<AutoIdAttribute>(false)) is not null)
                id = (int)fi.GetValue(null)!;
            else
                continue;

            string controlName = autoId?.ControlName ?? fi.Name;
            _idToName[id] = controlName;
            _nameToId[controlName] = id;
        }
    }

    public bool TryGetName(int id, out string name) => _idToName.TryGetValue(id, out name!);
    public bool TryGetId(string name, out int id) => _nameToId.TryGetValue(name, out id);
    public IEnumerable<KeyValuePair<int, string>> AllEntries => _idToName;
}
