using System.Globalization;

namespace DocumentView.Framework;

// ── Base attribute ────────────────────────────────────────────────────────

/// <summary>
/// Base class for MFC-style DDV_* validation attributes.
/// Apply alongside [DDX] on Document m_* fields.
///
/// MFC:  DDV_MinMaxInt(pDX, m_nAge, 0, 150);
/// C#:   [DDVMinMax(0, 150)]
///       public int m_nAge = 0;
/// </summary>
[AttributeUsage(
    AttributeTargets.Field | AttributeTargets.Property,
    AllowMultiple = true,
    Inherited = true)]
public abstract class DDVAttribute : Attribute
{
    /// <summary>Custom error message; when null, each DDV supplies a default.</summary>
    public string? Message { get; init; }

    /// <param name="value">Current value to validate (document field value).</param>
    /// <param name="errorMessage">Error text on failure; empty on success.</param>
    /// <returns>true if validation succeeded.</returns>
    public abstract bool Validate(object? value, out string errorMessage);
}

// ── DDV_MinMaxInt / DDV_MinMaxLong / DDV_MinMaxDouble equivalent ───────────

/// <summary>
/// Numeric range validation for int / long / double / decimal / float.
///
/// MFC:  DDV_MinMaxInt(pDX, m_nAge, 0, 150);
/// C#:   [DDVMinMax(0, 150)]
///
/// MFC:  DDV_MinMaxDouble(pDX, m_dRate, 0.0, 1.0);
/// C#:   [DDVMinMax(0.0, 1.0)]
/// </summary>
public sealed class DDVMinMaxAttribute : DDVAttribute
{
    public double Min { get; }
    public double Max { get; }

    public DDVMinMaxAttribute(double min, double max) { Min = min; Max = max; }

    public override bool Validate(object? value, out string errorMessage)
    {
        double d;
        try { d = Convert.ToDouble(value, CultureInfo.InvariantCulture); }
        catch
        {
            // Cannot convert type — skip validation
            errorMessage = string.Empty;
            return true;
        }

        if (d < Min || d > Max)
        {
            errorMessage = Message ?? $"Please enter a value between {FormatNumber(Min)} and {FormatNumber(Max)}.";
            return false;
        }
        errorMessage = string.Empty;
        return true;
    }

    private static string FormatNumber(double v)
        => v == Math.Floor(v) && !double.IsInfinity(v)
            ? ((long)v).ToString(CultureInfo.InvariantCulture)
            : v.ToString(CultureInfo.InvariantCulture);
}

// ── DDV_MaxChars equivalent ────────────────────────────────────────────────

/// <summary>
/// String length validation (maximum characters).
///
/// MFC:  DDV_MaxChars(pDX, m_strName, 50);
/// C#:   [DDVMaxChars(50)]
/// </summary>
public sealed class DDVMaxCharsAttribute : DDVAttribute
{
    public int MaxLength { get; }

    public DDVMaxCharsAttribute(int maxLength) { MaxLength = maxLength; }

    public override bool Validate(object? value, out string errorMessage)
    {
        var s = value?.ToString() ?? string.Empty;
        if (s.Length > MaxLength)
        {
            errorMessage = Message ?? $"Enter at most {MaxLength} characters (currently {s.Length}).";
            return false;
        }
        errorMessage = string.Empty;
        return true;
    }
}
