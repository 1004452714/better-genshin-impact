using Microsoft.ClearScript;
using System;
using System.Collections.Generic;

namespace BetterGenshinImpact.Core.Script.Dependence.Model.TimerConfig;

/// <summary>
/// 实时触发任务参数的严格绑定器。
/// <para>只按参数对象声明的白名单键逐个匹配，脚本里多写或拼错的键会立即抛出异常并回显可用参数，
/// 不做任何反射式属性填充。</para>
/// </summary>
public static class RealtimeTriggerParamBinder
{
    /// <summary>
    /// 把调用方配置绑定到参数对象。
    /// </summary>
    /// <param name="param">默认值全为 null 的参数对象</param>
    /// <param name="config">null、<typeparamref name="T"/> 实例，或脚本侧的对象字面量</param>
    /// <exception cref="ArgumentException">存在未知键、值类型错误或配置类型不受支持</exception>
    public static T Bind<T>(T param, object? config) where T : IRealtimeTriggerParam
    {
        switch (config)
        {
            case T typed:
                param = typed;
                break;
            case ScriptObject scriptObject:
                Apply(param, FromScriptObject(scriptObject));
                break;
            case IReadOnlyDictionary<string, object?> readOnlyDictionary:
                Apply(param, readOnlyDictionary);
                break;
            case null:
                break;
            default:
                if (!IsUnspecified(config))
                {
                    throw new ArgumentException(
                        $"配置必须是 {typeof(T).Name} 或对象字面量，实际为 {config.GetType().Name}");
                }

                break;
        }

        param.Validate();
        return param;
    }

    /// <summary>
    /// 取脚本对象上的可枚举键。默认只含自身可枚举属性，不含原型链上的 toString 等非枚举成员。
    /// </summary>
    public static IReadOnlyDictionary<string, object?> FromScriptObject(ScriptObject source)
    {
        var keys = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        foreach (string name in source.PropertyNames)
        {
            keys[name] = source.GetProperty(name);
        }

        return keys;
    }

    private static void Apply<T>(T param, IReadOnlyDictionary<string, object?> source) where T : IRealtimeTriggerParam
    {
        foreach (var entry in source)
        {
            // 先判键名：拼错的参数无论取值为何都要报错，否则依然会静默丢失
            var accepted = CanonicalizeKey(param, entry.Key);
            if (accepted is null)
            {
                throw new ArgumentException(
                    $"未知参数 '{entry.Key}'（{param.TriggerName}）。可用参数：{string.Join(", ", param.AcceptedKeys)}");
            }

            // 显式传 null 视为"不覆盖"，沿用用户已保存的配置
            if (IsUnspecified(entry.Value))
            {
                continue;
            }

            param.TryAssign(accepted, entry.Value);
        }
    }

    private static bool IsUnspecified(object? value)
    {
        return value is null or Undefined || ReferenceEquals(value, Type.Missing);
    }

    private static string? CanonicalizeKey<T>(T param, string key) where T : IRealtimeTriggerParam
    {
        foreach (var accepted in param.AcceptedKeys)
        {
            if (string.Equals(accepted, key, StringComparison.OrdinalIgnoreCase))
            {
                return accepted;
            }
        }

        return null;
    }
}
