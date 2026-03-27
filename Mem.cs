namespace System;

public static class Mem
{
    public static void Swap<T>(ref T a, ref T b)
    {
        (a, b) = (b, a);
    }

    public static T? Take<T>(ref T? self)
        where T : class
    {
        return Mem.Replace(ref self, default(T));
    }

    public static object? Take(ref object? self)
    {
        return Mem.Replace(ref self, null);
    }

    public static T? Replace<T>(ref T? self, T? newValue)
        where T : class
    {
        return Interlocked.Exchange(ref self, newValue);
    }

    public static int Replace(ref int self, int newValue)
    {
        return Interlocked.Exchange(ref self, newValue);
    }

    public static float Replace(ref float self, float newValue)
    {
        return Interlocked.Exchange(ref self, newValue);
    }

    public static long Replace(ref long self, long newValue)
    {
        return Interlocked.Exchange(ref self, newValue);
    }

    public static double Replace(ref double self, double newValue)
    {
        return Interlocked.Exchange(ref self, newValue);
    }

    public static IntPtr Replace(ref IntPtr self, IntPtr newValue)
    {
        return Interlocked.Exchange(ref self, newValue);
    }

    public static object? Replace(ref object? self, object? newValue)
    {
        return Interlocked.Exchange(ref self, newValue);
    }

    public static T? ReplaceIf<T>(ref T? self, T? newValue, T? expected)
        where T : class
    {
        return Interlocked.CompareExchange(ref self, newValue, expected);
    }

    public static int ReplaceIf(ref int self, int newValue, int expected)
    {
        return Interlocked.CompareExchange(ref self, newValue, expected);
    }

    public static float ReplaceIf(ref float self, float newValue, float expected)
    {
        return Interlocked.CompareExchange(ref self, newValue, expected);
    }

    public static long ReplaceIf(ref long self, long newValue, long expected)
    {
        return Interlocked.CompareExchange(ref self, newValue, expected);
    }

    public static double ReplaceIf(ref double self, double newValue, double expected)
    {
        return Interlocked.CompareExchange(ref self, newValue, expected);
    }

    public static IntPtr ReplaceIf(ref IntPtr self, IntPtr newValue, IntPtr expected)
    {
        return Interlocked.CompareExchange(ref self, newValue, expected);
    }

    public static object? ReplaceIf(ref object? self, object? newValue, object? expected)
    {
        return Interlocked.CompareExchange(ref self, newValue, expected);
    }
}
