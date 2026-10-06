namespace System;

/// <summary>A reference wrapper</summary>
public class Box<T>(T data)
    where T : struct
{
    /// <summary>data</summary>
    public T value = data;
}
