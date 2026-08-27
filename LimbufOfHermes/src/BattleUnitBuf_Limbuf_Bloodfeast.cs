using System.Runtime.CompilerServices;
using HarmonyLib;
using HarmonyExtension;

namespace LimbufOfHermes;

/// <summary>A stage buf the blood feast</summary>
public class BattleUnitBuf_Limbuf_Bloodfeast : StageBufBase
{
    /// <summary>Impl keywordId</summary>
    protected override string keywordId => "LimbufOfHermes_Bloodfeast";

    /// <summary>Impl OnConsume</summary>
    public override void OnConsume(int stack, BattleUnitModel? consume)
    {
        StageBufListDetail.ChangeBufStack<BattleUnitBuf_Limbuf_BloodfeastConsumedAll>(s => s + stack);

        consume?.bufListDetail?.AddKeywordBufThisRoundByEtc(LimKeywordBuf.BloodfeastConsumed, stack);
    }

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
            if (dty is DamageType.Buf && buf is KeywordBuf.Bleeding)
            {
                BleedDmgHistory.GetValue(StageController.Instance.waveHistory, _ => new(0)).value += dmg;

                StageBufListDetail.ChangeBufStack<BattleUnitBuf_Limbuf_Bloodfeast>(s => 999.Min(s + dmg));
                StageBufListDetail.UpdateUI();

                if (StageBufListDetail.GetBufCount(StageBuf.Bloodfeast) != 0)
                {
                    BattleObjectManager.instance.GetAliveList().ForEach(unit =>
                    {
                        unit.EachPassiveOf<ILimbuf.OnAddBloodfeast>(i => i.OnAddBloodfeast(dmg));
                        unit.EachUnitBufOf<ILimbuf.OnAddBloodfeast>(i => i.OnAddBloodfeast(dmg));
                    });
                }
            }
        }
    }

    internal static ConditionalWeakTable<StageWaveHistory, Box<int>> BleedDmgHistory = new();
}
