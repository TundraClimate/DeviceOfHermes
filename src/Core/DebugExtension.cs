using System.Diagnostics;
using System.Reflection;

namespace DeviceOfHermes;

/// <summary>An extension class the Debug utility</summary>
public static class DebugExtension
{
    extension(Exception ex)
    {
        /// <summary>Returns ExceptionInfo</summary>
        public ExceptionInfo? GetInfo()
        {
            if (new StackTrace(ex, false).GetFrame(0) is not StackFrame stackTrace)
            {
                return null;
            }

            return new() { Method = stackTrace.GetMethod(), ILOffset = stackTrace.GetILOffset() };
        }
    }

    /// <summary>A struct represents exception info</summary>
    public struct ExceptionInfo
    {
        /// <summary>Method</summary>
        public MethodBase Method;

        /// <summary>ILOffset</summary>
        public int ILOffset;

        /// <summary>Impl ToString</summary>
        public override string ToString()
        {
            return $"{{ Method: {Method.DeclaringType.FullName}.{Method.Name}['{Method.GetParameters().Length}], ILOffset: 0x{ILOffset:X} }}";
        }
    }
}
