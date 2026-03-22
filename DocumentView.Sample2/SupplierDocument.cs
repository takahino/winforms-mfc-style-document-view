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

    public void Reset()
    {
        m_strSupplierCode = string.Empty;
        m_strSupplierName = string.Empty;
        m_strAddress      = string.Empty;
        m_strTel          = string.Empty;
        m_strFax          = string.Empty;
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
