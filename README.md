# ymme-vscode-boilerplate

YMM4 plugin boilerplate template repository

- githubのtemplate repositoryで使えるリポジトリです。
 	- [テンプレートリポジトリを作成する - GitHub Docs](https://docs.github.com/ja/repositories/creating-and-managing-repositories/creating-a-template-repository)
- VSCode用ですが、タスクやデバッガの設定以外はVSやRiderなどでも使えます

## このテンプレートでできること

### 設定済み

- `src/Sample`以下に何もしないサンプルプラグインが設定済みです
  - これをもとにしても、公式のサンプルを元にしてもOK
  - ソリューションファイル：`Sample.sln`も設定済みです
- `dotnet publish`コマンドでymmeファイルを`publish`以下に作成＆YMM4のプラグインフォルダ以下に展開できます
- Code Analyzerを色々有効にしています
- ビルド時に `LICENSE` や `README.md` がdllやymmeに含まれるように設定済
- `licenses`フォルダ以下のファイルもdllやymmeに含まれるように
- [MinVer](https://github.com/adamralph/minver)ライブラリでgitのタグから自動でSemVerが付きます
- VSCode: VSCodeの`publish`タスクでも上記ができるようにしています
- VSCode: プラグインをYMM4ごとデバッガ接続で起動することができます
- gitignore, editorconfig設定済み

### 設定が必要なこと

- **環境変数：`YMM4_PATH`に「開発用YMM4」の置いてあるフォルダへのパスを通してください**
  - ※普段使い用YMM4とは別にするのをオススメ
- `LICENSE`：MITライセンスになっているのでお好きなライセンスに書き換えて下さい
- nuget以外のライブラリ：`lib`以下において参照してください
- ドキュメント: `docs`フォルダを用意しています
- ユニットテスト: `tests`フォルダだけ用意しています
  - お好きなテストフレームワークをお使いください

## Reference

YMM4のプラグイン開発は以下を参照してください。

- [プラグインを作成する | 饅頭遣いのおもちゃ箱](https://manjubox.net/ymm4/faq/plugin/how_to_make/)
- [manju-summoner/YukkuriMovieMaker4PluginSamples: YMM4用プラグインのサンプル集です](https://github.com/manju-summoner/YukkuriMovieMaker4PluginSamples)
- [manju-summoner/YukkuriMovieMaker.Plugin.Community: YMM4コミュニティで開発するYMM4プラグインです。 プラグインのサンプルも兼ねています。 ここで開発されたプラグインはYMM4にデフォルトで組み込まれます。](https://github.com/manju-summoner/YukkuriMovieMaker.Plugin.Community)
- [manju-summoner/SampleReversePlaybackAudioEffectPlugin: キャッシュ機構を備えたYMM4音声エフェクトの実装サンプルです。音声の逆再生機能を提供します。ただし、仮実装のため実用には適しません。](https://github.com/manju-summoner/SampleReversePlaybackAudioEffectPlugin)
- [manju-summoner/YukkuriMovieMaker.Generator](https://github.com/manju-summoner/YukkuriMovieMaker.Generator)