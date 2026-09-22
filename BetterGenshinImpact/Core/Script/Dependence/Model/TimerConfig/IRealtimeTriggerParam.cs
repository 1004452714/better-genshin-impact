using System.Collections.Generic;

namespace BetterGenshinImpact.Core.Script.Dependence.Model.TimerConfig;

/// <summary>
/// 实时触发任务的类型化参数。
/// <para>字段为 null 表示"沿用用户在设置页保存的配置"，赋值即覆盖。叠加不会改写用户已保存的配置实例。</para>
/// <para>截图循环的帧间隔是全局配置（AllConfig.TriggerInterval），因此参数中不提供 Interval。</para>
/// </summary>
public interface IRealtimeTriggerParam
{
    /// <summary>
    /// 目标触发器在 GameTaskManager.TriggerDictionary 中的名称
    /// </summary>
    string TriggerName { get; }

    /// <summary>
    /// 可接受的 camelCase 参数名，用于在参数拼错时把可用列表回显给调用方
    /// </summary>
    IReadOnlyList<string> AcceptedKeys { get; }

    /// <summary>
    /// 按 camelCase 键名赋值。
    /// </summary>
    /// <returns>键名不受支持时返回 false（由调用方汇总为异常）；键名受支持但值类型错误时直接抛出 ArgumentException。</returns>
    bool TryAssign(string key, object? value);

    /// <summary>
    /// 校验取值范围与枚举合法性，不合法时抛出带参数名的异常。
    /// </summary>
    void Validate();
}
