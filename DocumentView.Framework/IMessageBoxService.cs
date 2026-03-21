namespace DocumentView.Framework;

/// <summary>
/// Abstraction over MessageBox.Show for testability (mock injection).
/// </summary>
public interface IMessageBoxService
{
    DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon);

    // Short overloads (default implementation)
    DialogResult Show(string text) =>
        Show(text, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.None);

    DialogResult Show(string text, string caption) =>
        Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.None);
}

/// <summary>
/// Production <see cref="IMessageBoxService"/> that forwards to MessageBox.Show.
/// </summary>
public class MessageBoxService : IMessageBoxService
{
    public DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        => MessageBox.Show(text, caption, buttons, icon);
}
