using BetterGenshinImpact.Core.Script.Dependence.Model.TimerConfig;
using BetterGenshinImpact.GameTask.AutoEat;
using BetterGenshinImpact.GameTask.AutoSkip;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BetterGenshinImpact.UnitTest.GameTaskTests.RealtimeTriggerTests;

/// <summary>
/// 实时触发任务配置叠加器的功能、边界与异常路径测试
/// </summary>
public class RealtimeTriggerConfigMergerTests
{
    [Fact]
    public void MergeAutoPick_Unspecified_ShouldKeepForceInteractionOff()
    {
        var merged = RealtimeTriggerConfigMerger.Merge(new AutoPickTriggerParam());

        Assert.False(merged.ForceInteraction);
    }

    [Fact]
    public void MergeAutoPick_Specified_ShouldOverride()
    {
        var merged = RealtimeTriggerConfigMerger.Merge(new AutoPickTriggerParam { ForceInteraction = true });

        Assert.True(merged.ForceInteraction);
    }

    [Fact]
    public void MergeAutoEat_OnlyGivenFieldsShouldChange()
    {
        var saved = new AutoEatConfig { CheckInterval = 150, EatInterval = 1000 };

        var merged = RealtimeTriggerConfigMerger.Merge(saved, new AutoEatTriggerParam { CheckInterval = 200 });

        Assert.Equal(200, merged.CheckInterval);
        Assert.Equal(1000, merged.EatInterval);
        // 关键：不得改写用户已保存的配置实例
        Assert.Equal(150, saved.CheckInterval);
    }

    [Fact]
    public void MergeAutoSkip_ShouldNotMutateSavedConfig()
    {
        var saved = new AutoSkipConfig { ClickChatOption = "随机选择选项" };

        var merged = RealtimeTriggerConfigMerger.Merge(saved,
            new AutoSkipTriggerParam { ClickChatOption = "不选择选项", AutoHangoutEventEnabled = true });

        Assert.NotSame(saved, merged);
        Assert.Equal("不选择选项", merged.ClickChatOption);
        Assert.True(merged.AutoHangoutEventEnabled);
        Assert.Equal("随机选择选项", saved.ClickChatOption);
    }

    [Fact]
    public void MergeAutoSkip_EmptyParam_ShouldPreserveEveryField()
    {
        var saved = new AutoSkipConfig
        {
            QuicklySkipConversationsEnabled = false,
            AfterChooseOptionSleepDelay = 321,
            CustomPriorityOptions = "左边；右边",
            AutoHangoutEndChoose = "琳妮特结局1:大成功!",
            PictureInPictureSourceType = "TriggerDispatcher",
            Enabled = false,
            AutoWaitDialogueOptionVoiceEnabled = true,
            DialogueOptionVoiceMaxWaitSeconds = 44,
            SkipBuiltInClickOptions = true,
            AutoHangoutChooseOptionSleepDelay = 77,
        };
#pragma warning disable CS0618 // 已过时的字段：用于确认深拷贝不会漏掉任何公开属性
        saved.AutoReExploreCharacter = "旧版角色名";
#pragma warning restore CS0618

        var merged = RealtimeTriggerConfigMerger.Merge(saved, new AutoSkipTriggerParam());

        // 深拷贝漏字段会让脚本静默丢失用户设置，这里逐属性比对兜底
        var mismatched = typeof(AutoSkipConfig)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && p.GetIndexParameters().Length == 0)
            .Where(p => !Equals(p.GetValue(saved), p.GetValue(merged)))
            .Select(p => p.Name)
            .ToList();

        Assert.Empty(mismatched);
    }

    [Fact]
    public void MergeAutoSkip_UnknownHangoutBranch_ShouldFallBackAndNotify()
    {
        var saved = new AutoSkipConfig();
        var param = new AutoSkipTriggerParam { AutoHangoutEventEnabled = true, AutoHangoutEndChoose = "不存在的分支" };
        var reported = new List<string>();

        var merged = RealtimeTriggerConfigMerger.Merge(saved, param, ["有效分支"], reported.Add);

        Assert.True(merged.AutoHangoutEventEnabled);
        Assert.Equal(string.Empty, merged.AutoHangoutEndChoose);
        Assert.Single(reported, "不存在的分支");
    }

    [Fact]
    public void MergeAutoSkip_KnownHangoutBranch_ShouldApply()
    {
        var param = new AutoSkipTriggerParam { AutoHangoutEndChoose = "有效分支" };

        var merged = RealtimeTriggerConfigMerger.Merge(new AutoSkipConfig(), param, ["有效分支"]);

        Assert.Equal("有效分支", merged.AutoHangoutEndChoose);
    }

    [Fact]
    public void MergeAutoSkip_WithoutBranchSource_ShouldSkipValidation()
    {
        var param = new AutoSkipTriggerParam { AutoHangoutEndChoose = "任意写法" };

        var merged = RealtimeTriggerConfigMerger.Merge(new AutoSkipConfig(), param);

        Assert.Equal("任意写法", merged.AutoHangoutEndChoose);
    }

    [Fact]
    public void MergeAutoSkip_EmptyBranch_ShouldKeepSavedValue()
    {
        var saved = new AutoSkipConfig { AutoHangoutEndChoose = "用户已保存分支" };

        var merged = RealtimeTriggerConfigMerger.Merge(saved, new AutoSkipTriggerParam { AutoHangoutEndChoose = "" },
            ["有效分支"]);

        Assert.Equal("用户已保存分支", merged.AutoHangoutEndChoose);
    }
}
