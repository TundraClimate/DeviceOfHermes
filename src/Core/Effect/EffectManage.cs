using UnityEngine;

namespace DeviceOfHermes.Effect;

/// <summary>A behaviour the manages effect</summary>
public class EffectManage : MonoBehaviour
{
    void FixedUpdate()
    {
        var nLayer = layerUpdate();

        if (_cLayer != nLayer)
        {
            _cLayer = nLayer;

            gameObject.GetComponentsInChildren<Transform>().Filter(i => i.gameObject != gameObject)
                .Foreach(i => i.gameObject.layer = nLayer);
        }

        if (isDestroySelf())
        {
            UnityObject.Destroy(gameObject);
        }
    }

    /// <summary>The function update layer</summary>
    public Func<int> layerUpdate = () => 0;

    /// <summary>The function is destroy self</summary>
    public Func<bool> isDestroySelf = () => false;

    int _cLayer;
}
