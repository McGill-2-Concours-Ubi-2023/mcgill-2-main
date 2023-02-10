public class Ref<T> : IOutRef<T>, IInRef<T>
{
    public T Value { get; set; }
}

public interface IOutRef<out T>
{
    public T Value { get; }
}

public interface IInRef<in T>
{
    public T Value { set; }
}
