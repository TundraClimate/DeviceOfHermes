using UnityEngine;

namespace LimbufOfHermes;

/// <summary>A unit buf base</summary>
public class LimbufBase : AdvancedUnitBuf
{
    /// <summary>Change buf stack</summary>
    public virtual void ChangeStack(Func<int, int> f)
    {
        var bef = this.stack;

        this.stack = f(bef);

        if (this.stack > bef)
        {
            this.OnAddBuf(this.stack - bef);
        }

        if (0 >= this.stack)
        {
            this.Destroy();
        }
    }

    /// <summary>On buf activate</summary>
    public virtual void OnActivate(int stack)
    {
    }

    internal static AssetBundle bundle = AssetBundle.LoadFromStream(typeof(LimbufBase).Assembly.GetManifestResourceStream("LimbufOfHermes.public.limbuf.assetbundle"));
}
