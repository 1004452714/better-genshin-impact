using BetterGenshinImpact.Core.Script.Dependence.Model.TimerConfig;
using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;
using System;

namespace BetterGenshinImpact.UnitTest.GameTaskTests.RealtimeTriggerTests;

/// <summary>
/// 用真实 V8 引擎验证脚本对象字面量的绑定路径：可用参数、未知参数、数字装箱与原型链成员。
/// </summary>
public class RealtimeTriggerParamBinderV8Tests : IDisposable
{
    private readonly V8ScriptEngine _engine = new();

    public void Dispose()
    {
        _engine.Dispose();
    }

    [Fact]
    public void Bind_V8Literal_ShouldAssignKnownKey()
    {
        var config = Evaluate("({ forceInteraction: true })");

        var result = RealtimeTriggerParamBinder.Bind(new AutoPickTriggerParam(), config);

        Assert.True(result.ForceInteraction);
    }

    [Fact]
    public void Bind_V8LiteralWithTypoKey_ShouldThrow()
    {
        var config = Evaluate("({ forcePickAll: true })");

        var exception = Assert.Throws<ArgumentException>(() => RealtimeTriggerParamBinder.Bind(new AutoPickTriggerParam(), config));

        Assert.Contains("forcePickAll", exception.Message);
    }

    [Fact]
    public void Bind_V8Number_ShouldBindAsInt()
    {
        var config = Evaluate("({ checkInterval: 120, eatInterval: 2500 })");

        var result = RealtimeTriggerParamBinder.Bind(new AutoEatTriggerParam(), config);

        Assert.Equal(120, result.CheckInterval);
        Assert.Equal(2500, result.EatInterval);
    }

    [Fact]
    public void Bind_V8ClassInstance_ShouldIgnorePrototypeMethods()
    {
        // 原型链上的方法不属于自有可枚举属性，不应被当成未知参数
        _engine.Execute("class Box { constructor() { this.forceInteraction = true } describe() { return 1 } }");
        var config = Evaluate("new Box()");

        var result = RealtimeTriggerParamBinder.Bind(new AutoPickTriggerParam(), config);

        Assert.True(result.ForceInteraction);
    }

    [Fact]
    public void Bind_V8EmptyObject_ShouldKeepSavedDefaults()
    {
        var config = Evaluate("({})");

        var result = RealtimeTriggerParamBinder.Bind(new AutoSkipTriggerParam(), config);

        Assert.Null(result.ClickChatOption);
        Assert.Null(result.AutoHangoutEventEnabled);
    }

    private ScriptObject Evaluate(string code)
    {
        return (ScriptObject)_engine.Evaluate("probe", code)!;
    }
}
