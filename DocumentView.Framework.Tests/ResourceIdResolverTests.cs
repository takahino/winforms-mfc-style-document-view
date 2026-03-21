using DocumentView.Framework;
using Xunit;

namespace DocumentView.Framework.Tests;

public class ResourceIdResolverTests
{
    // AutoId style (controlName specified)
    private static class AutoIdSource
    {
        [AutoId("IDC_EDIT_NAME")] public static int IDC_EDIT_NAME;
        [AutoId("IDC_EDIT_AGE")]  public static int IDC_EDIT_AGE;

        static AutoIdSource() => AutoIdAssigner.Assign(typeof(AutoIdSource));
    }

    // const int style (#define in resource.h)
    private static class ConstSource
    {
        public const int IDC_BUTTON1 = 101;
        public const int IDC_BUTTON2 = 102;
    }

    // AutoId without controlName: field name is the control name
    private static class AutoIdNoName
    {
        [AutoId] public static int IDC_LABEL1;

        static AutoIdNoName() => AutoIdAssigner.Assign(typeof(AutoIdNoName));
    }

    // static int fields without [AutoId] or const are skipped
    private static class MixedSource
    {
        [AutoId("IDC_EDIT1")] public static int IDC_EDIT1;
        public static int NotDecorated;  // skipped

        static MixedSource() => AutoIdAssigner.Assign(typeof(MixedSource));
    }

    [Fact]
    public void TryGetName_ReturnsControlName_ForAutoIdField()
    {
        var resolver = new ResourceIdResolver(typeof(AutoIdSource));

        Assert.True(resolver.TryGetName(AutoIdSource.IDC_EDIT_NAME, out var name));
        Assert.Equal("IDC_EDIT_NAME", name);
    }

    [Fact]
    public void TryGetId_ReturnsId_ForControlName()
    {
        var resolver = new ResourceIdResolver(typeof(AutoIdSource));

        Assert.True(resolver.TryGetId("IDC_EDIT_AGE", out var id));
        Assert.Equal(AutoIdSource.IDC_EDIT_AGE, id);
    }

    [Fact]
    public void TryGetName_ReturnsFalse_ForUnknownId()
    {
        var resolver = new ResourceIdResolver(typeof(AutoIdSource));

        Assert.False(resolver.TryGetName(9999, out _));
    }

    [Fact]
    public void TryGetId_ReturnsFalse_ForUnknownName()
    {
        var resolver = new ResourceIdResolver(typeof(AutoIdSource));

        Assert.False(resolver.TryGetId("IDC_NONEXISTENT", out _));
    }

    [Fact]
    public void AllEntries_ReturnsAllRegisteredIds()
    {
        var resolver = new ResourceIdResolver(typeof(AutoIdSource));

        Assert.Equal(2, resolver.AllEntries.Count());
    }

    [Fact]
    public void TryGetName_WorksWithConstIntFields()
    {
        var resolver = new ResourceIdResolver(typeof(ConstSource));

        Assert.True(resolver.TryGetName(101, out var name));
        Assert.Equal("IDC_BUTTON1", name);
    }

    [Fact]
    public void TryGetId_WorksWithConstIntFields()
    {
        var resolver = new ResourceIdResolver(typeof(ConstSource));

        Assert.True(resolver.TryGetId("IDC_BUTTON2", out var id));
        Assert.Equal(102, id);
    }

    [Fact]
    public void AutoId_WithoutControlName_UsesFieldName()
    {
        var resolver = new ResourceIdResolver(typeof(AutoIdNoName));

        Assert.True(resolver.TryGetName(AutoIdNoName.IDC_LABEL1, out var name));
        Assert.Equal("IDC_LABEL1", name);
    }

    [Fact]
    public void NonDecoratedStaticField_IsSkipped()
    {
        var resolver = new ResourceIdResolver(typeof(MixedSource));

        // NotDecorated skipped → only one registered entry
        Assert.Single(resolver.AllEntries);
    }
}
