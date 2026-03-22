using DocumentView.Framework;

namespace DocumentView.Sample2;

/// <summary>
/// IDD_SUPPLIER_INFO に相当するDocument。
/// 仕入先情報の DDX/DDV フィールドとバリデーションを管理する。
/// </summary>
public partial class SupplierDocument : MfcDocument
{
    public SupplierDocument(IMessageBoxService messageBoxService) : base(messageBoxService) { }

    [DDX(OrderView.Ctrl.IDC_EDIT_SUPPLIER_CODE)]
    [DDVMaxChars(10)]
    public string m_strSupplierCode = string.Empty;

    [DDX(OrderView.Ctrl.IDC_EDIT_SUPPLIER_NAME)]
    [DDVMaxChars(50)]
    public string m_strSupplierName = string.Empty;

    [DDX(OrderView.Ctrl.IDC_EDIT_ADDRESS)]
    [DDVMaxChars(100)]
    public string m_strAddress = string.Empty;

    [DDX(OrderView.Ctrl.IDC_EDIT_TEL)]
    [DDVMaxChars(20)]
    public string m_strTel = string.Empty;

    [DDX(OrderView.Ctrl.IDC_EDIT_FAX)]
    [DDVMaxChars(20)]
    public string m_strFax = string.Empty;

    /// <summary>フィールドを初期値に戻し、UI に反映する。</summary>
    public void Reset()
    {
        m_strSupplierCode = string.Empty;
        m_strSupplierName = string.Empty;
        m_strAddress      = string.Empty;
        m_strTel          = string.Empty;
        m_strFax          = string.Empty;
        UpdateData(false);
    }

    /// <summary>DTO からフィールドを復元し、UI に反映する。</summary>
    public void RestoreFrom(PurchaseOrderData d)
    {
        m_strSupplierCode = d.SupplierCode;
        m_strSupplierName = d.SupplierName;
        m_strAddress      = d.Address;
        m_strTel          = d.Tel;
        m_strFax          = d.Fax;
        UpdateData(false);
    }

    public bool ValidateBusinessRule(out string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(m_strSupplierCode))
        {
            errorMessage = "Supplier code is required.";
            return false;
        }
        errorMessage = string.Empty;
        return true;
    }
}
