using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace DeviceOfHermes;

/// <summary>A options of new map generation</summary>
[Flags]
public enum NewMapOptions
{
    /// <summary>None</summary>
    None,

    /// <summary>NoBlackFrame</summary>
    NoBlackFrame,
}

/// <summary>A setter the reception map</summary>
public class AutoMapSetter
{
    static AutoMapSetter()
    {
        var harmony = new Harmony("DeviceOfHermes.AutoMapSetter");

        harmony.CreateClassProcessor(typeof(PatchOnMapInitialize)).Patch();
        harmony.CreateClassProcessor(typeof(PatchOnMapChange)).Patch();
    }

    private AutoMapSetter()
    {
    }

    /// <summary>Sets map factory</summary>
    public static void SetMapFactory(string key, Func<AutoMapSetter, MapManager> factory)
    {
        if (mapFactory.ContainsKey(key))
        {
            Hermes.Say($"AutoMap: overwritten key the '{key}'", MessageLevel.Warn);
        }

        mapFactory[key] = factory;
    }

    /// <summary>Creates new map from parts</summary>
    public MapManager CreateNewMap(
        Sprite bgSprite,
        Sprite floorSprite,
        AudioClip[]? bgm = null,
        Color? dlgColor = null,
        List<string>? dlgIds = null,
        NewMapOptions options = NewMapOptions.None)
    {
        var origin = Util.LoadPrefab("CreatureMaps/CreatureMap_Wizard", BattleSceneRoot.Instance.transform);

        origin.GetComponentsInChildren<Component>().Foreach(cpnt =>
        {
            if (cpnt is SpriteRenderer renderer)
            {
                if (renderer.name.Contains("BG"))
                {
                    renderer.sprite = bgSprite;
                }
                else if (renderer.name.Contains("Floor"))
                {
                    renderer.sprite = floorSprite;
                }
                else if (renderer.name.Contains("Backgrounds_Boundaries"))
                {
                    UnityObject.Destroy(cpnt);
                }
                else if (renderer.name.Contains("Frame") && options.HasFlag(NewMapOptions.NoBlackFrame))
                {
                    UnityObject.Destroy(cpnt);
                }
            }
        });

        var manager = origin.GetComponent<MapManager>();

        manager.isCreature = false;
        manager.isBossPhase = false;
        manager.isEgo = false;

        if (bgm is not null)
        {
            manager.mapBgm = bgm;
        }

        if (dlgIds is not null && dlgIds.Count != 0)
        {
            origin.AddComponent<DialogManager>().Init(dlgColor ?? Color.white, dlgIds);
        }

        return manager;
    }

    static Dictionary<string, Func<AutoMapSetter, MapManager>> mapFactory = new();

    [RequireComponent(typeof(MapManager))]
    class DialogManager : MonoBehaviour
    {
        public void Init(Color dlgColor, params List<string> dlgIds)
        {
            _dlgIds = dlgIds;
            _dlgColor = dlgColor;
        }

        void CreateDialog()
        {
            if (_dlgIds is null || _dlgIds.Count == 0)
            {
                return;
            }

            _idx %= _dlgIds.Count;

            var txt = TextDataModel.GetText(_dlgIds[_idx], []);

            _dlgEffect?.FadeOut();

            _dlgEffect = CreatureDlgManagerUI.Instance.SetDlg(txt, _dlgColor, null);

            _idx += 1;
        }

        void Update()
        {
            if (!_init)
            {
                CreatureDlgManagerUI.Instance.Init(true);

                _init = true;
            }

            if (_dlgEffect is not null && !_dlgEffect.DisplayDone)
            {
                return;
            }

            CreateDialog();
        }

        CreatureDlgEffectUI? _dlgEffect;

        List<string>? _dlgIds;

        Color _dlgColor;

        int _idx;

        bool _init;
    }

    [HarmonyPatch(typeof(StageController), "InitializeMap")]
    class PatchOnMapInitialize
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);

            matcher.MatchStartForward(CodeMatch.IsLdloc(), CodeMatch.IsOpCode(OpCodes.Brfalse), CodeMatch.IsLdloc(), CodeMatch.IsOpCode(OpCodes.Ldsfld), CodeMatch.Calls(typeof(string).Method("op_Inequality")))
                .Insert(new CodeInstruction(OpCodes.Ldloca, 2), CodeInstruction.Call(typeof(PatchOnMapInitialize).Method("InjectMethod")));

            return matcher.Instructions();
        }

        static void InjectMethod(ref string mapinfo)
        {
            if (mapinfo.StripPrefix("AutoMap_") is string pref && mapFactory.TryGetValue(pref, out var factory))
            {
                var map = factory(new());

                map.gameObject.name = $"InvitationMap_{mapinfo}";

                BattleSceneRoot.Instance.InitInvitationMap(map);

                mapinfo = "";
            }
        }
    }

    [HarmonyPatch(typeof(StageController), "CheckMapChange")]
    class PatchOnMapChange
    {
        static void Prefix()
        {
            var mapinfo = StageController.Instance.GetStageModel().GetCurrentMapInfo();

            if (mapinfo.StripPrefix("AutoMap_") is string pref && mapFactory.ContainsKey(pref))
            {
                StageController.Instance.GetCurrentWaveModel().team.emotionTotalBonus = StageController.Instance.GetCurrentStageFloorModel().team.emotionTotalCoinNumber + 1;
            }
        }
    }
}
