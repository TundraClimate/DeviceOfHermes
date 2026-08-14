using UnityEngine;

namespace DeviceOfHermes;

internal class SpeedDiceBufUI : MonoBehaviour
{
    public void Init(int idx, SpeedDiceBuf buf)
    {
        this.index = idx;
        this.buf = buf;
    }

    void Start()
    {
        gameObject.SetImage(buf.GetBufIcon())
            .Also(img =>
            {
                img.color = new Color(1f, 1f, 1f, 0.7f);
                img.rectTransform.localScale = new Vector3(0.3f, 0.3f, 1f);
            });
    }

    internal int index;

    internal SpeedDiceBuf? buf;
}
