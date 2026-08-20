using System.Collections;
using UnityEngine;
using Sound;

namespace LimbufOfHermes;

/// <summary>A unit buf the Tremor</summary>
public class BattleUnitBuf_Limbuf_TremorBurst : LimbufBase
{
    /// <summary>Impl keywordId</summary>
    protected override string keywordId => "LimbufOfHermes_TremorBurst";

    /// <summary>Impl bufType</summary>
    public override KeywordBuf bufType => LimKeywordBuf.TremorBurst;

    /// <summary>Impl IsInstant</summary>
    public override bool IsInstant => true;

    /// <summary>Impl OnInstant</summary>
    public override void OnInstant()
    {
        if (StageController.Instance.IsLogState())
        {
            base._owner.AddRencounterEvent(RencounterEvent.PrintEffect, () => OnActivate(this.stack));
        }
        else
        {
            OnActivate(this.stack);
        }
    }

    /// <summary>Impl OnActivate</summary>
    public override void OnActivate(int stack)
    {
        TremorBurstEffect.ApplyUnit(base._owner, this);
    }
}

internal class TremorBurstEffect : MonoBehaviour
{
    public static void ApplyUnit(BattleUnitModel owner, BattleUnitBuf self)
    {
        owner.CreateTextEffect(
            TextDataModel.GetText("LimbufOfHermes_TremorBurst"),
            self.GetBufIcon(),
            AttackEffectManager.Instance.damageRwbpTextColor[2],
            new Color32(200, 200, 0, 255)
        );

        SoundEffectManager.Instance.PlayClip("creature/quitegirl_hit", false, 10f, null).source.pitch = 3.2f;
    }

    static IEnumerator RingingRoutine(GameObject target)
    {
        yield return null;

        target.AddChildObject("Burst", "Effect").AddComponent<TremorBurstEffect>().Also(ring =>
        {
            ring.maxRadius = 1.0f;
        }).transform.localPosition = new Vector3(0, 2, 0);

        yield return new WaitForSeconds(0.1f);

        target.AddChildObject("Burst", "Effect").AddComponent<TremorBurstEffect>().Also(ring =>
        {
            ring.startRadius = 0.5f;
            ring.width = 0.2f;
            ring.color = new Color(1.2f, 0.85f, 0.25f, 1.0f);
        }).transform.localPosition = new Vector3(0, 2, 0);

        yield return new WaitForSeconds(0.1f);

        target.AddChildObject("Burst", "Effect").AddComponent<TremorBurstEffect>().Also(ring =>
        {
            ring.startRadius = 0.5f;
            ring.width = 0.2f;
        }).transform.localPosition = new Vector3(0, 2, 0);
    }

    private float startRadius = 0.1f;
    private float maxRadius = 2.0f;
    private float duration = 0.1f;
    private float width = 0.1f;
    private int segments = 64;
    private Color color = new Color(1.0f, 0.65f, 0.05f, 1.0f);

    private Mesh? mesh;
    private Vector3[]? vertices;

    private float elapsed;

    private void Start()
    {
        CreateRing();
    }

    private void Update()
    {
        elapsed += Time.deltaTime;

        float t = Mathf.Clamp01(elapsed / duration);

        float radius = Mathf.Lerp(
            startRadius,
            maxRadius,
            t
        );

        UpdateRing(radius);

        if (elapsed >= duration)
        {
            Destroy(gameObject);
        }
    }

    private void CreateRing()
    {
        mesh = new Mesh
        {
            name = "TremorBurst"
        };

        vertices = new Vector3[segments * 2];
        int[] triangles = new int[segments * 6];

        for (int i = 0; i < segments; i++)
        {
            int next = (i + 1) % segments;

            int v0 = i * 2;
            int v1 = i * 2 + 1;
            int v2 = next * 2;
            int v3 = next * 2 + 1;

            int t = i * 6;

            triangles[t] = v0;
            triangles[t + 1] = v2;
            triangles[t + 2] = v1;

            triangles[t + 3] = v1;
            triangles[t + 4] = v2;
            triangles[t + 5] = v3;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();

        var meshFilter = gameObject.AddComponent<MeshFilter>();
        var meshRenderer = gameObject.AddComponent<MeshRenderer>();

        meshFilter.mesh = mesh;

        var material = new Material(Shader.Find("Standard"));

        material.color = color;

        material.SetFloat("_Metallic", 0.2f);
        material.SetFloat("_Glossiness", 0.1f);

        meshRenderer.material = material;
    }

    private void UpdateRing(float radius)
    {
        float outerRadius = radius + width * 0.5f;
        float innerRadius = Mathf.Max(0, radius - width * 0.5f);

        for (int i = 0; i < segments; i++)
        {
            float angle = 2.0f * Mathf.PI * i / segments;

            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);

            vertices?[i * 2] = new Vector3(
                cos * outerRadius,
                sin * outerRadius,
                0
            );

            vertices?[i * 2 + 1] = new Vector3(
                cos * innerRadius,
                sin * innerRadius,
                0
            );
        }

        mesh?.vertices = vertices;

        var normals = new Vector3[vertices?.Length ?? -1];

        for (int i = 0; i < normals.Length; i++)
        {
            normals[i] = Vector3.forward;
        }

        mesh?.normals = normals;

        mesh?.RecalculateBounds();
    }
}
