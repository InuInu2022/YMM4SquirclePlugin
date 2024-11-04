using System.ComponentModel.DataAnnotations;
using System.Windows.Media;

using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin.Brush;
using YukkuriMovieMaker.Plugin.Shape;
using YukkuriMovieMaker.Project;

namespace Ymm4SquirclePlugin;

internal partial class SquircleParameter(SharedDataStore? sharedData) : ShapeParameterBase(sharedData)
{
	[Display(GroupName = "", Name = "形状タイプ", Description = "形状の計算方式")]
	[EnumComboBox]
	public SquircleType SquircleType {
		get => _squircleType;
		set => Set(ref _squircleType, value);
	}
	SquircleType _squircleType = SquircleType.Superellipse;

	[Display(Name = "幅")]
	[AnimationSlider("F0", "px", 0, 100)]
	public Animation Width { get; } = new Animation(100, 0, 1000);

	[Display(Name = "高さ")]
	[AnimationSlider("F0", "px", 0, 100)]
	public Animation Height { get; } = new Animation(100, 0, 1000);

	[Display(Name = "曲率指数")]
	[AnimationSlider("F1", "", 0.0, 15.0)]
	public Animation Radius { get; } = new Animation(5.0, 0.0, 100.0);

	[Display(GroupName = "図形の模様", Name = "色")]
	[ColorPicker]
	public Color Color { get => color; set => Set(ref color, value); }
	Color color = Colors.White;

	/*
	[Display(GroupName = "図形の模様", AutoGenerateField = true)]
	public YukkuriMovieMaker.Plugin.Brush.Brush Brush { get; } = new();
	*/


	//YukkuriMovieMaker.ItemEditor.ICustomVisibilityAttribute

	//必ず引数なしのコンストラクタを定義してください。
	//これがないとプロジェクトファイルの読み込みに失敗します。
	public SquircleParameter()
		: this(null) { }

	/// <summary>
	/// マスクのExoFilterを生成する。
	/// </summary>
	/// <param name="keyFrameIndex">キーフレーム番号</param>
	/// <param name="desc">exo出力に必要な各種パラメーター</param>
	/// <param name="shapeMaskParameters">マスクのexo出力に必要な各種パラメーター</param>
	/// <returns>exoフィルタ</returns>
	public override IEnumerable<string> CreateMaskExoFilter(
		int keyFrameIndex,
		ExoOutputDescription desc,
		ShapeMaskExoOutputDescription shapeMaskParameters
	)
	{
		int fps = desc.VideoInfo.FPS;
		return
		[
			$"_name=マスク\r\n"
				+ $"_disable={(shapeMaskParameters.IsEnabled ? 0 : 1)}\r\n"
				+ $"X={shapeMaskParameters.X.ToExoString(keyFrameIndex, "F1", fps)}\r\n"
				+ $"Y={shapeMaskParameters.Y.ToExoString(keyFrameIndex, "F1", fps)}\r\n"
				+ $"回転={shapeMaskParameters.Rotation.ToExoString(keyFrameIndex, "F2", fps)}\r\n"
				+ $"サイズ=100\r\n"
				+ $"縦横比=0\r\n"
				+ $"ぼかし={shapeMaskParameters.Blur.ToExoString(keyFrameIndex, "F0", fps)}\r\n"
				+ $"マスクの反転={(shapeMaskParameters.IsInverted ? 1 : 0):F0}\r\n"
				+ $"元のサイズに合わせる=0\r\n"
				+ $"type=0\r\n"
				+ $"name=\r\n"
				+ $"mode=0\r\n"
		];
	}


	/// <summary>
	/// 図形アイテムのExoFilterを生成する。
	/// </summary>
	/// <param name="keyFrameIndex">キーフレーム番号</param>
	/// <param name="desc">exo出力に必要な各種パラメーター</param>
	/// <returns>exoフィルタ</returns>
	public override IEnumerable<string> CreateShapeItemExoFilter(
		int keyFrameIndex,
		ExoOutputDescription desc
	)
	{
		return [];
		/*
		var fps = desc.VideoInfo.FPS;
		return
		[
			$"_name=図形\r\n"
				+ $"サイズ={Size.ToExoString(keyFrameIndex, "F0", fps)}\r\n"
				+ $"縦横比=0\r\n"
				+ $"ライン幅=4000\r\n"
				+ $"type=0\r\n"
				+ $"color=FFFFFF\r\n"
				+ $"name=\r\n"
		];
		*/
	}


	/// <summary>
	/// 図形ソースを生成する。
	/// </summary>
	/// <param name="devices">デバイス</param>
	/// <returns>図形ソース</returns>
	public override IShapeSource CreateShapeSource(IGraphicsDevicesAndContext devices)
	{
		return new SquircleSource(devices, this);
	}

	/// <summary>
	/// このクラス内のIAnimatable一覧を返す。
	/// </summary>
	/// <returns>IAnimatable一覧</returns>
	protected override IEnumerable<IAnimatable> GetAnimatables()
		=> [Width, Height, Radius];

	/// <summary>
	/// 設定を一時的に保存する。
	/// 図形の種類を切り替えたときに元の設定項目を復元するために必要。
	/// </summary>
	/// <param name="store"></param>
	protected override void LoadSharedData(SharedDataStore store)
	{
		var sData = store.Load<SharedData>();
		if (sData is null)
			return;

		sData.CopyTo(this);
	}

	/// <summary>
	/// 設定を復元する。
	/// 図形の種類を切り替えたときに元の設定項目を復元するために必要。
	/// </summary>
	/// <param name="store"></param>
	protected override void SaveSharedData(SharedDataStore store)
	{
		store.Save(new SharedData(this));
	}

	/// <summary>
	/// 設定の一時保存用クラス
	/// </summary>
	class SharedData
	{
		//public Animation Size { get; } = new Animation(100, 0, 1000);
		public Animation Width { get; } = new Animation(100, 0, 1000);
		public Animation Height { get; } = new Animation(100, 0, 1000);
		public Animation Radius { get; } = new Animation(5.0, 0.0, 100.0);
		//public Brush Brush { get; } = new();

		public SharedData(SquircleParameter param)
		{
			//Size.CopyFrom(param.Size);
			Width.CopyFrom(param.Width);
			Height.CopyFrom(param.Height);
			Radius.CopyFrom(param.Radius);
			//Brush.CopyFrom(param.Brush);
		}

		public void CopyTo(SquircleParameter param)
		{
			//param.Size.CopyFrom(Size);
			param.Width.CopyFrom(Width);
			param.Height.CopyFrom(Height);
			param.Radius.CopyFrom(Radius);
			//param.Brush.CopyFrom(Brush);
		}
	}
}
