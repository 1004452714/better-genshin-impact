using System;

namespace BetterGenshinImpact.Core.Script.Dependence.Model.TimerConfig;

/// <summary>
/// 把调用方传来的原始值收敛到参数声明的类型，失败时抛出指明参数名的异常。
/// <para>脚本侧的整数可能以 double 抵达，因此数值需要显式归一化。</para>
/// </summary>
public static class RealtimeTriggerParamValue
{
    public static bool AsBool(string key, string owner, object? value)
    {
        if (value is bool b)
        {
            return b;
        }

        throw TypeMismatch(key, owner, "布尔值(true/false)", value);
    }

    public static int AsInt(string key, string owner, object? value)
    {
        switch (value)
        {
            case int i:
                return i;
            case long l when l is >= int.MinValue and <= int.MaxValue:
                return (int)l;
            case double d when d == Math.Floor(d) && d is >= int.MinValue and <= int.MaxValue:
                return (int)d;
            default:
                throw TypeMismatch(key, owner, "整数", value);
        }
    }

    public static string AsString(string key, string owner, object? value)
    {
        if (value is string s)
        {
            return s;
        }

        throw TypeMismatch(key, owner, "字符串", value);
    }

    /// <summary>
    /// 越界即抛出，异常消息中包含参数名与允许范围。
    /// </summary>
    public static void CheckRange(string key, string owner, int? value, int min, int max)
    {
        if (value is null)
        {
            return;
        }

        if (value.Value < min || value.Value > max)
        {
            throw new ArgumentOutOfRangeException(key, value,
                $"参数 '{key}'（{owner}）取值 {value.Value} 超出允许范围 [{min}, {max}]");
        }
    }

    private static ArgumentException TypeMismatch(string key, string owner, string expected, object? value)
    {
        var actual = value?.GetType().Name ?? "null";
        return new ArgumentException($"参数 '{key}'（{owner}）类型错误，应为 {expected}，实际为 {actual}");
    }
}
