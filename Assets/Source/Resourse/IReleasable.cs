using System;

public interface IReleasable<T>
{
    event Action<T> Released;
}
