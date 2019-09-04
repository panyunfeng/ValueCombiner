public class ValueWrapperBool : ValueWrapper<bool>
{
    public override bool Compare(bool _value)
    {
        if (this._value)
            return true;

        return !_value;
    }

    public override bool Equals(bool value)
    {
        return this._value == value;
    }

    public override bool Add(bool value)
    {
        return this._value || value;
    }

    public override bool Multiply(bool value)
    {
        return this._value || value;
    }
}
