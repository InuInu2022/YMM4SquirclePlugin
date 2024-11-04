using System.Diagnostics;
using System.Numerics;
using System.Windows.Media;

using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace Ymm4SquirclePlugin;

internal partial class SquircleSource : IShapeSource
{
	readonly IGraphicsDevicesAndContext _devices;
	readonly SquircleParameter _squircleParameter;
	readonly DisposeCollector disposer = new();

	double _width;
	double _height;
	double _radius;
	//YukkuriMovieMaker.Plugin.Brush.Brush _brush;

	readonly ID2D1SolidColorBrush _whiteBrush;
	ID2D1CommandList? _commandList;
	ID2D1PathGeometry? _geometry;
	bool isFirst = true;
	SquircleType _lastType = SquircleType.Superellipse;
	private Color _color;
	private ID2D1SolidColorBrush _dxColor;

	/// <summary>
	/// 描画結果
	/// </summary>
	public ID2D1Image Output =>
		_commandList
		?? throw new InvalidOperationException(
			$"{nameof(_commandList)}がnullです。事前にUpdateを呼び出す必要があります。"
		);

	public SquircleSource(
		IGraphicsDevicesAndContext devices,
		SquircleParameter sampleShapeParameter
	)
	{
		this._devices = devices;
		this._squircleParameter = sampleShapeParameter;
		var color = _squircleParameter.Color;

		_whiteBrush = devices.DeviceContext.CreateSolidColorBrush(
			new Vortice.Mathematics.Color4(
				color.R / 255f,
				color.G / 255f,
				color.B / 255f,
				color.A / 255f
			)
		);
		disposer.Collect(_whiteBrush);
	}

	/// <summary>
	/// 図形を更新する
	/// </summary>
	/// <param name="timelineItemSourceDescription"></param>
	public void Update(TimelineItemSourceDescription timelineItemSourceDescription)
	{
		var fps = timelineItemSourceDescription.FPS;
		var frame = timelineItemSourceDescription.ItemPosition.Frame;
		var length = timelineItemSourceDescription.ItemDuration.Frame;

		var width = _squircleParameter.Width.GetValue(frame, length, fps);
		var height = _squircleParameter.Height.GetValue(frame, length, fps);
		var radius = _squircleParameter.Radius.GetValue(frame, length, fps);
		var type = _squircleParameter.SquircleType;
		var color = _squircleParameter.Color;

		//変わっていない場合は何もしない
		if (
			!isFirst
			&& _lastType == type
			&& _commandList is not null
			&& _width == width
			&& _height == height
			&& _radius == radius
			&& _color == color
		)
		{
			return;
		}

		var dc = _devices.DeviceContext;

		if (_geometry is not null)
		{
			disposer.RemoveAndDispose(ref _geometry);
		}
		_geometry = _devices.D2D.Factory.CreatePathGeometry();
		disposer.Collect(_geometry);

		//前回のUpdateで作成したコマンドリストを破棄して新しいコマンドリストを作成する
		if (_commandList is not null)
		{
			disposer.RemoveAndDispose(ref _commandList);
		}
		_commandList = _devices.DeviceContext.CreateCommandList();
		disposer.Collect(_commandList);

		if (_dxColor is not null)
		{
			disposer.RemoveAndDispose(ref _dxColor!);
		}
		_dxColor = dc.CreateSolidColorBrush(
			new Vortice.Mathematics.Color4(
				color.R / 255f,
				color.G / 255f,
				color.B / 255f,
				color.A / 255f
			)
		);
		disposer.Collect(_dxColor);

		dc.Target = _commandList;
		dc.BeginDraw();
		dc.Clear(clearColor: null);
		switch (_squircleParameter.SquircleType)
		{
			case SquircleType.Superellipse:
				DrawSuperellipseSquircle(dc, (float)width / 2, (float)height / 2, (float)radius);
				break;
			case SquircleType.Complex:
				DrawComplexSquircle(dc, (float)width, (float)height, (float)radius);
				break;
			case SquircleType.FernandezGuasti:
				DrawFernandezGuastiSquircle(dc, (float)width / 2, (float)height / 2, (float)radius);
				break;
			default:
				break;
		}
		dc.EndDraw();
		dc.Target = null; //Targetは必ずnullに戻す。
		_commandList.Close(); //CommandListはEndDraw()の後に必ずClose()を呼んで閉じる必要がある

		_width = width;
		_height = height;
		_radius = radius;
		_lastType = type;
		_color = color;

		//キャッシュ用の情報を保存しておく
		isFirst = false;
	}

	void DrawSuperellipseSquircle(ID2D1DeviceContext6 context, float a, float b, float n)
	{
		const int pointCount = 1000;
		var points = new Vector2[pointCount];

		const float angleIncrement = (float)(2 * Math.PI / pointCount);
		float powerFactor = 2 / n; // 事前計算

		// 並列ループを使用して計算
		Parallel.For(
			0,
			pointCount,
			i =>
			{
				float t = i * angleIncrement;
				var cosT = MathF.Cos(t);
				var sinT = MathF.Sin(t);

				var x = a * MathF.Sign(cosT) * MathF.Pow(MathF.Abs(cosT), powerFactor);
				var y = b * MathF.Sign(sinT) * MathF.Pow(MathF.Abs(sinT), powerFactor);

				points[i] = new Vector2(x, y);
			}
		);

		DrawPath(context, points);
	}

	// 複素数ベースのスクワークル
	[System.Diagnostics.CodeAnalysis.SuppressMessage(
		"Performance",
		"HLQ013",
		Justification = "<保留中>"
	)]
	void DrawComplexSquircle(ID2D1DeviceContext6 context, float a, float b, float n)
	{
		const int pointCount = 1000;
		var points = new Vector2[pointCount];
		float maxReal = 0;
		float maxImaginary = 0;

		for (int i = 0; i < pointCount; i++)
		{
			float t = (float)(i * (2 * Math.PI / pointCount));
			float cosT = MathF.Cos(t);
			float sinT = MathF.Sin(t);

			var z = new Complex(
				MathF.Sign(cosT) * MathF.Pow(MathF.Abs(cosT), 2 / n),
				MathF.Sign(sinT) * MathF.Pow(MathF.Abs(sinT), 2 / n)
			);

			// 実部と虚部の最大値を追跡
			maxReal = MathF.Max(maxReal, MathF.Abs((float)z.Real));
			maxImaginary = MathF.Max(maxImaginary, MathF.Abs((float)z.Imaginary));

			points[i] = new Vector2((float)z.Real, (float)z.Imaginary);
		}

		// 最大実部と最大虚部に基づいてスケーリングを適用
		for (int i = 0; i < pointCount; i++)
		{
			points[i] = new Vector2(
				points[i].X * (a / (2 * maxReal)),
				points[i].Y * (b / (2 * maxImaginary))
			);
		}

		DrawPath(context, points);
	}

	void DrawFernandezGuastiSquircle(ID2D1DeviceContext6 context, float a, float b, float roundness)
	{
		const int pointCount = 1000;
		var points = new Vector2[pointCount];

		for (int i = 0; i < pointCount; i++)
		{
			float t = (float)(i * (2 * Math.PI / pointCount));

			// 角の丸さを反映させる計算
			float cosT = MathF.Cos(t);
			float sinT = MathF.Sin(t);

			float x = a * MathF.Sign(cosT) * MathF.Pow(MathF.Abs(cosT), 4 / (4 + roundness));
			float y = b * MathF.Sign(sinT) * MathF.Pow(MathF.Abs(sinT), 4 / (4 + roundness));

			points[i] = new Vector2(x, y);
		}

		DrawPath(context, points);
	}

	// 描画のためのメソッド
	void DrawPath(ID2D1DeviceContext6 context, Vector2[] points)
	{
		// PathGeometryを使用してスクワークルの形状を作成
		if (_geometry is null)
			return;

		using (var sink = _geometry.Open())
		{
			sink.BeginFigure(points[0], FigureBegin.Filled);
			sink.AddLines(points[1..]);
			sink.EndFigure(FigureEnd.Closed);
			sink.Close();
		}

		// パスを描画
		context.FillGeometry(_geometry, _dxColor ?? _whiteBrush);
	}

	#region IDisposable
	private bool disposedValue;


	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
				// マネージド状態を破棄します (マネージド オブジェクト)
				disposer.DisposeAndClear();
			}

			// アンマネージド リソース (アンマネージド オブジェクト) を解放し、ファイナライザーをオーバーライドします
			// 大きなフィールドを null に設定します
			disposedValue = true;
		}
	}

	// // 'Dispose(bool disposing)' にアンマネージド リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします
	// ~SampleShapeSource()
	// {
	//     // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
	//     Dispose(disposing: false);
	// }

	public void Dispose()
	{
		// このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
	#endregion
}
