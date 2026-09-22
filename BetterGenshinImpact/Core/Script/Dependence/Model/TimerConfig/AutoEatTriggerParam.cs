using System.Collections.Generic;

namespace BetterGenshinImpact.Core.Script.Dependence.Model.TimerConfig;

/// <summary>
/// 自动吃药触发器参数。
/// <para>仅包含 AutoEatTrigger 在截图循环里真正消费的字段。</para>
/// </summary>
public class AutoEatTriggerParam : IRealtimeTriggerParam
{
    /// <summary>
    /// 检测间隔（毫秒），最小 16。
    /// </summary>
    public int? CheckInterval { get; set; }

    /// <summary>
    /// 两次吃药的最小间隔（毫秒），最小 100。
    /// </summary>
    public int? EatInterval { get; set; }

    public string TriggerName => "AutoEat";

    private static readonly string[] Keys = ["checkInterval", "eatInterval"];

    public IReadOnlyList<string> AcceptedKeys => Keys;

    public bool TryAssign(string key, object? value)
    {
        switch (key)
        {
            case "checkInterval":
                CheckInterval = RealtimeTriggerParamValue.AsInt(key, TriggerName, value);
                return true;
            case "eatInterval":
                EatInterval = RealtimeTriggerParamValue.AsInt(key, TriggerName, value);
                return true;
            default:
                return false;
        }
    }

    public void Validate()
    {
        RealtimeTriggerParamValue.CheckRange("checkInterval", TriggerName, CheckInterval, 16, 60000);
        RealtimeTriggerParamValue.CheckRange("eatInterval", TriggerName, EatInterval, 100, 600000);
    }
}
