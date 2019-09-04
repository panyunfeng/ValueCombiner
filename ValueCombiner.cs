using System;
using System.Collections.Generic;

public enum BlendType
{
    ADDITIVE,
    REPLACE,
    MAXIMUM,
    MINIMUM,
    MULTIPLY,
}

/// <summary>
/// 数值合并器，用于合并同类型不同组合方式的数值
/// </summary>
/// <typeparam name="TWrapper"></typeparam>
/// <typeparam name="TValue"></typeparam>
public abstract class ValueCombiner<TWrapper, TValue> where TWrapper : ValueWrapper<TValue>, new()
{
    public BlendType blendType;

    protected TValue _defaultValue;

    public TValue defaultValue
    {
        set { _defaultValue = this.value = value; }
    }

    protected TValue _value;

    public TValue value
    {
        get
        {
            return _value;
        }

        set
        {
            if (!Equals(value))
            {
                _value = value;
                OnValueModified();
            }
        }
    }

    public void SetValueWithOutUpdate(TValue value)
    {
        if (!_value.Equals(value))
        {
            _value = value;
        }
    }

    protected virtual void OnValueModified()
    {
        if (_valueModified == null)
            return;
        foreach (Action<TValue> action in _valueModified)
        {
            action(value);
        }
    }
    private const int InitListCapacity = 7;

    private List<TWrapper> _valueWrappers;

    private List<Action<TValue>> _valueModified;

    public ValueCombiner()
    {

    }
    public abstract bool Equals(TValue _value);

    public void AddValueModified(Action<TValue> _value)
    {
        if (_valueModified == null)
        {
            _valueModified = new List<Action<TValue>>();
        }
        _valueModified.Add(_value);
    }

    public void AddWrapper(TWrapper ret)
    {
        if (ret == null)
            return;

        if (_valueWrappers == null)
            _valueWrappers = new List<TWrapper>(InitListCapacity);

        if (_valueWrappers.Contains(ret))
        {
            return;
        }
        _valueWrappers.Add(ret);
        ret.onValueModified.Add(OnItemValueModified);

        value = GetValue();
    }

    public void RemoveWrapper(TWrapper ret)
    {
        if (ret == null || _valueWrappers == null)
            return;

        if (!_valueWrappers.Contains(ret))
            return;

        _valueWrappers.Remove(ret);
        ret.onValueModified.Remove(OnItemValueModified);

        value = GetValue();
    }

    protected void OnItemValueModified()
    {
        value = GetValue();
    }

    private TValue GetValue()
    {
        switch (blendType)
        {
            case BlendType.ADDITIVE:
                return value = GetValueAdditive();
            case BlendType.MAXIMUM:
                return value = GetValueMaximum();
            case BlendType.MINIMUM:
                return value = GetValueMinimum();
            case BlendType.MULTIPLY:
                return value = GetValueMultiply();
        }

        return _defaultValue;
    }

    public void Clear()
    {
        if (_valueWrappers != null)
        {
            for (int i = 0; i < _valueWrappers.Count; i++)
            {
                _valueWrappers[i] = null;
            }

            _valueWrappers.Clear();
        }
    }

    #region Utility

    /// <summary>
    /// 累加方式
    /// </summary>
    /// <returns></returns>
    private TValue GetValueAdditive()
    {
        if (_valueWrappers == null || _valueWrappers.Count <= 0)
            return _defaultValue;

        TValue ret = _valueWrappers[0].value;

        for (int i = 1; i < _valueWrappers.Count; i++)
        {
            ret = _valueWrappers[i].Add(ret);
        }
        return ret;
    }

    /// <summary>
    /// 取最大效果
    /// </summary>
    /// <returns></returns>
    private TValue GetValueMaximum()
    {
        if (_valueWrappers == null || _valueWrappers.Count <= 0)
            return _defaultValue;

        TValue ret = _valueWrappers[0].value;

        for (int i = 1; i < _valueWrappers.Count; i++)
        {
            if (_valueWrappers[i].Compare(ret))
                ret = _valueWrappers[i].value;
        }

        return ret;
    }

    private TValue GetValueMinimum()
    {
        if (_valueWrappers == null || _valueWrappers.Count <= 0)
            return _defaultValue;

        TValue ret = _valueWrappers[0].value;

        for (int i = 1; i < _valueWrappers.Count; i++)
        {
            if (!_valueWrappers[i].Compare(ret))
                ret = _valueWrappers[i].value;
        }

        return ret;
    }

    private TValue GetValueMultiply()
    {
        if (_valueWrappers == null || _valueWrappers.Count <= 0)
            return _defaultValue;

        TValue ret = _valueWrappers[0].value;

        for (int i = 1; i < _valueWrappers.Count; i++)
        {
            ret = _valueWrappers[i].Multiply(ret);
        }

        return ret;
    }

    #endregion
}

public class ValueCombinerInt : ValueCombiner<ValueWrapperInt, int>
{
    public override bool Equals(int value)
    {
        return _value == value;
    }
}

public class ValueCombinerBool : ValueCombiner<ValueWrapperBool, bool>
{
    public override bool Equals(bool value)
    {
        return _value == value;
    }
}
public class ValueCombinerFloat : ValueCombiner<ValueWrapperFloat, float>
{
    public override bool Equals(float value)
    {
        return _value == value;
    }
}


public class UnitBuffValueCombinerBool : ValueCombiner<ValueWrapperBool, bool>
{
    public LogicX2.X2BuffType buffType;
    public Action<LogicX2.X2BuffType, bool> onModified;
    public override bool Equals(bool value)
    {
        return _value == value;
    }

    protected override void OnValueModified()
    {
        base.OnValueModified();

        if (onModified != null)
        {
            onModified(buffType, value);
        }
    }
}

public class UnitBuffValueCombinerSmartLong : ValueCombiner<ValueWrapperSmartLong, LogicX2.SmartLong>
{
    public LogicX2.X2BuffType buffType;
    public Action<LogicX2.X2BuffType, LogicX2.SmartLong> onModified;

    public override bool Equals(LogicX2.SmartLong value)
    {
        return _value == value;
    }
    protected override void OnValueModified()
    {
        base.OnValueModified();

        if (onModified != null)
        {
            onModified(buffType, value);
        }
    }
}

public class UnitPropertyValueCombinerSmartLong : ValueCombiner<ValueWrapperSmartLong, LogicX2.SmartLong>
{
    public Action propertyValueUpdate;
    public Example.AttribTypeENameEnu type;
    private ValueWrapperSmartLong defaultWrapper;
    private Dictionary<string, LogicX2.SmartLong> TempVariables;
    public LogicX2.SmartLong max;
    public LogicX2.SmartLong min;
    public override bool Equals(LogicX2.SmartLong value)
    {
        return _value == value;
    }
    public void AddNum(string keyname, LogicX2.SmartLong num)
    {
        if (TempVariables == null)
            TempVariables = new Dictionary<string, LogicX2.SmartLong>();
        LogicX2.SmartLong val;
        if (TempVariables.TryGetValue(keyname, out val))
        {
            if (val != 0)
            {
                SubNum(val);
            }
        }
        TempVariables[keyname] = num;
        AddNum(num);
    }

    public void SubNum(string keyname)
    {
        LogicX2.SmartLong val;
        if (TempVariables != null && TempVariables.TryGetValue(keyname, out val))
        {

            if (val != 0)
            {
                SubNum(val);
                TempVariables[keyname] = 0;
            }
        }
    }
    public void AddNum(LogicX2.SmartLong num)
    {
        if (defaultWrapper == null)
        {
            defaultWrapper = new ValueWrapperSmartLong();
            defaultWrapper.value = num;
            AddWrapper(defaultWrapper);
        }
        else
        {
            defaultWrapper.value += num;
        }
    }

    public void SubNum(LogicX2.SmartLong num)
    {
        if (defaultWrapper != null)
        {
            defaultWrapper.value -= num;
        }
    }

    protected override void OnValueModified()
    {
        if (min != max)
        {
            if (value > max)
            {
                value = max;
            }
            if (value < min)
            {
                value = min;
            }
        }
        base.OnValueModified();
        if (propertyValueUpdate != null)
        {
            propertyValueUpdate();
        }
    }

    public void ResetValue()
    {
        value = 0;
        if (TempVariables != null)
            TempVariables.Clear();
        Clear();
        if (defaultWrapper != null)
        {
            defaultWrapper.value = 0;
            AddWrapper(defaultWrapper);
        }
    }
}
