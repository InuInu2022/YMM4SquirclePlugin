using System.ComponentModel.DataAnnotations;

namespace Ymm4SquirclePlugin;

public enum SquircleType
{
	/// <summary>
	/// スーパー楕円(Superellipse)
	/// </summary>
	[Display(Name = "スーパー楕円", Description = "スーパー楕円(Superellipse)ベースのスクワークル角丸")]
	Superellipse,

	/// <summary>
	/// 複素数
	/// </summary>
	[Display(Name = "複素数", Description = "複素数方式ベースのスクワークル角丸")]
	Complex,

	[Display(Name = "Fernández–Guasti", Description = "Fernández–Guastiさん考式のスクワークル角丸")]
	FernandezGuasti,
}
