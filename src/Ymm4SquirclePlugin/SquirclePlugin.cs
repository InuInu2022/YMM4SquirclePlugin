using System.Reflection;

using YukkuriMovieMaker.Plugin;
using YukkuriMovieMaker.Plugin.Shape;
using YukkuriMovieMaker.Project;
namespace Ymm4SquirclePlugin;

[PluginDetails(AuthorName = "InuInu", ContentId = "")]
public class SquirclePlugin : IShapePlugin
{
	/// <summary>
	/// プラグインの名前
	/// </summary>
	public string Name => "スクワークル角丸";

	public PluginDetailsAttribute Details
		=> GetType().GetCustomAttribute<PluginDetailsAttribute>()
			?? new();

	/// <summary>
	/// 図形アイテムのexo出力に対応しているかどうか
	/// </summary>
	public bool IsExoShapeSupported => false;

	/// <summary>
	/// マスク系（図形切り抜きエフェクト、エフェクトアイテム）のexo出力に対応しているかどうか
	/// </summary>
	public bool IsExoMaskSupported => false;

	/// <summary>
	/// 図形パラメーターを作成する
	/// </summary>
	/// <param name="sharedData">共有データ。図形の種類を切り替えたときに元の設定項目を復元するために必要。</param>
	/// <returns>図形パラメータ</returns>
	public IShapeParameter CreateShapeParameter(SharedDataStore? sharedData)
	{
		return new SquircleParameter(sharedData);
	}
}
