using System.Reflection.Emit;
using HarmonyLib;
using HarmonyExtension;

namespace LimbufOfHermes;

/// <summary>A unit buf the Nails</summary>
public class BattleUnitBuf_Limbuf_Nails : LimbufBase
{
    /// <summary>Impl keywordId</summary>
    protected override string keywordId => "LimbufOfHermes_Nails";

    /// <summary>Impl bufType</summary>
    public override KeywordBuf bufType => LimKeywordBuf.Nails;

    /// <summary>Impl positiveType</summary>
    public override BufPositiveType positiveType => BufPositiveType.Negative;

    [HarmonyPatch(typeof(BattleUnitBuf_bleeding), "AfterDiceAction")]
    class PatchAfterAction
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);

            var skip = matcher.MatchEndForward(
                    new CodeMatch(i => i.opcode == OpCodes.Ldc_I4_S && i.OperandIs(47)),
                    CodeMatch.Calls(typeof(BattleUnitBufListDetail).Method("GetActivatedBuf")),
                    CodeMatch.IsOpCode(OpCodes.Brtrue))
                .Instruction
                .Clone();

            matcher.Advance(1)
                .Insert(
                    CodeInstruction.Instance,
                    CodeInstruction.Field(typeof(BattleUnitBuf).Field("_owner")),
                    CodeInstruction.Call(typeof(PatchAfterAction).Method("InjectMethod")),
                    skip
                );

            return matcher.Instructions();
        }

        static bool InjectMethod(BattleUnitModel owner)
        {
            if (owner.bufListDetail.GetActivatedBuf(LimKeywordBuf.Nails) is BattleUnitBuf_Limbuf_Nails nail && nail.stack > 0)
            {
                nail.ChangeStack(s => s - 1);

                return true;
            }

            return false;
        }
    }
}
