using System.ComponentModel;
using DocumentView.Framework;
using static DocumentView.Sample2.ResourceId;

namespace DocumentView.Sample2;

/// <summary>
/// IDD_ORDER_DETAIL に相当するDocument。
/// 発注明細グリッドと合計金額の DDX フィールド、リアルタイム集計を管理する。
/// </summary>
public partial class OrderDetailDocument : MfcDocument
{
    public OrderDetailDocument(IMessageBoxService messageBoxService) : base(messageBoxService) { }

    [DDX(OrderView.Ctrl.IDC_GRID_LINES)]
    public BindingList<OrderLine> m_gridLines = new();

    [DDX(OrderView.Ctrl.IDC_LABEL_TOTAL)]
    public string m_strTotal = "¥0";

    public void Reset()
    {
        m_gridLines = new BindingList<OrderLine>();
        m_strTotal  = "¥0";
    }

    /// <summary>
    /// m_gridLines の合計を再計算し、IDC_LABEL_TOTAL を直接更新する。
    /// </summary>
    public void RecalculateTotal()
    {
        var total  = m_gridLines.Sum(l => l.Amount);
        m_strTotal = $"¥{total:#,##0}";

        if (GetControl(IDC_LABEL_TOTAL) is Label lbl)
            lbl.Text = m_strTotal;
    }

    /// <summary>
    /// m_gridLines の変更イベントを購読し、変更のたびに RecalculateTotal を呼び出す。
    /// m_gridLines を差し替えた後（Reset / FromData の直後）に呼び出すこと。
    /// </summary>
    public void SubscribeGridLines()
    {
        m_gridLines.ListChanged += (_, _) => RecalculateTotal();
        foreach (var line in m_gridLines)
            line.PropertyChanged += (_, _) => RecalculateTotal();
        RecalculateTotal();
    }
}
