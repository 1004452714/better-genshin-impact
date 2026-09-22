using BetterGenshinImpact.GameTask.AutoSkip;
using System;
using System.Collections.Generic;

namespace BetterGenshinImpact.Core.Script.Dependence.Model.TimerConfig;

/// <summary>
/// 自动剧情触发器参数，自动邀约相关开关同属本触发器。
/// <para>仅包含 AutoSkipTrigger 在截图循环里真正消费的字段；Enabled 不在此列，脚本添加的触发器会被强制启用，
/// 关闭请改用对应的移除方法。</para>
/// </summary>
public class AutoSkipTriggerParam : IRealtimeTriggerParam
{
    /// <summary>
    /// 快速跳过对话
    /// </summary>
    public bool? QuicklySkipConversationsEnabled { get; set; }

    /// <summary>
    /// 选项策略，取值见 AutoSkipConfig.ClickChatOptionValues
    /// </summary>
    public string? ClickChatOption { get; set; }

    /// <summary>
    /// 选择选项后的延迟（毫秒），0~60000
    /// </summary>
    public int? AfterChooseOptionSleepDelay { get; set; }

    /// <summary>
    /// 点击对话框前的延迟（毫秒），0~60000
    /// </summary>
    public int? BeforeClickConfirmDelay { get; set; }

    /// <summary>
    /// 选择剧情选项前等待语音播报结束
    /// </summary>
    public bool? AutoWaitDialogueOptionVoiceEnabled { get; set; }

    /// <summary>
    /// 等待语音结束的上限（秒），0~600
    /// </summary>
    public int? DialogueOptionVoiceMaxWaitSeconds { get; set; }

    /// <summary>
    /// 后台剧情结束后切回游戏前台
    /// </summary>
    public bool? BringGameToFrontAfterBackgroundDialogEnabled { get; set; }

    /// <summary>
    /// 自定义优先选项启用
    /// </summary>
    public bool? CustomPriorityOptionsEnabled { get; set; }

    /// <summary>
    /// 自定义优先选项文本，每行一个或用分号分隔
    /// </summary>
    public string? CustomPriorityOptions { get; set; }

    /// <summary>
    /// 提交物品
    /// </summary>
    public bool? SubmitGoodsEnabled { get; set; }

    /// <summary>
    /// 关闭弹出层
    /// </summary>
    public bool? ClosePopupPagedEnabled { get; set; }

    /// <summary>
    /// 游戏失焦时后台运行
    /// </summary>
    public bool? RunBackgroundEnabled { get; set; }

    /// <summary>
    /// 跳过内置默认点击选项，由脚本自行处理选项
    /// </summary>
    public bool? SkipBuiltInClickOptions { get; set; }

    /// <summary>
    /// 自动领取每日委托奖励
    /// </summary>
    public bool? AutoGetDailyRewardsEnabled { get; set; }

    /// <summary>
    /// 自动重新派遣
    /// </summary>
    public bool? AutoReExploreEnabled { get; set; }

    /// <summary>
    /// 自动邀约启用
    /// </summary>
    public bool? AutoHangoutEventEnabled { get; set; }

    /// <summary>
    /// 自动邀约分支，取值必须是 hangout.json 中的分支名；非法时记日志并回退默认选择逻辑
    /// </summary>
    public string? AutoHangoutEndChoose { get; set; }

    /// <summary>
    /// 自动邀约选择选项前的延迟（毫秒），0~60000
    /// </summary>
    public int? AutoHangoutChooseOptionSleepDelay { get; set; }

    /// <summary>
    /// 自动邀约自动点击跳过按钮
    /// </summary>
    public bool? AutoHangoutPressSkipEnabled { get; set; }

    public string TriggerName => "AutoSkip";

    private static readonly string[] Keys =
    [
        "quicklySkipConversationsEnabled",
        "clickChatOption",
        "afterChooseOptionSleepDelay",
        "beforeClickConfirmDelay",
        "autoWaitDialogueOptionVoiceEnabled",
        "dialogueOptionVoiceMaxWaitSeconds",
        "bringGameToFrontAfterBackgroundDialogEnabled",
        "customPriorityOptionsEnabled",
        "customPriorityOptions",
        "submitGoodsEnabled",
        "closePopupPagedEnabled",
        "runBackgroundEnabled",
        "skipBuiltInClickOptions",
        "autoGetDailyRewardsEnabled",
        "autoReExploreEnabled",
        "autoHangoutEventEnabled",
        "autoHangoutEndChoose",
        "autoHangoutChooseOptionSleepDelay",
        "autoHangoutPressSkipEnabled"
    ];

    public IReadOnlyList<string> AcceptedKeys => Keys;

    public bool TryAssign(string key, object? value)
    {
        switch (key)
        {
            case "quicklySkipConversationsEnabled":
                QuicklySkipConversationsEnabled = RealtimeTriggerParamValue.AsBool(key, TriggerName, value);
                return true;
            case "clickChatOption":
                ClickChatOption = RealtimeTriggerParamValue.AsString(key, TriggerName, value);
                return true;
            case "afterChooseOptionSleepDelay":
                AfterChooseOptionSleepDelay = RealtimeTriggerParamValue.AsInt(key, TriggerName, value);
                return true;
            case "beforeClickConfirmDelay":
                BeforeClickConfirmDelay = RealtimeTriggerParamValue.AsInt(key, TriggerName, value);
                return true;
            case "autoWaitDialogueOptionVoiceEnabled":
                AutoWaitDialogueOptionVoiceEnabled = RealtimeTriggerParamValue.AsBool(key, TriggerName, value);
                return true;
            case "dialogueOptionVoiceMaxWaitSeconds":
                DialogueOptionVoiceMaxWaitSeconds = RealtimeTriggerParamValue.AsInt(key, TriggerName, value);
                return true;
            case "bringGameToFrontAfterBackgroundDialogEnabled":
                BringGameToFrontAfterBackgroundDialogEnabled = RealtimeTriggerParamValue.AsBool(key, TriggerName, value);
                return true;
            case "customPriorityOptionsEnabled":
                CustomPriorityOptionsEnabled = RealtimeTriggerParamValue.AsBool(key, TriggerName, value);
                return true;
            case "customPriorityOptions":
                CustomPriorityOptions = RealtimeTriggerParamValue.AsString(key, TriggerName, value);
                return true;
            case "submitGoodsEnabled":
                SubmitGoodsEnabled = RealtimeTriggerParamValue.AsBool(key, TriggerName, value);
                return true;
            case "closePopupPagedEnabled":
                ClosePopupPagedEnabled = RealtimeTriggerParamValue.AsBool(key, TriggerName, value);
                return true;
            case "runBackgroundEnabled":
                RunBackgroundEnabled = RealtimeTriggerParamValue.AsBool(key, TriggerName, value);
                return true;
            case "skipBuiltInClickOptions":
                SkipBuiltInClickOptions = RealtimeTriggerParamValue.AsBool(key, TriggerName, value);
                return true;
            case "autoGetDailyRewardsEnabled":
                AutoGetDailyRewardsEnabled = RealtimeTriggerParamValue.AsBool(key, TriggerName, value);
                return true;
            case "autoReExploreEnabled":
                AutoReExploreEnabled = RealtimeTriggerParamValue.AsBool(key, TriggerName, value);
                return true;
            case "autoHangoutEventEnabled":
                AutoHangoutEventEnabled = RealtimeTriggerParamValue.AsBool(key, TriggerName, value);
                return true;
            case "autoHangoutEndChoose":
                AutoHangoutEndChoose = RealtimeTriggerParamValue.AsString(key, TriggerName, value);
                return true;
            case "autoHangoutChooseOptionSleepDelay":
                AutoHangoutChooseOptionSleepDelay = RealtimeTriggerParamValue.AsInt(key, TriggerName, value);
                return true;
            case "autoHangoutPressSkipEnabled":
                AutoHangoutPressSkipEnabled = RealtimeTriggerParamValue.AsBool(key, TriggerName, value);
                return true;
            default:
                return false;
        }
    }

    public void Validate()
    {
        RealtimeTriggerParamValue.CheckRange("afterChooseOptionSleepDelay", TriggerName, AfterChooseOptionSleepDelay, 0, 60000);
        RealtimeTriggerParamValue.CheckRange("beforeClickConfirmDelay", TriggerName, BeforeClickConfirmDelay, 0, 60000);
        RealtimeTriggerParamValue.CheckRange("dialogueOptionVoiceMaxWaitSeconds", TriggerName, DialogueOptionVoiceMaxWaitSeconds, 0, 600);
        RealtimeTriggerParamValue.CheckRange("autoHangoutChooseOptionSleepDelay", TriggerName, AutoHangoutChooseOptionSleepDelay, 0, 60000);

        if (ClickChatOption is not null && !Array.Exists(AutoSkipConfig.ClickChatOptionValues, o => o == ClickChatOption))
        {
            throw new ArgumentException(
                $"参数 'clickChatOption'（{TriggerName}）取值 '{ClickChatOption}' 无效，可选值：{string.Join("、", AutoSkipConfig.ClickChatOptionValues)}");
        }
    }
}
