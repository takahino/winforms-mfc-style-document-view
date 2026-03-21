using DocumentView.Framework;
using Microsoft.Extensions.DependencyInjection;

namespace DocumentView.Sample;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        var services = new ServiceCollection();
        services.AddSingleton<IMessageBoxService, MessageBoxService>();
        services.AddTransient<SampleDocument>();
        services.AddTransient<SampleView>();
        services.AddSingleton<MfcWinApp, SampleWinApp>();

        using var provider = services.BuildServiceProvider();
        provider.GetRequiredService<MfcWinApp>().Run();
    }
}
