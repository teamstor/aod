using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamStor.AOD.Menu;
using TeamStor.Engine;
using TeamStor.Engine.Coroutine;
using TeamStor.Engine.Graphics;
using SpriteBatch = TeamStor.Engine.Graphics.SpriteBatch;

namespace TeamStor.AOD
{
	/// <summary>
	/// Preloads assets such as textures and maps.
	/// </summary>
	public class PreloaderState : GameState
	{
		public enum PreloaderProgress
		{
			Tiles,
			Maps
		}

		/// <summary>
		/// Progress the preloader has made.
		/// Maps > Tiles
		/// </summary>
		public PreloaderProgress Progress { get; private set; } = PreloaderProgress.Tiles;
		
		/// <summary>
		/// All maps that were preloaded by the preloader.
		/// </summary>
		public static Dictionary<string, Map> PreloadedMaps { get; private set; } = new Dictionary<string, Map>();
		
		/// <summary>
		/// The latest asset that was preloaded.
		/// </summary>
		public string LatestAsset { get; private set; } = "";
		
		public override void OnEnter(GameState previousState)
		{
			Coroutine.Start(LoadCoroutine);
		}

		public override void OnLeave(GameState nextState)
		{
		}

		private bool _mapsLoaded;

		private void MapThread()
		{
			foreach(string map in Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory + "data/maps", "*.json", SearchOption.AllDirectories))
			{
				PreloadedMaps.Add(map, Map.Load(map));
				LatestAsset = map;
				
				Thread.Sleep(50);
			}
			
			_mapsLoaded = true;
		}

		private IEnumerator<ICoroutineOperation> LoadCoroutine()
		{
			for(int i = 0; i < 5; i++)
				yield return null;
			
			foreach(string asset in Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory + "data/tiles", "*.png", SearchOption.AllDirectories))
			{
				Texture2D a;
				Assets.TryLoadAsset(asset.
						Replace(AppDomain.CurrentDomain.BaseDirectory + "data/", "").
						Replace(AppDomain.CurrentDomain.BaseDirectory + "data\\", ""), 
					out a, 
					true);
										
				LatestAsset = asset;
			}
			
			yield return null;

			foreach(string asset in Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory + "data/fonts", "*.ttf", SearchOption.AllDirectories))
			{
				Font a;
				Assets.TryLoadAsset(asset.
						Replace(AppDomain.CurrentDomain.BaseDirectory + "data/", "").
						Replace(AppDomain.CurrentDomain.BaseDirectory + "data\\", ""), 
					out a, 
					true);
					
				LatestAsset = asset;
			}

			Progress = PreloaderProgress.Maps;
			Task.Run(() => MapThread());

			while(!_mapsLoaded)
				yield return null;
			
			Game.CurrentState = new MenuState();
		}

		public override void Update(double deltaTime, double totalTime, long count)
		{
		}

		public override void FixedUpdate(long count)
		{
		}

		public override void Draw(SpriteBatch batch, Vector2 screenSize)
		{
			screenSize = Program.ScaleBatch(batch);		
			batch.Rectangle(new Rectangle(0, 0, (int)screenSize.X, (int)screenSize.Y), Color.Black);

			Font font = Assets.Get<Font>("fonts/Alkhemikal.ttf");
			string loadingText = "Loading " + (Progress == PreloaderProgress.Maps ? "maps" : "tiles") + "...";
			batch.Text(font, 16, loadingText, screenSize / 2 - font.Measure(16, loadingText) / 2, Color.White * 0.6f);
			
			string text = "........................";

			for(int i = 0; i < 24; i++)
			{
				float fade = 0.6f * (float)(Math.Sin((Game.Time - i * 0.15f) * 10) + 1) * 0.5f;
				fade = (int)(fade * 10) / 10f;
                
				batch.Text(font, 16, ".", screenSize / 2 - new Vector2(text.Length * 4, 0) + new Vector2(8 * i, 0),
					Color.Lerp(Color.White * (0.2f + fade), Color.LightBlue * (0.2f + fade), fade) * 0.6f);
			}			
			
			Program.BlackBorders(batch);
		}
	}
}