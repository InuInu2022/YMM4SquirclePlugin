/**
IPluginを継承する何もしないプラグイン

実際には manju-summoner/YukkuriMovieMaker4PluginSamples を
参考に適切なプラグインベースを継承して作ってください。

**/
using YukkuriMovieMaker.Plugin;

namespace Ymm4PluginSample;

//以下のサンプルを参考に作ってください
//[manju-summoner/YukkuriMovieMaker4PluginSamples: YMM4用プラグインのサンプル集です](https://github.com/manju-summoner/YukkuriMovieMaker4PluginSamples)

[PluginDetails(AuthorName = "", ContentId = "")]
public class Sample : IPlugin
{
    public string Name => "プラグインテンプレート";
}