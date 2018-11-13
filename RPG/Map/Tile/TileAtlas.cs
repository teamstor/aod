using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamStor.Engine;

using Game = TeamStor.Engine.Game;

namespace TeamStor.RPG
{
    /// <summary>
    /// Handles caching and creation of tile atlases.
    /// </summary>
    public class TileAtlas : IDisposable
    {
        // Standard atlas size.
        public const int ATLAS_SIZE = 1024;

        private Point _currentPoint;
        private int _minHeightThisRow;

        public struct Region
        {
            public Texture2D Texture;
            public Rectangle Rectangle;
        }

        public Region MissingRegion;

        private Dictionary<string, Region> _tiles = new Dictionary<string, Region>();
        private List<Texture2D> _atlases = new List<Texture2D>();

        public Game Game
        {
            get;
            private set;
        }

        public TileAtlas(Game game)
        {
            Game = game;
            Game.Assets.AssetChanged += OnAssetChanged;

            MissingRegion.Texture = game.Assets.Get<Texture2D>("tiles/missing.png", true);
            MissingRegion.Rectangle = new Rectangle(0, 0, 16, 16);

            _atlases.Add(new Texture2D(game.GraphicsDevice, ATLAS_SIZE, ATLAS_SIZE));
        }

        private void OnAssetChanged(object sender, AssetsManager.AssetChangedEventArgs e)
        {
            if(_tiles.ContainsKey(e.Name))
                Clear();
        }

        public Region this[string texture]
        {
            get
            {
                Region region;
                if(_tiles.TryGetValue(texture, out region))
                    return region;

                Texture2D t;
                if(Game.Assets.TryLoadAsset(texture, out t))
                {
                    if(t.Width > ATLAS_SIZE || t.Height > ATLAS_SIZE)
                    {
                        region.Rectangle = t.Bounds;
                        region.Texture = t;
                        return region;
                    }

                    if(_currentPoint.X + t.Width > ATLAS_SIZE)
                    {
                        _currentPoint.X = 0;
                        _currentPoint.Y += _minHeightThisRow;
                        _minHeightThisRow = 0;
                    }

                    if(_currentPoint.Y + t.Height > ATLAS_SIZE)
                    {
                        _currentPoint = new Point(0, 0);
                        _minHeightThisRow = 0;
                        _atlases.Add(new Texture2D(Game.GraphicsDevice, ATLAS_SIZE, ATLAS_SIZE));
                    }

                    Color[] tileData = new Color[t.Width * t.Height];
                    t.GetData(0, t.Bounds, tileData, 0, t.Width * t.Height);

                    region.Texture = _atlases.Last();
                    region.Rectangle = new Rectangle(_currentPoint, new Point(t.Width, t.Height));

                    _atlases.Last().SetData(0, region.Rectangle, tileData, 0, t.Width * t.Height);

                    Console.WriteLine("Added " + texture + " to atlas " + _atlases.Count + " @ " + region.Rectangle);

                    _minHeightThisRow = Math.Max(_minHeightThisRow, t.Height);
                    _currentPoint.X += t.Width;

                    Game.Assets.UnloadAsset(texture);
                    _tiles.Add(texture, region);
                    return region;
                }

                return MissingRegion;
            }
        }

        /// <summary>
        /// Clears all atlases.
        /// </summary>
        public void Clear()
        {
            _tiles.Clear();

            foreach(Texture2D t in _atlases)
                t.Dispose();

            _atlases.Clear();
            _atlases.Add(new Texture2D(Game.GraphicsDevice, ATLAS_SIZE, ATLAS_SIZE));

            _currentPoint = new Point(0, 0);
            _minHeightThisRow = 0;
        }

        public void Dispose()
        {
            Clear();
            Game.Assets.AssetChanged -= OnAssetChanged;
        }
    }
}
