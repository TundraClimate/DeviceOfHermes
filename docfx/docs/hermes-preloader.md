# HermesPreloader

対象のDLLファイルをDeviceOfHermesのロード時にセットで読み込む仕組みです。  

MODフォルダ内、`Assemblies/HermesAssemblies/`フォルダに配置したDLLを対象とします。(`HermesAssemblies`が存在しない場合は作成してください)  
同じAssemblyNameのDLLが存在する場合、有効化されている全てのMODの`HermesAssemblies`の中から最新の`FileVersion`の対象DLLのみがロードされます。

対象になる全てのDLLがロードされた後、ロードしたDLLの中から`DeviceOfHermes.Boot.HermesInitializer`が実装されているクラスを探します。

見つかったクラスはそれぞれ実装されている`OnInitMod`メソッドを実行します。  

`HermesInitializer`実装例:  
```cs
internal class FooInit : DeviceOfHermes.Boot.HermesInitializer
{
    public void OnInitMod()
    {
        // ロード時にログを出力
        Hermes.Say("Load by FooMod!");
    }
}
```
