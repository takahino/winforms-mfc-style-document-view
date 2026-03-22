using DocumentView.Framework;
using Microsoft.Extensions.DependencyInjection;

namespace DocumentView.Sample2;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        var services = new ServiceCollection();
        services.AddSingleton<IMessageBoxService, MessageBoxService>();

        // 3つのDocumentを個別登録（MFCの3ダイアログに対応）
        services.AddTransient<SupplierDocument>();
        services.AddTransient<OrderHeaderDocument>();
        services.AddTransient<OrderDetailDocument>();

        services.AddTransient<OrderView>();
        services.AddSingleton<MfcWinApp, PurchaseOrderWinApp>();

        using var provider = services.BuildServiceProvider();
        provider.GetRequiredService<MfcWinApp>().Run();
    }
}
