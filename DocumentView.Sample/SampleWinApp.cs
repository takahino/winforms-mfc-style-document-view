using DocumentView.Framework;
using Microsoft.Extensions.DependencyInjection;

namespace DocumentView.Sample;

/// <summary>
/// Sample <see cref="MfcWinApp"/> implementation; main window is <see cref="SampleView"/>.
/// </summary>
public sealed class SampleWinApp : MfcWinApp
{
    public SampleWinApp(IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
    }

    protected override Form CreateMainForm() =>
        ServiceProvider.GetRequiredService<SampleView>();
}
