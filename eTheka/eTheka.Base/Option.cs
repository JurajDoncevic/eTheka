using System;
using System.Collections.Generic;

namespace eTheka.Base;

/// <summary>
/// Option class abstracts the existence of data
/// </summary>
/// <typeparam name="T">Abstracted data type</typeparam>
public readonly struct Option<T>
{
    private readonly T _value;
    private readonly bool _isSome;

    /// <summary>
    /// Stored value
    /// </summary>
    public T Value => _value;

    /// <summary>
    /// Is there a stored value
    /// </summary>
    public bool IsSome => _isSome;

    /// <summary>
    /// Is there no stored value
    /// </summary>
    public bool IsNone => !_isSome;

    internal Option(T value)
    {
        _value = value;
        _isSome = _value is { };
    }

    public override bool Equals(object? obj)
    {
        return obj is Option<T> option &&
            _isSome == option._isSome &&
            EqualityComparer<T>.Default.Equals(_value, option._value);

    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_value, _isSome);
    }

    public static implicit operator bool(Option<T> option)
    {
        return option.IsSome;
    }

    public static implicit operator Option<T>(T data)
    {
        return Option.Some(data);
    }

    public static bool operator ==(Option<T> left, Option<T> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Option<T> left, Option<T> right)
    {
        return !(left == right);
    }
}

public static class Option
{
    /// <summary>
    /// Creates a None Option: () -> O[T]
    /// </summary>
    /// <returns>Option object marked as None</returns>
    public static Option<T> None<T>() => new Option<T>();

    /// <summary>
    /// Creates an Option of a value: T -> O[T] 
    /// </summary>
    /// <param name="value">Value inside the Option</param>
    /// <returns>Option object marked as Some</returns>
    public static Option<T> Some<T>(T value) => new Option<T>(value);

    /// <summary>
    /// 
    /// Match if there is Some value or None and execute functions
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="R"></typeparam>
    /// <param name="target"></param>
    /// <param name="onSomeFunc"></param>
    /// <param name="onNoneFunc"></param>
    /// <returns></returns>
    public static R Match<T, R>(this Option<T> target, Func<T, R> onSomeFunc, Func<R> onNoneFunc)
        where T : notnull
        where R : notnull
        => target.IsSome
            ? onSomeFunc(target.Value)
            : onNoneFunc();

    /// <summary>
    /// Bind operation for Option: O[T] -> (T -> O[R]) -> O[R] 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="R"></typeparam>
    /// <param name="target"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    public static Option<R> Bind<T, R>(this Option<T> target, Func<T, Option<R>> func)
        where T : notnull
        where R : notnull
        => target.Match(
            _ => func(_),
            () => Option.None<R>()
            );

    /// <summary>
    /// Map operation for Option: O[T] -> (T -> R) -> O[R]
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="R"></typeparam>
    /// <param name="target"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    public static Option<R> Map<T, R>(this Option<T> target, Func<T, R> func)
        where T : notnull
        where R : notnull
        => target.Match(
            _ => Option.Some<R>(func(_)),
            () => Option.None<R>()
            );
}


