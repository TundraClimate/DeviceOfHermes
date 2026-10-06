using System.Runtime.CompilerServices;
using HarmonyLib;
using HarmonyExtension;

namespace LimbufOfHermes;

/// <summary>A stage buf the scorch field</summary>
public class BattleUnitBuf_Limbuf_Scorchfield : StageBufBase
{
    /// <summary>Impl keywordId</summary>
    protected override string keywordId => "LimbufOfHermes_Scorchfield";

    [HarmonyPatch(typeof(BattleUnitModel), "TakeDamage")]
    class PatchOnTakeDamage
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);

            matcher.MatchEndForward(CodeMatch.IsLdarg(0), CodeMatch.IsLdloc(), CodeMatch.Calls(typeof(BattleUnitModel).Method("OnLoseHp")))
                .Advance(1)
                .Insert(CodeInstruction.Local(1), CodeInstruction.Arg(2), CodeInstruction.Arg(4), CodeInstruction.Call(typeof(PatchOnTakeDamage).Method("InjectMethod")));

            return matcher.Instructions();
        }

        static void InjectMethod(int dmg, DamageType dty, KeywordBuf buf)
        {
            if (dty is DamageType.Buf && buf is KeywordBuf.Burn)
            {
                BurnDmgHistory.GetValue(StageController.Instance.waveHistory, _ => new(0)).value += dmg;

                StageBufListDetail.AddBufCount(StageBuf.Scorchfield, dmg);
            }
        }
    }

    internal override void AddBuf(int stack)
    {
        StageBufListDetail.ChangeBufStack<BattleUnitBuf_Limbuf_Scorchfield>(s => 999.Min(s + stack));
        StageBufListDetail.UpdateUI();

        if (StageBufListDetail.GetBufCount(StageBuf.Scorchfield) != 0)
        {
            BattleObjectManager.instance.GetAliveList().ForEach(unit =>
            {
                unit.EachPassiveOf<ILimbuf.OnAddScorchfield>(i => i.OnAddScorchfield(stack));
                unit.EachUnitBufOf<ILimbuf.OnAddScorchfield>(i => i.OnAddScorchfield(stack));
            });
        }
    }

    internal static ConditionalWeakTable<StageWaveHistory, Box<int>> BurnDmgHistory = new();
}
