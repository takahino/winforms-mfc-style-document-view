namespace DocumentView.Sample2;

// ── JSON DTOs ──────────────────────────────────────────────────────────────
// 発注データを JSON で永続化するための DTO。
// SupplierDocument / OrderHeaderDocument / OrderDetailDocument 3クラスの
// データを1ファイルにまとめて保存・読み込みする。

public sealed class PurchaseOrderData
{
    public string SupplierCode  { get; set; } = string.Empty;
    public string SupplierName  { get; set; } = string.Empty;
    public string Address       { get; set; } = string.Empty;
    public string Tel           { get; set; } = string.Empty;
    public string Fax           { get; set; } = string.Empty;
    public string OrderNo       { get; set; } = string.Empty;
    public string OrderDate     { get; set; } = string.Empty;
    public string DeliveryDate  { get; set; } = string.Empty;
    public int    Status        { get; set; }
    public bool   Urgent        { get; set; }
    public string Memo          { get; set; } = string.Empty;
    public List<OrderLineData> Lines { get; set; } = [];
}

public sealed class OrderLineData
{
    public string  ItemCode  { get; set; } = string.Empty;
    public string  ItemName  { get; set; } = string.Empty;
    public int     Qty       { get; set; } = 1;
    public decimal UnitPrice { get; set; }
}
