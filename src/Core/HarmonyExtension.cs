using System.Reflection.Emit;
using System.Reflection;
using HarmonyLib;

namespace DeviceOfHermes;

/// <summary>Harmony extensions</summary>
public static class HarmonyExtension
{
    static readonly HashSet<OpCode> ldloc = [
        OpCodes.Ldloc,
        OpCodes.Ldloc_S,
        OpCodes.Ldloc_0,
        OpCodes.Ldloc_1,
        OpCodes.Ldloc_2,
        OpCodes.Ldloc_3,
    ];

    static readonly HashSet<OpCode> ldloca = [
        OpCodes.Ldloca,
        OpCodes.Ldloca_S,
    ];

    static readonly HashSet<OpCode> stloc = [
        OpCodes.Stloc,
        OpCodes.Stloc_S,
        OpCodes.Stloc_0,
        OpCodes.Stloc_1,
        OpCodes.Stloc_2,
        OpCodes.Stloc_3,
    ];

    static readonly HashSet<OpCode> ldfld = [
        OpCodes.Ldfld,
        OpCodes.Ldsfld,
    ];

    static readonly HashSet<OpCode> stfld = [
        OpCodes.Stfld,
        OpCodes.Stsfld,
    ];

    extension(CodeMatch)
    {
        /// <summary>Matches OpCode</summary>
        public static CodeMatch IsOpCode(OpCode op) => new CodeMatch(i => i.opcode == op);

        /// <summary>Calls method</summary>
        public static CodeMatch Calls(MethodInfo method) => new CodeMatch(instruction => instruction.Calls(method));

        /// <summary>Is Ldarg</summary>
        public static CodeMatch IsLdarg(int? num = null) => new CodeMatch(instruction => instruction.IsLdarg(num));

        /// <summary>Is Ldarga</summary>
        public static CodeMatch IsLdarga(int? num = null) => new CodeMatch(instruction => instruction.IsLdarga(num));

        /// <summary>Is Starg</summary>
        public static CodeMatch IsStarg(int? num = null) => new CodeMatch(instruction => instruction.IsStarg(num));

        /// <summary>Is Ldloc</summary>
        public static CodeMatch IsLdloc(int? idx = null) => new CodeMatch(instruction => ldloc.Contains(instruction.opcode) && (idx is null || instruction.operand is LocalVariableInfo i && i.LocalIndex == idx));

        /// <summary>Is Ldloca</summary>
        public static CodeMatch IsLdloca(int? idx = null) => new CodeMatch(instruction => ldloca.Contains(instruction.opcode) && (idx is null || instruction.operand is LocalVariableInfo i && i.LocalIndex == idx));

        /// <summary>Is Stloc</summary>
        public static CodeMatch IsStloc(int? idx = null) => new CodeMatch(instruction => stloc.Contains(instruction.opcode) && (idx is null || instruction.operand is LocalVariableInfo i && i.LocalIndex == idx));

        /// <summary>Is Ldfld</summary>
        public static CodeMatch IsLdfld(FieldInfo? info = null) => new CodeMatch(i => info is null ? ldfld.Contains(i.opcode) : i.LoadsField(info));

        /// <summary>Is Stfld</summary>
        public static CodeMatch IsStfld(FieldInfo? info = null) => new CodeMatch(i => info is null ? stfld.Contains(i.opcode) : i.StoresField(info));
    }

    extension(Type ty)
    {
        /// <summary>Access method</summary>
        public MethodInfo Method(string name, Type[]? parameters = null, Type[]? generics = null) => AccessTools.Method(ty, name, parameters, generics);

        /// <summary>Access property</summary>
        public PropertyInfo Property(string name) => AccessTools.Property(ty, name);

        /// <summary>Access ctor</summary>
        public ConstructorInfo Ctor(Type[]? parameters = null) => AccessTools.Constructor(ty, parameters);

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

        /// <summary>Replace labels</summary>
        public List<Label> ReplaceLabels(List<Label> newLabels)
        {
            var old = instruction.ExtractLabels();

            instruction.WithLabels(newLabels);

            return old;
        }
    }
}
