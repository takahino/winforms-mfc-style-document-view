using DocumentView.Framework;
using static DocumentView.Sample2.OrderView;

namespace DocumentView.Sample2;

/// <summary>
/// Equivalent to MFC resource.h — 13 IDC_* resource IDs for the purchase-order form.
/// <see cref="AutoIdAttribute"/> assigns sequential IDs in declaration order.
/// </summary>
public static class ResourceId
{
    [AutoId(Ctrl.IDC_EDIT_SUPPLIER_CODE)]  public static int IDC_EDIT_SUPPLIER_CODE;   // 1
    [AutoId(Ctrl.IDC_EDIT_SUPPLIER_NAME)]  public static int IDC_EDIT_SUPPLIER_NAME;   // 2
    [AutoId(Ctrl.IDC_EDIT_ADDRESS)]        public static int IDC_EDIT_ADDRESS;         // 3
    [AutoId(Ctrl.IDC_EDIT_TEL)]            public static int IDC_EDIT_TEL;             // 4
    [AutoId(Ctrl.IDC_EDIT_FAX)]            public static int IDC_EDIT_FAX;             // 5
    [AutoId(Ctrl.IDC_EDIT_ORDER_NO)]       public static int IDC_EDIT_ORDER_NO;        // 6
    [AutoId(Ctrl.IDC_EDIT_ORDER_DATE)]     public static int IDC_EDIT_ORDER_DATE;      // 7
    [AutoId(Ctrl.IDC_EDIT_DELIVERY_DATE)]  public static int IDC_EDIT_DELIVERY_DATE;   // 8
    [AutoId(Ctrl.IDC_COMBO_STATUS)]        public static int IDC_COMBO_STATUS;         // 9
    [AutoId(Ctrl.IDC_CHECK_URGENT)]        public static int IDC_CHECK_URGENT;         // 10
    [AutoId(Ctrl.IDC_EDIT_MEMO)]           public static int IDC_EDIT_MEMO;            // 11
    [AutoId(Ctrl.IDC_GRID_LINES)]          public static int IDC_GRID_LINES;           // 12
    [AutoId(Ctrl.IDC_LABEL_TOTAL)]         public static int IDC_LABEL_TOTAL;          // 13

    static ResourceId() => AutoIdAssigner.Assign(typeof(ResourceId));
}
