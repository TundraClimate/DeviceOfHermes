using System.Reflection;
using HarmonyLib;

namespace HarmonyExtension;

/// <summary>Harmony extensions</summary>
public static class HarmonyExtension
{
    extension(CodeMatch)
    {
        /// <summary>Calls method</summary>
        public static CodeMatch Calls(MethodInfo method)
        {
            return new CodeMatch(instruction => instruction.Calls(method));
        }

        /// <summary>Is Ldloc</summary>
        public static CodeMatch IsLdloc()
        {
            return new CodeMatch(instruction => instruction.IsLdloc());
        }
    }

    extension(Type ty)
    {
        /// <summary>Access method</summary>
        public MethodInfo Method(string name, Type[]? parameters = null, Type[]? generics = null)
        {
            return AccessTools.Method(ty, name, parameters, generics);
        }

        /// <summary>Access property</summary>
        public PropertyInfo Property(string name)
        {
            return AccessTools.Property(ty, name);
        }

        /// <summary>Access field</summary>
        public FieldInfo Field(string name)
        {
            return AccessTools.Field(ty, name);
        }

        /// <summary>Returns FieldRefAccess</summary>
        public AccessTools.FieldRef<object, F> FieldRefAccess<F>(string name)
        {
            return AccessTools.FieldRefAccess<F>(ty, name);
        }

        /// <summary>Returns StaticFieldRefAccess</summary>
        public ref F StaticFieldRefAccess<F>(string name)
        {
            return ref AccessTools.StaticFieldRefAccess<F>(ty, name);
        }
    }
}
