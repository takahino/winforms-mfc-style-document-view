using DocumentView.Framework;
using static DocumentView.Sample.SampleView;

namespace DocumentView.Sample;

/// <summary>
/// Equivalent to MFC resource.h.
/// <see cref="AutoIdAttribute"/> on static int fields assigns sequential IDs in declaration order.
/// Pass <see cref="Ctrl"/> constants for <c>controlName</c> to document the control mapping.
/// </summary>
public static class ResourceId
{
    [AutoId(Ctrl.IDC_EDIT_NAME)]     public static int IDC_EDIT_NAME;      // 1
    [AutoId(Ctrl.IDC_EDIT_AGE)]      public static int IDC_EDIT_AGE;       // 2
    [AutoId(Ctrl.IDC_EDIT_MEMO)]     public static int IDC_EDIT_MEMO;      // 3
    [AutoId(Ctrl.IDC_CHECK_ACTIVE)]  public static int IDC_CHECK_ACTIVE;   // 4
    [AutoId(Ctrl.IDC_CHECK_NOTIFY)]  public static int IDC_CHECK_NOTIFY;   // 5
    [AutoId(Ctrl.IDC_COMBO_PREF)]    public static int IDC_COMBO_PREF;     // 6
    [AutoId(Ctrl.IDC_COMBO_LANG)]    public static int IDC_COMBO_LANG;     // 7
    [AutoId(Ctrl.IDC_SPIN_PRIORITY)] public static int IDC_SPIN_PRIORITY;  // 8
    [AutoId(Ctrl.IDC_RADIO_MALE)]    public static int IDC_RADIO_MALE;     // 9
    [AutoId(Ctrl.IDC_RADIO_FEMALE)]  public static int IDC_RADIO_FEMALE;   // 10
    [AutoId(Ctrl.IDC_LIST_ITEMS)]    public static int IDC_LIST_ITEMS;     // 11
    [AutoId(Ctrl.IDC_STATIC_STATUS)] public static int IDC_STATIC_STATUS;  // 12
    [AutoId(Ctrl.IDC_GRID_ITEMS)]    public static int IDC_GRID_ITEMS;     // 13
    [AutoId(Ctrl.IDC_EDIT_EMPNO)]    public static int IDC_EDIT_EMPNO;     // 14
    [AutoId(Ctrl.IDC_RADIO_FULL_TIME)] public static int IDC_RADIO_FULL_TIME; // 15
    [AutoId(Ctrl.IDC_RADIO_CONTRACT)] public static int IDC_RADIO_CONTRACT; // 16
    [AutoId(Ctrl.IDC_RADIO_PART_TIME)] public static int IDC_RADIO_PART_TIME; // 17

    static ResourceId() => AutoIdAssigner.Assign(typeof(ResourceId));
}
