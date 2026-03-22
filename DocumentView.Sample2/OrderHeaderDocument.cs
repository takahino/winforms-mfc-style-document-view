using DocumentView.Framework;
using static DocumentView.Sample2.ResourceId;

namespace DocumentView.Sample2;

/// <summary>
/// IDD_ORDER_HEADER に相当するDocument。
/// 発注ヘッダの DDX フィールド、ステータスロック、発注番号生成を管理する。
/// </summary>
public partial class OrderHeaderDocument : MfcDocument
{
    public OrderHeaderDocument(IMessageBoxService messageBoxService) : base(messageBoxService) { }

    [DDX(OrderView.Ctrl.IDC_EDIT_ORDER_NO)]
    public string m_strOrderNo = string.Empty;

    [DDX(OrderView.Ctrl.IDC_EDIT_ORDER_DATE)]
    public string m_strOrderDate = string.Empty;

    [DDX(OrderView.Ctrl.IDC_EDIT_DELIVERY_DATE)]
    public string m_strDeliveryDate = string.Empty;

    /// <summary>0=Not ordered 1=Ordered 2=Awaiting delivery 3=Received</summary>
    [DDX(OrderView.Ctrl.IDC_COMBO_STATUS)]
    public int m_nStatus = 0;

    [DDX(OrderView.Ctrl.IDC_CHECK_URGENT)]
    public bool m_bUrgent = false;

    [DDX(OrderView.Ctrl.IDC_EDIT_MEMO)]
    public string m_strMemo = string.Empty;

    /// <summary>フィールドを初期値に戻し、UI に反映する。</summary>
    public void Reset(string orderNo)
    {
        m_strOrderNo      = orderNo;
        m_strOrderDate    = DateTime.Today.ToString("yyyy/MM/dd");
        m_strDeliveryDate = string.Empty;
        m_nStatus         = 0;
        m_bUrgent         = false;
        m_strMemo         = string.Empty;
        UpdateData(false);
    }

    /// <summary>DTO からフィールドを復元し、UI に反映する。</summary>
    public void RestoreFrom(PurchaseOrderData d)
    {
        m_strOrderNo      = d.OrderNo;
        m_strOrderDate    = d.OrderDate;
        m_strDeliveryDate = d.DeliveryDate;
        m_nStatus         = d.Status;
        m_bUrgent         = d.Urgent;
        m_strMemo         = d.Memo;
        UpdateData(false);
    }

    /// <summary>
    /// ステータスが 3（Received）の場合、全コントロールをロック。
    /// IDC_COMBO_STATUS の SelectedIndexChanged から呼び出す。
    /// </summary>
    public void OnStatusChanged()
    {
        int status = (GetControl(IDC_COMBO_STATUS) is ComboBox cb)
            ? cb.SelectedIndex
            : m_nStatus;

        bool enabled = (status != 3);

        SetEnabled(IDC_EDIT_SUPPLIER_CODE,  enabled);
        SetEnabled(IDC_EDIT_SUPPLIER_NAME,  enabled);
        SetEnabled(IDC_EDIT_ADDRESS,        enabled);
        SetEnabled(IDC_EDIT_TEL,            enabled);
        SetEnabled(IDC_EDIT_FAX,            enabled);
        SetEnabled(IDC_EDIT_ORDER_NO,       enabled);
        SetEnabled(IDC_EDIT_ORDER_DATE,     enabled);
        SetEnabled(IDC_EDIT_DELIVERY_DATE,  enabled);
        SetEnabled(IDC_COMBO_STATUS,        enabled);
        SetEnabled(IDC_CHECK_URGENT,        enabled);
        SetEnabled(IDC_EDIT_MEMO,           enabled);
        SetEnabled(IDC_GRID_LINES,          enabled);
    }
}
