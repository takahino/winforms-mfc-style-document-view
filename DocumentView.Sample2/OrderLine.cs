using System.ComponentModel;

namespace DocumentView.Sample2;

/// <summary>
/// One row of the purchase order detail grid.
/// Implements <see cref="INotifyPropertyChanged"/> so cell edits flow immediately to the document.
/// Amount = Qty × UnitPrice is a computed property.
/// </summary>
public class OrderLine : INotifyPropertyChanged
{
    private string  _itemCode  = string.Empty;
    private string  _itemName  = string.Empty;
    private int     _qty       = 1;
    private decimal _unitPrice = 0m;

    public string ItemCode
    {
        get => _itemCode;
        set { _itemCode = value; OnPropertyChanged(nameof(ItemCode)); }
    }

    public string ItemName
    {
        get => _itemName;
        set { _itemName = value; OnPropertyChanged(nameof(ItemName)); }
    }

    public int Qty
    {
        get => _qty;
        set { _qty = value; OnPropertyChanged(nameof(Qty)); OnPropertyChanged(nameof(Amount)); }
    }

    public decimal UnitPrice
    {
        get => _unitPrice;
        set { _unitPrice = value; OnPropertyChanged(nameof(UnitPrice)); OnPropertyChanged(nameof(Amount)); }
    }

    /// <summary>Qty × UnitPrice — computed, not stored.</summary>
    public decimal Amount => Qty * UnitPrice;

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string name) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
