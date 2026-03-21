using DocumentView.Sample;
using Xunit;

namespace DocumentView.Sample.Tests;

public class ResourceIdTests
{
    [Fact]
    public void ResourceId_AutoIds_AreSequentialStartingAt1()
    {
        // static ctor already ran AutoIdAssigner.Assign()
        Assert.Equal(1,  ResourceId.IDC_EDIT_NAME);
        Assert.Equal(2,  ResourceId.IDC_EDIT_AGE);
        Assert.Equal(3,  ResourceId.IDC_EDIT_MEMO);
        Assert.Equal(4,  ResourceId.IDC_CHECK_ACTIVE);
        Assert.Equal(5,  ResourceId.IDC_CHECK_NOTIFY);
        Assert.Equal(6,  ResourceId.IDC_COMBO_PREF);
        Assert.Equal(7,  ResourceId.IDC_COMBO_LANG);
        Assert.Equal(8,  ResourceId.IDC_SPIN_PRIORITY);
        Assert.Equal(9,  ResourceId.IDC_RADIO_MALE);
        Assert.Equal(10, ResourceId.IDC_RADIO_FEMALE);
        Assert.Equal(11, ResourceId.IDC_LIST_ITEMS);
        Assert.Equal(12, ResourceId.IDC_STATIC_STATUS);
        Assert.Equal(13, ResourceId.IDC_GRID_ITEMS);
        Assert.Equal(14, ResourceId.IDC_EDIT_EMPNO);
    }

    [Fact]
    public void ResourceId_AllIds_AreUnique()
    {
        var ids = new[]
        {
            ResourceId.IDC_EDIT_NAME,    ResourceId.IDC_EDIT_AGE,
            ResourceId.IDC_EDIT_MEMO,    ResourceId.IDC_CHECK_ACTIVE,
            ResourceId.IDC_CHECK_NOTIFY, ResourceId.IDC_COMBO_PREF,
            ResourceId.IDC_COMBO_LANG,   ResourceId.IDC_SPIN_PRIORITY,
            ResourceId.IDC_RADIO_MALE,   ResourceId.IDC_RADIO_FEMALE,
            ResourceId.IDC_LIST_ITEMS,   ResourceId.IDC_STATIC_STATUS,
            ResourceId.IDC_GRID_ITEMS,   ResourceId.IDC_EDIT_EMPNO,
        };

        Assert.Equal(ids.Length, ids.Distinct().Count());
    }
}
