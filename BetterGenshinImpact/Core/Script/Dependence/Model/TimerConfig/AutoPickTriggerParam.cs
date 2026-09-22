using System.Collections.Generic;

namespace BetterGenshinImpact.Core.Script.Dependence.Model.TimerConfig;

/// <summary>
/// 自动拾取触发器参数。
/// <para>拾取名单、识别偏移等仍完全来自用户在设置页保存的 AutoPickConfig，此处只提供脚本专用的临时开关。</para>
/// </summary>
public class AutoPickTriggerParam : IRealtimeTriggerParam
{
    /// <summary>
    /// 无视文字与图标识别结果，遇到交互键直接拾取。null 表示不覆盖，按未开启处理。
    /// </summary>
    public bool? ForceInteraction { get; set; }

    public string TriggerName => "AutoPick";

    private static readonly string[] Keys = ["forceInteraction"];

    public IReadOnlyList<string> AcceptedKeys => Keys;

    public bool TryAssign(string key, object? value)
    {
        switch (key)
        {
            case "forceInteraction":
                ForceInteraction = RealtimeTriggerParamValue.AsBool(key, TriggerName, value);
                return true;
            default:
                return false;
        }
    }

    public void Validate()
    {
    }
}
