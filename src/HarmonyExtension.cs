using System.Reflection.Emit;
using System.Reflection;
using HarmonyLib;

namespace HarmonyExtension;

/// <summary>Harmony extensions</summary>
public static class HarmonyExtension
{
    extension(CodeMatch)
    {
        /// <summary>Matches OpCode</summary>
        public static CodeMatch IsOpCode(OpCode op) => new CodeMatch(i => i.opcode == op);

        /// <summary>Calls method</summary>
        public static CodeMatch Calls(MethodInfo method) => new CodeMatch(instruction => instruction.Calls(method));

        /// <summary>Is Ldarg</summary>
        public static CodeMatch IsLdarg() => new CodeMatch(instruction => instruction.IsLdarg());

        /// <summary>Is Ldloc</summary>
        public static CodeMatch IsLdloc() => new CodeMatch(instruction => instruction.IsLdloc());

        /// <summary>Is Ldfld</summary>
        public static CodeMatch IsLdfld() => IsOpCode(OpCodes.Ldfld);

        /// <summary>Is Starg</summary>
        public static CodeMatch IsStarg() => new CodeMatch(instruction => instruction.IsStarg());

        /// <summary>Is Stloc</summary>
        public static CodeMatch IsStloc() => new CodeMatch(instruction => instruction.IsStloc());

        /// <summary>Is Stfld</summary>
        public static CodeMatch IsStfld() => IsOpCode(OpCodes.Stfld);
    }

    extension(Type ty)
    {
        /// <summary>Access method</summary>
        public MethodInfo Method(string name, Type[]? parameters = null, Type[]? generics = null)
            => AccessTools.Method(ty, name, parameters, generics);

        /// <summary>Access property</summary>
        public PropertyInfo Property(string name) => AccessTools.Property(ty, name);

        /// <summary>Access field</summary>
        public FieldInfo Field(string name) => AccessTools.Field(ty, name);

        /// <summary>Returns FieldRefAccess</summary>
        public AccessTools.FieldRef<object, F> FieldRefAccess<F>(string name) => AccessTools.FieldRefAccess<F>(ty, name);

        /// <summary>Returns StaticFieldRefAccess</summary>
        public ref F StaticFieldRefAccess<F>(string name) => ref AccessTools.StaticFieldRefAccess<F>(ty, name);
    }

    extension(CodeInstruction instruction)
    {
        /// <summary>Create ldarg_0 instruction</summary>
        public static CodeInstruction Instance => new CodeInstruction(OpCodes.Ldarg_0);

        /// <summary>Create nop instruction</summary>
        public static CodeInstruction Nop => new CodeInstruction(OpCodes.Nop);

        /// <summary>Create ldarg instruction</summary>
        public static CodeInstruction Arg(int idx) => new CodeInstruction(OpCodes.Ldarg, idx);

        /// <summary>Create Call instruction</summary>
        public static CodeInstruction Call(MethodInfo target) => new CodeInstruction(OpCodes.Call, target);

        /// <summary>Create Ldloc instruction</summary>
        public static CodeInstruction Local(int idx) => new CodeInstruction(OpCodes.Ldloc, idx);

        /// <summary>Create Stloc instruction</summary>
        public static CodeInstruction SetLocal(int idx) => new CodeInstruction(OpCodes.Stloc, idx);

        /// <summary>Create Ldfld instruction</summary>
        public static CodeInstruction Field(FieldInfo target) => new CodeInstruction(OpCodes.Ldfld, target);

        /// <summary>Create Stfld instruction</summary>
        public static CodeInstruction SetField(FieldInfo target) => new CodeInstruction(OpCodes.Stfld, target);

        /// <summary>Create literal instruction</summary>
        public static CodeInstruction Literal<T>(T value)
        {
            var result = value switch
            {
                null => new CodeInstruction(OpCodes.Ldnull),

                int i when i == -1 => new CodeInstruction(OpCodes.Ldc_I4_M1),
                int i when i is >= -128 and < 128 => new CodeInstruction(OpCodes.Ldc_I4_S, i),
                int i => new CodeInstruction(OpCodes.Ldc_I4, i),

                uint i when i < 128 => new CodeInstruction(OpCodes.Ldc_I4_S, i),
                uint i => new CodeInstruction(OpCodes.Ldc_I4, i),

                bool b when !b => new CodeInstruction(OpCodes.Ldc_I4_0),
                bool b when b => new CodeInstruction(OpCodes.Ldc_I4_1),

                char c => new CodeInstruction(OpCodes.Ldc_I4_S, (ushort)c),

                byte b when b < 128 => new CodeInstruction(OpCodes.Ldc_I4_S, (int)b),
                byte b => new CodeInstruction(OpCodes.Ldc_I4, (int)b),
                sbyte b => new CodeInstruction(OpCodes.Ldc_I4_S, (int)b),
                short s => new CodeInstruction(OpCodes.Ldc_I4, (int)s),
                ushort s => new CodeInstruction(OpCodes.Ldc_I4, (int)s),

                long l => new CodeInstruction(OpCodes.Ldc_I8, l),
                ulong l => new CodeInstruction(OpCodes.Ldc_I8, l),

                float f => new CodeInstruction(OpCodes.Ldc_R4, f),

                double d => new CodeInstruction(OpCodes.Ldc_R8, d),

                string s => new CodeInstruction(OpCodes.Ldstr, s),

                _ => throw new InvalidOperationException($"{typeof(T).Name} is not literal"),
            };

            return result;
        }

        /// <summary>Create enum instruction</summary>
        public static CodeInstruction Enum<T>(T e)
            where T : Enum
        {
            var num = (int)(object)e;

            return Literal<int>(num);
        }
    }
}
