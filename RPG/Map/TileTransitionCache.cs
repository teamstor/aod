using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Game = TeamStor.Engine.Game;

namespace TeamStor.RPG
{
	/// <summary>
	/// Handles caching of generated tile transitions.
	/// </summary>
	public class TileTransitionCache : IDisposable
	{
		private Dictionary<string, BitArray> _masks = new Dictionary<string, BitArray>();
		
		private List<Texture2D> _textures = new List<Texture2D>();
		private Point _currentPointOnTexture = Point.Zero;

		private struct TileTransition
		{
			public int Texture;
			public Point Point;
		}
		
		private Dictionary<long, TileTransition> _cachedTransitions = new Dictionary<long, TileTransition>();

		private void GenerateTileTransition(Tile tile, SortedDictionary<string, string> metadata, Map.Environment environment)
		{
			string transitionBase = tile.TransitionTexture(metadata, environment);

            if(!transitionBase.Contains("_color"))
            {
                if(!_masks.ContainsKey(transitionBase))
                {
                    _masks.Add(transitionBase, new BitArray(16 * 16));

                    Color[] data = new Color[16 * 16];
                    Game.Assets.Get<Texture2D>(transitionBase).GetData(data);

                    for(int i = 0; i < data.Length; i++)
                        _masks[transitionBase][i] = data[i] == Color.Black;

                    Game.Assets.UnloadAsset(transitionBase);
                }
            }
			
			if(_textures.Count == 0 || _currentPointOnTexture.Y >= 16)
			{
				_textures.Add(new Texture2D(Game.GraphicsDevice, 256, 256));
				_currentPointOnTexture = new Point(0, 0);
			}
			
			TileTransition transition = new TileTransition();
			transition.Point = _currentPointOnTexture;
			transition.Texture = _textures.Count - 1;
			
			Rectangle textureRectangle = new Rectangle(
				tile.TextureSlot(metadata, environment).X * 16,
				tile.TextureSlot(metadata, environment).Y * 16,
				16,
				16);
			
			Color[] tileData = new Color[16 * 16];
			Tile.LayerToTexture(Game, tile.Layer).GetData(0, textureRectangle, tileData, 0, 16 * 16);
			
			Color[] tileTransition = new Color[16 * 16];
            if(transitionBase.Contains("_color"))
            {
                Game.Assets.Get<Texture2D>(transitionBase).GetData(tileTransition);
                Game.Assets.UnloadAsset(transitionBase);
            }
            else
            {
                for(int i = 0; i < tileTransition.Length; i++)
                    tileTransition[i] = _masks[transitionBase][i] ? tileData[i] : Color.Transparent;
            }
			_textures.Last().SetData(0, new Rectangle(_currentPointOnTexture.X * 16, _currentPointOnTexture.Y * 16, 16, 16), tileTransition, 0, 16 * 16);

			_currentPointOnTexture.X++;
			if(_currentPointOnTexture.X >= 16)
			{
				_currentPointOnTexture.X = 0;
				_currentPointOnTexture.Y++;
			}

			_cachedTransitions.Add(tile.UniqueIdentity(metadata, environment), transition);
			
			Console.WriteLine("Generated transition for " + tile.Name(metadata, environment) + " in environment " + environment + " (metadata: \"" + metadata + "\")");
		}
		
		public Game Game
		{
			get;
			private set;
		}

		public TileTransitionCache(Game game)
		{
			Game = game;
		}

		/// <param name="tile">The tile to get the transition for.</param>
		/// <param name="metadata">The metadata for the tile.</param>
		/// <param name="environment">The environment the tile is in.</param>
		/// <param name="pointOnTexture">The point on the texture the tile is on.</param>
		/// <returns>The texture for the specified tile.</returns>
		public Texture2D TextureForTile(Tile tile, SortedDictionary<string, string> metadata, Map.Environment environment, out Point pointOnTexture)
		{
			if(!HasTransition(tile, metadata, environment))
				GenerateTileTransition(tile, metadata, environment);
			
			TileTransition transition = _cachedTransitions[tile.UniqueIdentity(metadata, environment)];
			pointOnTexture = transition.Point;
			return _textures[transition.Texture];
		}
		
		/// <param name="tile">The tile to get the transition for.</param>
		/// <param name="metadata">The metadata for the tile.</param>
		/// <param name="environment">The environment the tile is in.</param>
		/// <returns>true if the cache has a transition for the specified tile</returns>
		public bool HasTransition(Tile tile, SortedDictionary<string, string> metadata, Map.Environment environment)
		{
			return _cachedTransitions.ContainsKey(tile.UniqueIdentity(metadata, environment));
		}
		
		/// <summary>
		/// Clears the cache.
		/// </summary>
		public void Clear()
		{
			foreach(Texture2D texture in _textures)
				texture.Dispose();
			
			_currentPointOnTexture = Point.Zero;

			_textures.Clear();
			_cachedTransitions.Clear();
			_masks.Clear();
		}

		public void Dispose()
		{
			Clear();
		}
	}
}