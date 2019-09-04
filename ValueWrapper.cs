using System;
using System.Collections.Generic;

public abstract class ValueWrapper<TValue>
{
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
                foreach (Action action in onValueModified)
                {
                    action();
                }
            }
        }
    }

    public List<Action> onValueModified  = new List<Action>();

    //public IPropertyModify propertyModify;
    public bool Compare(ValueWrapper<TValue> _wrapper)
    {
        return this.Compare(_wrapper._value);
    }

    public bool Equals(ValueWrapper<TValue> _wrapper)
    {
        return this.Equals(_wrapper._value);
    }

    public TValue Add(ValueWrapper<TValue> _wrapper)
    {
        return this.Add(_wrapper.value);
    }

    public TValue Multiply(ValueWrapper<TValue> _wrapper)
    {
        return this.Multiply(_wrapper._value);
    }

    public abstract bool Compare(TValue _value);

    public abstract bool Equals(TValue _value);

    public abstract TValue Add(TValue _value);
    public abstract TValue Multiply(TValue _value);
}

