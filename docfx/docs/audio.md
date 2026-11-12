# `using DeviceOfHermes.Resource;`

名前空間`DeviceOfHermes.Resource`に存在するクラスです。  
この記事はこれを前提に書かれています。

# Audio

音声ファイルを読み込んだりするクラスです。

```cs
// pathにあるファイルをAudioClipとして読み込む
AudioClip audio = Audio.ReadFromMp3(path);
```

## Specified usage

### `ReadFromMp3(string)`

```cs
AudioClip audio = Audio.ReadFromMp3(path);
```

mp3ファイルをAudioClipとして読み込みます。

- `path`: ファイルパスです。
