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

    static readonly HashSet<OpCode> ldelem = [
        OpCodes.Ldelem,
        OpCodes.Ldelem_I,
        OpCodes.Ldelem_I1,
        OpCodes.Ldelem_I2,
        OpCodes.Ldelem_I4,
        OpCodes.Ldelem_I8,
        OpCodes.Ldelem_R4,
        OpCodes.Ldelem_R8,
        OpCodes.Ldelem_Ref,
        OpCodes.Ldelem_U1,
        OpCodes.Ldelem_U2,
        OpCodes.Ldelem_U4,
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

        /// <summary>Is Newobj</summary>
        public static CodeMatch IsNewobj(ConstructorInfo? info) => new CodeMatch(i => i.opcode == OpCodes.Newobj && (info is null || i.operand is ConstructorInfo ctor && ctor == info));

        /// <summary>Is Const</summary>
        public static CodeMatch IsConst() => new CodeMatch(i => i.LoadsConstant());

        /// <summary>Is Const</summary>
        public static CodeMatch IsConst(long num) => new CodeMatch(i => i.LoadsConstant(num));

        /// <summary>Is Const</summary>
        public static CodeMatch IsConst(double num) => new CodeMatch(i => i.LoadsConstant(num));

        /// <summary>Is Const</summary>
        public static CodeMatch IsConst(Enum num) => new CodeMatch(i => i.LoadsConstant(num));

        /// <summary>Is Ldelem</summary>
        public static CodeMatch IsLdelem() => new CodeMatch(i => ldelem.Contains(i.opcode));

        /// <summary>Is Ldlen</summary>
        public static CodeMatch IsLdlen() => new CodeMatch(i => i.opcode == OpCodes.Ldlen);

        /// <summary>Is Ret</summary>
        public static CodeMatch IsRet() => new CodeMatch(i => i.opcode == OpCodes.Ret);

        /// <summary>Is Dup</summary>
        public static CodeMatch IsDup() => new CodeMatch(i => i.opcode == OpCodes.Dup);

        /// <summary>Is Pop</summary>
        public static CodeMatch IsPop() => new CodeMatch(i => i.opcode == OpCodes.Pop);

        /// <summary>Is Leave</summary>
        public static CodeMatch IsLeave() => new CodeMatch(i => i.opcode == OpCodes.Leave || i.opcode == OpCodes.Leave_S);
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

        /// <summary>Create dup instruction</summary>
        public static CodeInstruction Dup => new CodeInstruction(OpCodes.Dup);

        /// <summary>Create Null instruction</summary>
        public static CodeInstruction Null => new CodeInstruction(OpCodes.Ldnull);

        /// <summary>Create Ret instruction</summary>
        public static CodeInstruction Ret => new CodeInstruction(OpCodes.Ret);

        /// <summary>Create Add instruction</summary>
        public static CodeInstruction Add => new CodeInstruction(OpCodes.Add);

        /// <summary>Create Sub instruction</summary>
        public static CodeInstruction Sub => new CodeInstruction(OpCodes.Sub);

        /// <summary>Create Mul instruction</summary>
        public static CodeInstruction Mul => new CodeInstruction(OpCodes.Mul);

        /// <summary>Create Div instruction</summary>
        public static CodeInstruction Div => new CodeInstruction(OpCodes.Div);

        /// <summary>Create ldarg instruction</summary>
        public static CodeInstruction Ldarg(int idx) => new CodeInstruction(OpCodes.Ldarg, idx);

        /// <summary>Create ldarga instruction</summary>
        public static CodeInstruction Ldarga(int idx) => new CodeInstruction(OpCodes.Ldarga, idx);

        /// <summary>Create Call instruction</summary>
        public static CodeInstruction Call(MethodInfo target) => new CodeInstruction(OpCodes.Call, target);

        /// <summary>Create Ldloc instruction</summary>
        public static CodeInstruction Ldloc(int idx) => new CodeInstruction(OpCodes.Ldloc, idx);

        /// <summary>Create Ldloca instruction</summary>
        public static CodeInstruction Ldloca(int idx) => new CodeInstruction(OpCodes.Ldloca, idx);

        /// <summary>Create Stloc instruction</summary>
        public static CodeInstruction Stloc(int idx) => new CodeInstruction(OpCodes.Stloc, idx);

        /// <summary>Create Ldfld instruction</summary>
        public static CodeInstruction Ldfld(FieldInfo target) => new CodeInstruction(OpCodes.Ldfld, target);

        /// <summary>Create Stfld instruction</summary>
        public static CodeInstruction Stfld(FieldInfo target) => new CodeInstruction(OpCodes.Stfld, target);

        /// <summary>Create const instruction</summary>
        public static CodeInstruction Const(int num) => new CodeInstruction(OpCodes.Ldc_I4, num);

        /// <summary>Create const instruction</summary>
        public static CodeInstruction Const(long num) => new CodeInstruction(OpCodes.Ldc_I8, num);

        /// <summary>Create const instruction</summary>
        public static CodeInstruction Const(float num) => new CodeInstruction(OpCodes.Ldc_R4, num);

        /// <summary>Create const instruction</summary>
        public static CodeInstruction Const(double num) => new CodeInstruction(OpCodes.Ldc_R8, num);

        /// <summary>Create const instruction</summary>
        public static CodeInstruction Const(bool b) => b ? new CodeInstruction(OpCodes.Ldc_I4_1) : new CodeInstruction(OpCodes.Ldc_I4_0);

        /// <summary>Create const instruction</summary>
        public static CodeInstruction Const(string str) => new CodeInstruction(OpCodes.Ldstr, str);

        /// <summary>Create const instruction</summary>
        public static CodeInstruction Const(Enum e) => new CodeInstruction(OpCodes.Ldc_I4, Convert.ToInt32(e));
    }

    extension(MethodInfo method)
    {
        /// <summary>Returns patch list</summary>
        public List<PatchInfo> GetPatches()
        {
            var info = Harmony.GetPatchInfo(method);

            if (info is null)
            {
                return [];
            }

            var res = new List<PatchInfo>();

            foreach (var prefix in info.Prefixes)
            {
                res.Add(new(PatchType.Prefix, prefix.owner, prefix.PatchMethod, prefix.priority));
            }

            foreach (var postfix in info.Postfixes)
            {
                res.Add(new(PatchType.Postfix, postfix.owner, postfix.PatchMethod, postfix.priority));
            }

            foreach (var transpiler in info.Transpilers)
            {
                res.Add(new(PatchType.Transpiler, transpiler.owner, transpiler.PatchMethod, transpiler.priority));
            }

            foreach (var finalizer in info.Finalizers)
            {
                res.Add(new(PatchType.Transpiler, finalizer.owner, finalizer.PatchMethod, finalizer.priority));
            }

            return res;
        }
    }

    /// <summary>A struct patch info</summary>
    public struct PatchInfo(PatchType patchType, string owner, MethodInfo method, int priority)
    {
        /// <summary>PatchType</summary>
        public PatchType PatchType { get; private set; } = patchType;

        /// <summary>Owner</summary>
        public string Owner { get; private set; } = owner;

        /// <summary>Method</summary>
        public MethodInfo Method { get; private set; } = method;

        /// <summary>Priority</summary>
        public int Priority { get; private set; } = priority;
    }

    /// <summary>A list patch type</summary>
    public enum PatchType
    {
        /// <summary>Prefix</summary>
        Prefix,

        /// <summary>Postfix</summary>
        Postfix,

        /// <summary>Transpiler</summary>
        Transpiler,

        /// <summary>Finalizer</summary>
        Finalizer,
    }
}
