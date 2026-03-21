using DocumentView.Framework;
using Xunit;

namespace DocumentView.Framework.Tests;

public class AutoIdAssignerTests
{
    // static class with [AutoId] fields (no static ctor)
    private static class ThreeFields
    {
        [AutoId] public static int A;
        [AutoId] public static int B;
        [AutoId] public static int C;
    }

    private static class CustomStart
    {
        [AutoId] public static int X;
        [AutoId] public static int Y;
    }

    private static class NoAutoId
    {
        public static int NotTagged;
    }

    private static class MixedFields
    {
        [AutoId] public static int First;
        public static int SkipMe;   // no [AutoId] → not numbered
        [AutoId] public static int Second;
    }

    [Fact]
    public void Assign_SequentialIds_StartAt1()
    {
        AutoIdAssigner.Assign(typeof(ThreeFields));

        Assert.Equal(1, ThreeFields.A);
        Assert.Equal(2, ThreeFields.B);
        Assert.Equal(3, ThreeFields.C);
    }

    [Fact]
    public void Assign_CustomStartId_StartsAtSpecifiedValue()
    {
        AutoIdAssigner.Assign(typeof(CustomStart), startId: 100);

        Assert.Equal(100, CustomStart.X);
        Assert.Equal(101, CustomStart.Y);
    }

    [Fact]
    public void Assign_SkipsFields_WithoutAutoIdAttribute()
    {
        NoAutoId.NotTagged = 999;
        AutoIdAssigner.Assign(typeof(NoAutoId));

        Assert.Equal(999, NoAutoId.NotTagged); // unchanged
    }

    [Fact]
    public void Assign_SkipsNonAutoIdFields_InDeclarationOrder()
    {
        AutoIdAssigner.Assign(typeof(MixedFields));

        Assert.Equal(1, MixedFields.First);
        Assert.Equal(0, MixedFields.SkipMe); // not numbered (stays 0)
        Assert.Equal(2, MixedFields.Second);
    }
}
