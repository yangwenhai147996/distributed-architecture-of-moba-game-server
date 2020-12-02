namespace PurificationPioneer.Utility
{
	/// <summary>
	/// 这个类提供了Resources下文件名和路径字典访问方式，同名资源可能引起bug
	/// </summary>
	public partial class AssetConstUtil : ReadyGamerOne.MemorySystem.AssetConstUtil<AssetConstUtil>
	{
		private System.Collections.Generic.Dictionary<string,string> nameToPath 
			= new System.Collections.Generic.Dictionary<string,string>{
					{ @"DebugPanel" , @"ClassPanel\DebugPanel" },
					{ @"HomePanel" , @"ClassPanel\HomePanel" },
					{ @"MatchPanel" , @"ClassPanel\MatchPanel" },
					{ @"WelcomePanel" , @"ClassPanel\WelcomePanel" },
					{ @"HeroIcon0" , @"ClassSprite\ClassHeroIcon\HeroIcon0" },
					{ @"HeroIcon1" , @"ClassSprite\ClassHeroIcon\HeroIcon1" },
					{ @"HeroIcon2" , @"ClassSprite\ClassHeroIcon\HeroIcon2" },
					{ @"Avatar0" , @"ClassSprite\ClassUserIcon\Avatar0" },
					{ @"Avatar1" , @"ClassSprite\ClassUserIcon\Avatar1" },
					{ @"Avatar2" , @"ClassSprite\ClassUserIcon\Avatar2" },
					{ @"Avatar3" , @"ClassSprite\ClassUserIcon\Avatar3" },
					{ @"Avatar4" , @"ClassSprite\ClassUserIcon\Avatar4" },
					{ @"Avatar5" , @"ClassSprite\ClassUserIcon\Avatar5" },
					{ @"Avatar6" , @"ClassSprite\ClassUserIcon\Avatar6" },
					{ @"Avatar7" , @"ClassSprite\ClassUserIcon\Avatar7" },
					{ @"Avatar8" , @"ClassSprite\ClassUserIcon\Avatar8" },
					{ @"HeroOption" , @"ClassUi\HeroOption" },
					{ @"MatcherRect" , @"ClassUi\MatcherRect" },
				};
		public override System.Collections.Generic.Dictionary<string,string> NameToPath => nameToPath;
	}
}