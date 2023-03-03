using System;

public class Ref<T> : IOutRef<T>, IInRef<T>
{
    public bool HasValue { get; private set; }
    private T m_Value;

    public T Value
    {
        get
        {
            if (!HasValue)
            {
                throw new InvalidOperationException("Value is not set");
            }
            
            return m_Value;
        }
        set
        {
            m_Value = value;
            HasValue = true;
        }
    }
    
    public static implicit operator T(Ref<T> refValue)
    {
        return refValue.Value;
    }
    
    public static implicit operator Ref<T>(T value)
    {
        return new Ref<T>
        {
            Value = value
        };
    }

    public override string ToString()
    {
        return $"{nameof(Value)}: {Value}";
    }
}

public interface IOutRef<out T>
{
    public T Value { get; }
}

public interface IInRef<in T>
{
    public T Value { set; }
}
