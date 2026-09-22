using BetterGenshinImpact.Core.Script.Dependence.Model.TimerConfig;
using System;
using System.Collections.Generic;

namespace BetterGenshinImpact.UnitTest.GameTaskTests.RealtimeTriggerTests;

/// <summary>
/// 实时触发任务参数绑定器的功能、边界与异常路径测试
/// </summary>
public class RealtimeTriggerParamBinderTests
{
    [Fact]
    public void Bind_UnknownKey_ShouldThrowWithAcceptedKeyList()
    {
        var param = new AutoPickTriggerParam();
        var config = new Dictionary<string, object?> { ["forcePickAll"] = true };

        var exception = Assert.Throws<ArgumentException>(() => RealtimeTriggerParamBinder.Bind(param, config));

        Assert.Contains("forcePickAll", exception.Message);
        Assert.Contains("forceInteraction", exception.Message);
    }

    [Fact]
    public void Bind_KeyInAnyCase_ShouldStillAssign()
    {
        var param = new AutoEatTriggerParam();
        // 与 ClearScript 宿主成员绑定一致：仅大小写不同的键仍视为同一参数
        var config = new Dictionary<string, object?> { ["CHECKINTERVAL"] = 200 };

        var result = RealtimeTriggerParamBinder.Bind(param, config);

        Assert.Equal(200, result.CheckInterval);
    }

    [Fact]
    public void Bind_ExplicitNull_ShouldKeepFieldUnset()
    {
        var param = new AutoEatTriggerParam();
        var config = new Dictionary<string, object?> { ["checkInterval"] = null };

        var result = RealtimeTriggerParamBinder.Bind(param, config);

        Assert.Null(result.CheckInterval);
    }

    [Fact]
    public void Bind_UnknownKeyWithNullValue_ShouldStillThrow()
    {
        var param = new AutoSkipTriggerParam();
        // 拼错的参数名即使取值为 null 也要报错，否则依旧会静默丢失
        var config = new Dictionary<string, object?> { ["autoHangoutEnabled"] = null };

        var exception = Assert.Throws<ArgumentException>(() => RealtimeTriggerParamBinder.Bind(param, config));

        Assert.Contains("autoHangoutEventEnabled", exception.Message);
    }

    [Fact]
    public void Bind_NullConfig_ShouldOnlyValidate()
    {
        var param = new AutoSkipTriggerParam();

        var result = RealtimeTriggerParamBinder.Bind(param, null);

        Assert.Same(param, result);
        Assert.Null(result.ClickChatOption);
    }

    [Fact]
    public void Bind_IntegerFromDouble_ShouldNormalize()
    {
        var param = new AutoEatTriggerParam();
        // 脚本侧的数字可能以 double 抵达
        var config = new Dictionary<string, object?> { ["checkInterval"] = 120d, ["eatInterval"] = 1500d };

        var result = RealtimeTriggerParamBinder.Bind(param, config);

        Assert.Equal(120, result.CheckInterval);
        Assert.Equal(1500, result.EatInterval);
    }

    [Fact]
    public void Bind_FractionalNumberForInt_ShouldThrow()
    {
        var param = new AutoEatTriggerParam();
        var config = new Dictionary<string, object?> { ["checkInterval"] = 1.5d };

        var exception = Assert.Throws<ArgumentException>(() => RealtimeTriggerParamBinder.Bind(param, config));

        Assert.Contains("checkInterval", exception.Message);
        Assert.Contains("整数", exception.Message);
    }

    [Fact]
    public void Bind_StringForBool_ShouldThrow()
    {
        var param = new AutoPickTriggerParam();
        var config = new Dictionary<string, object?> { ["forceInteraction"] = "yes" };

        var exception = Assert.Throws<ArgumentException>(() => RealtimeTriggerParamBinder.Bind(param, config));

        Assert.Contains("布尔值", exception.Message);
    }

    [Fact]
    public void Bind_UnsupportedConfigType_ShouldThrow()
    {
        var param = new AutoPickTriggerParam();

        Assert.Throws<ArgumentException>(() => RealtimeTriggerParamBinder.Bind(param, 42));
    }

    [Fact]
    public void Bind_TypedInstance_ShouldBeUsedDirectly()
    {
        var typed = new AutoSkipTriggerParam { RunBackgroundEnabled = true };

        var result = RealtimeTriggerParamBinder.Bind(new AutoSkipTriggerParam(), typed);

        Assert.Same(typed, result);
        Assert.True(result.RunBackgroundEnabled);
    }

    [Fact]
    public void Bind_OutOfRangeDelay_ShouldThrowWithParamName()
    {
        var param = new AutoSkipTriggerParam();
        var config = new Dictionary<string, object?> { ["beforeClickConfirmDelay"] = -1 };

        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => RealtimeTriggerParamBinder.Bind(param, config));

        Assert.Equal("beforeClickConfirmDelay", exception.ParamName);
    }

    [Fact]
    public void Bind_InvalidClickChatOption_ShouldThrowWithAllowedValues()
    {
        var param = new AutoSkipTriggerParam();
        var config = new Dictionary<string, object?> { ["clickChatOption"] = "随便选一个" };

        var exception = Assert.Throws<ArgumentException>(() => RealtimeTriggerParamBinder.Bind(param, config));

        Assert.Contains("clickChatOption", exception.Message);
        Assert.Contains("优先选择第一个选项", exception.Message);
    }

    [Fact]
    public void Bind_SmallCheckInterval_ShouldThrow()
    {
        var param = new AutoEatTriggerParam();
        param.CheckInterval = 5;

        Assert.Throws<ArgumentOutOfRangeException>(param.Validate);
    }
}
