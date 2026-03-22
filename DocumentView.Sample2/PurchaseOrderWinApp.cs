using DocumentView.Framework;
using Microsoft.Extensions.DependencyInjection;

namespace DocumentView.Sample2;

/// <summary>
/// Purchase-order application entry point.
/// Main window is <see cref="OrderView"/>.
/// </summary>
public sealed class PurchaseOrderWinApp : MfcWinApp
{
    public PurchaseOrderWinApp(IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
    }

    protected override Form CreateMainForm() =>
        ServiceProvider.GetRequiredService<OrderView>();
}
