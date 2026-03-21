namespace DocumentView.Framework;

/// <summary>
/// Application object equivalent to MFC <c>CWinApp</c>.
/// Creates the main window and runs <see cref="Application.Run(Form)"/>.
/// </summary>
public abstract class MfcWinApp
{
    /// <param name="serviceProvider">DI-built <see cref="IServiceProvider"/> used to resolve the main form.</param>
    protected MfcWinApp(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    /// <summary>Service provider passed from the composition root.</summary>
    protected IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Creates the main window. Derived classes typically resolve it via <c>GetRequiredService&lt;TMainForm&gt;()</c>.
    /// </summary>
    protected abstract Form CreateMainForm();

    /// <summary>Starts the application message loop.</summary>
    public void Run() => Application.Run(CreateMainForm());
}
