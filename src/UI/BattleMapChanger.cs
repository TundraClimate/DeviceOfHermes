using System.Reflection.Emit;
using HarmonyLib;
using HarmonyExtension;
using UnityEngine;

namespace DeviceOfHermes.UI;

/// <summary>Changes battle map</summary>
public static class BattleMapChanger
{
    static BattleMapChanger()
    {
        var harmony = new Harmony("DeviceOfHermes.UI.BattleMapChanger");

        harmony.CreateClassProcessor(typeof(PatchChangeMap)).Patch();
        harmony.CreateClassProcessor(typeof(PatchOnClearMap)).Patch();
        harmony.CreateClassProcessor(typeof(PatchOnCreateDialog)).Patch();
    }

    /// <summary>Change to Original map with MapManager</summary>
    public static void SetMap(MapManager manager, AudioClip? bgm = null)
    {
        manager.gameObject.SetActive(false);

        _fixedMap = true;
        _map = manager;
        _mapObject = manager.gameObject;
        _originalMap = true;
        _bgSprite = null;
        _floorSprite = null;
        _bgm = bgm;
        _dialogs.Clear();
        _dlgColor = Color.white;
    }

    /// <summary>Change to sprites with dialogs</summary>
    public static void SetMap(
        Sprite bg,
        Sprite floor,
        AudioClip? bgm = null,
        List<string>? dialogIds = null,
        Color? dlgColor = null
    )
    {
        var pref = Util.LoadPrefab("CreatureMaps/CreatureMap_Wizard", BattleSceneRoot.Instance.transform);

        pref.SetActive(false);

        _fixedMap = true;
        _map = pref.GetComponent<MapManager>()!;
        _mapObject = pref;
        _originalMap = false;
        _bgSprite = bg;
        _floorSprite = floor;
        _bgm = bgm;
        _dialogs = dialogIds ?? new();
        _dlgColor = dlgColor ?? Color.white;
    }

    private static void UnsetMap()
    {
        if (_mapObject is not null)
        {
            UnityObject.Destroy(_mapObject.gameObject);
        }

        if (_prevMap is not null)
        {
            BattleSceneRoot.Instance.currentMapObject = _prevMap;

            BattleSoundManager.Instance.SetEnemyTheme(_prevMap.mapBgm);
            BattleSoundManager.Instance.ChangeEnemyTheme(0);
        }

        _fixedMap = false;
        _map = null;
        _originalMap = false;
        _bgSprite = null;
        _floorSprite = null;
        _bgm = null;
        _prevMap = null;
        _dialogs.Clear();
        _dlgColor = Color.white;
    }

    private static bool _fixedMap;

    private static MapManager? _prevMap;

    private static MapManager? _map;

    private static GameObject? _mapObject;

    private static bool _originalMap;

    private static Sprite? _bgSprite;

    private static Sprite? _floorSprite;

    private static AudioClip? _bgm;

    private static List<string> _dialogs = new();

    private static Color _dlgColor = Color.white;

    [HarmonyPatch(typeof(StageController), "CheckMapChange")]
    class PatchChangeMap
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var matcher = new CodeMatcher(instructions);

            matcher.MatchEndForward(CodeMatch.IsLdarg(0), CodeMatch.IsLdfld(), CodeMatch.IsOpCode(OpCodes.Brtrue));

            var label = matcher.Instruction.operand;
            var midLabel = generator.DefineLabel();

            matcher.MatchStartForward(
                CodeMatch.IsOpCode(OpCodes.Ldloc_0),
                CodeMatch.IsOpCode(OpCodes.Ldloc_1),
                CodeMatch.IsOpCode(OpCodes.Ble)
            )
                .Insert(
                    CodeInstruction.Call(typeof(PatchChangeMap).Method("Pred")).WithLabels(matcher.Instruction.ReplaceLabels([midLabel])),
                    new CodeInstruction(OpCodes.Brfalse, midLabel),
                    CodeInstruction.Call(typeof(PatchChangeMap).Method("InjectMethod")),
                    new CodeInstruction(OpCodes.Br, label)
                );

            return matcher.Instructions();
        }

        static bool Pred()
        {
            return _fixedMap;
        }

        static void InjectMethod()
        {
            if (_map is null)
            {
                return;
            }

            ref var currentMapRef = ref BattleSceneRoot.Instance.currentMapObject;

            if (currentMapRef.isCreature)
            {
                UnityObject.Destroy(currentMapRef.gameObject);
            }
            else
            {
                currentMapRef.EnableMap(false);

                _prevMap = currentMapRef;
            }

            currentMapRef = _map;

            BattleSceneRoot.Instance.mapList.Add(currentMapRef);

            if (!currentMapRef.IsMapInitialized)
            {
                currentMapRef.InitializeMap();

                _mapFilterRef(BattleSceneRoot.Instance).StartMapChangingEffect(Direction.LEFT, true);

                currentMapRef.gameObject.SetActive(true);

                if (!_originalMap)
                {
                    _dialogIdListRef((CreatureMapManager)currentMapRef).AddRange(_dialogs);

                    CreatureDlgManagerUI.Instance.Init(true);

                    currentMapRef.GetComponentsInChildren<Component>().Foreach(cpnt =>
                    {
                        if (cpnt is SpriteRenderer renderer)
                        {
                            if (renderer.name.Contains("BG"))
                            {
                                renderer.sprite = _bgSprite;
                            }
                            else if (renderer.name.Contains("Floor"))
                            {
                                renderer.sprite = _floorSprite;
                            }
                        }
                    });

                    BattleCamManager.Instance?.bgCam?.GetComponent<CameraFilterPack_TV_Vignetting>()?.enabled = false;
                }
            }

            if (_bgm is not null)
            {
                currentMapRef.mapBgm = [_bgm];
            }

            currentMapRef.EnableMap(true);
            currentMapRef.PlayMapChangedSound();

            BattleSoundManager.Instance.SetEnemyTheme(currentMapRef.mapBgm);
            BattleSoundManager.Instance.ChangeEnemyTheme(0);

            foreach (var unit in BattleObjectManager.instance.GetList())
            {
                unit.view.ChangeScale(currentMapRef.mapSize);
            }

            _map = null;
        }

        static AccessTools.FieldRef<BattleSceneRoot, MapChangeFilter> _mapFilterRef
            = typeof(BattleSceneRoot).FieldRefAccess<MapChangeFilter>("_mapChangeFilter");

        static AccessTools.FieldRef<CreatureMapManager, List<string>> _dialogIdListRef
            = typeof(CreatureMapManager).FieldRefAccess<List<string>>("_creatureDlgIdList");
    }

    [HarmonyPatch(typeof(BattleSceneRoot), "ClearFloorMap")]
    class PatchOnClearMap
    {
        static Exception Finalizer(Exception __exception)
        {
            UnsetMap();

            return __exception;
        }
    }

    [HarmonyPatch(typeof(WizardMapManager), "CreateDialog")]
    class PatchOnCreateDialog
    {
        static bool Prefix(ref int ____dlgIdx, CreatureDlgEffectUI ____dlgEffect)
        {
            if (_fixedMap && !_originalMap)
            {
                if (_dialogs.Count > 0)
                {
                    ____dlgIdx %= _dialogs.Count;
                }

                _mapObject?.GetComponent<CreatureMapManager>()?.CreateDialog(_dlgColor);

                return false;
            }

            return true;
        }
    }
}
