using DocumentView.Framework;

namespace DocumentView.Sample.Tests;

/// <summary>
/// Test double for <see cref="IMessageBoxService"/>; records calls without showing a dialog.
/// </summary>
internal class FakeMessageBoxService : IMessageBoxService
{
    public record Call(string Text, string Caption, MessageBoxButtons Buttons, MessageBoxIcon Icon);

    public List<Call> Calls { get; } = [];

    /// <summary>Return value for the next <c>Show()</c> (default OK).</summary>
    public DialogResult Result { get; set; } = DialogResult.OK;

    public DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
    {
        Calls.Add(new Call(text, caption, buttons, icon));
        return Result;
    }
}
