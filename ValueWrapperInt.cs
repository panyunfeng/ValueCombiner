
public class ValueWrapperInt : ValueWrapper<int>
{
    public override bool Compare(int value)
    {
        return this._value > value;
    }

    public override bool Equals(int value)
    {
        return this._value == value;
    }

    public override int Add(int value)
    {
        return this._value + value;
    }

    public override int Multiply(int value)
    {
        return this._value * value;
    }
}
