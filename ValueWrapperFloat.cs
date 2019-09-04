public class ValueWrapperFloat : ValueWrapper<float>
{
    public override bool Compare(float value)
    {
        return this._value > value;
    }

    public override bool Equals(float value)
    {
        return this._value == value;
    }

    public override float Add(float value)
    {
        return this._value + value;
    }

    public override float Multiply(float value)
    {
        return this._value * value;
    }
}
