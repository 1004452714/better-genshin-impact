using BetterGenshinImpact.GameTask.AutoEat;
using BetterGenshinImpact.GameTask.AutoSkip;
using BetterGenshinImpact.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace BetterGenshinImpact.Core.Script.Dependence.Model.TimerConfig;

/// <summary>
/// 把类型化参数叠加到用户已保存的配置上。
/// <para>入参配置一律先深拷贝再覆盖，返回新实例，绝不修改 <c>TaskContext.Instance().Config</c> 中的对象。</para>
/// </summary>
public static class RealtimeTriggerConfigMerger
{
    /// <summary>
    /// 复用配置落盘所用的序列化选项，保证拷贝语义与持久化语义一致。
    /// </summary>
    public static T DeepClone<T>(T source) where T : class
    {
        return JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(source, ConfigService.JsonOptions),
            ConfigService.JsonOptions)!;
    }

    /// <summary>
    /// 合并自动拾取参数。拾取名单等仍由触发器自行读取全局配置，此处只产出脚本侧的临时开关。
    /// </summary>
    public static AutoPickExternalConfig Merge(AutoPickTriggerParam param)
    {
        return new AutoPickExternalConfig
        {
            ForceInteraction = param.ForceInteraction ?? false
        };
    }

    public static AutoEatConfig Merge(AutoEatConfig saved, AutoEatTriggerParam param)
    {
        var target = DeepClone(saved);
        if (param.CheckInterval is { } checkInterval)
        {
            target.CheckInterval = checkInterval;
        }

        if (param.EatInterval is { } eatInterval)
        {
            target.EatInterval = eatInterval;
        }

        return target;
    }

    /// <param name="validHangoutBranches">hangout.json 中的分支名集合；为 null 时跳过分支名校验</param>
    /// <param name="onHangoutBranchFallback">分支名非法时回调，传入被丢弃的分支名；分支名非法不报错，只回退默认选择逻辑</param>
    public static AutoSkipConfig Merge(AutoSkipConfig saved, AutoSkipTriggerParam param,
        IReadOnlyCollection<string>? validHangoutBranches = null, Action<string>? onHangoutBranchFallback = null)
    {
        var target = DeepClone(saved);

        if (param.QuicklySkipConversationsEnabled is { } quicklySkip)
        {
            target.QuicklySkipConversationsEnabled = quicklySkip;
        }

        if (param.ClickChatOption is { } clickChatOption)
        {
            target.ClickChatOption = clickChatOption;
        }

        if (param.AfterChooseOptionSleepDelay is { } afterChooseDelay)
        {
            target.AfterChooseOptionSleepDelay = afterChooseDelay;
        }

        if (param.BeforeClickConfirmDelay is { } beforeClickDelay)
        {
            target.BeforeClickConfirmDelay = beforeClickDelay;
        }

        if (param.AutoWaitDialogueOptionVoiceEnabled is { } autoWaitVoice)
        {
            target.AutoWaitDialogueOptionVoiceEnabled = autoWaitVoice;
        }

        if (param.DialogueOptionVoiceMaxWaitSeconds is { } maxWaitSeconds)
        {
            target.DialogueOptionVoiceMaxWaitSeconds = maxWaitSeconds;
        }

        if (param.BringGameToFrontAfterBackgroundDialogEnabled is { } bringToFront)
        {
            target.BringGameToFrontAfterBackgroundDialogEnabled = bringToFront;
        }

        if (param.CustomPriorityOptionsEnabled is { } customPriorityEnabled)
        {
            target.CustomPriorityOptionsEnabled = customPriorityEnabled;
        }

        if (param.CustomPriorityOptions is { } customPriorityOptions)
        {
            target.CustomPriorityOptions = customPriorityOptions;
        }

        if (param.SubmitGoodsEnabled is { } submitGoods)
        {
            target.SubmitGoodsEnabled = submitGoods;
        }

        if (param.ClosePopupPagedEnabled is { } closePopup)
        {
            target.ClosePopupPagedEnabled = closePopup;
        }

        if (param.RunBackgroundEnabled is { } runBackground)
        {
            target.RunBackgroundEnabled = runBackground;
        }

        if (param.SkipBuiltInClickOptions is { } skipBuiltIn)
        {
            target.SkipBuiltInClickOptions = skipBuiltIn;
        }

        if (param.AutoGetDailyRewardsEnabled is { } dailyRewards)
        {
            target.AutoGetDailyRewardsEnabled = dailyRewards;
        }

        if (param.AutoReExploreEnabled is { } reExplore)
        {
            target.AutoReExploreEnabled = reExplore;
        }

        if (param.AutoHangoutEventEnabled is { } hangoutEnabled)
        {
            target.AutoHangoutEventEnabled = hangoutEnabled;
        }

        if (param.AutoHangoutPressSkipEnabled is { } hangoutSkip)
        {
            target.AutoHangoutPressSkipEnabled = hangoutSkip;
        }

        if (param.AutoHangoutChooseOptionSleepDelay is { } hangoutDelay)
        {
            target.AutoHangoutChooseOptionSleepDelay = hangoutDelay;
        }

        if (param.AutoHangoutEndChoose is { Length: > 0 } hangoutBranch)
        {
            if (ResolveHangoutBranch(hangoutBranch, validHangoutBranches, out var invalidBranch))
            {
                target.AutoHangoutEndChoose = hangoutBranch;
            }
            else
            {
                target.AutoHangoutEndChoose = string.Empty;
                onHangoutBranchFallback?.Invoke(hangoutBranch);
            }
        }

        return target;
    }

    /// <summary>
    /// 校验邀约分支名。非法分支在当前实现里会被静默忽略并走默认选肢逻辑，因此这里只判定，不抛异常。
    /// </summary>
    /// <returns>分支名可用（含无需校验）时返回 true</returns>
    public static bool ResolveHangoutBranch(string? branch, IReadOnlyCollection<string>? validBranches, out bool invalid)
    {
        invalid = false;
        if (string.IsNullOrEmpty(branch) || validBranches is null || validBranches.Count == 0)
        {
            return true;
        }

        if (validBranches.Contains(branch))
        {
            return true;
        }

        invalid = true;
        return false;
    }
}
