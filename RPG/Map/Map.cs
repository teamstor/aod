using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Game = TeamStor.Engine.Game;

namespace TeamStor.RPG
{
    /// <summary>
    /// Map data.
    /// </summary>
    public class Map
    {
        /// <summary>
        /// Map save version.
        /// </summary>
        public const int MAP_FORMAT_VERSION = 2;
        
        /// <summary>
        /// Map environment.
        /// </summary>
        public enum Environment : byte
        {
            Forest = 0,
            SnowMountain = 1,
            Swamp = 2,
            Lava = 3,

            Inside = 10
        }

        /// <summary>
        /// Map weather.
        /// </summary>
        public enum Weather : byte
        {
            Sunny = 0,
            Rainy = 1,
            Snowy = 2
        }

        /// <summary>
        /// Info about the map such as the name and environment.
        /// </summary>
        public class Information
        {
            public Information(string name, Environment environment, Weather weather)
            {
                Name = name;
                Environment = environment;
            }

            /// <summary>
            /// Name of the map that will be shown when you enter it.
            /// </summary>
            public string Name;

            /// <summary>
            /// Environment the map is in.
            /// </summary>
            public Environment Environment;

            /// <summary>
            /// Weather of the map.
            /// </summary>
            public Weather Weather;
        }

        // (ID 16 bit) | (Metadata 16 bit)
        private uint[] _layerTerrain, _layerDecoration, _layerNPC, _layerControl;
        private Tile[] _idmapTerrain, _idmapDecoration, _idmapNPC, _idmapControl;
        private List<SortedDictionary<string, string>> _metadataTerrain, _metadataDecoration, _metadataNPC, _metadataControl;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private uint[] LayerToTileArray(Tile.MapLayer layer)
        {
            switch(layer)
            {
                case Tile.MapLayer.Terrain:
                    return _layerTerrain;

                case Tile.MapLayer.Decoration:
                    return _layerDecoration;

                case Tile.MapLayer.NPC:
                    return _layerNPC;

                case Tile.MapLayer.Control:
                    return _layerControl;
            }

            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Tile[] LayerToIDMap(Tile.MapLayer layer)
        {
            switch(layer)
            {
                case Tile.MapLayer.Terrain:
                    return _idmapTerrain;

                case Tile.MapLayer.Decoration:
                    return _idmapDecoration;

                case Tile.MapLayer.NPC:
                    return _idmapNPC;

                case Tile.MapLayer.Control:
                    return _idmapControl;
            }

            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private List<SortedDictionary<string, string>> LayerToMetadataArray(Tile.MapLayer layer)
        {
            switch(layer)
            {
                case Tile.MapLayer.Terrain:
                    return _metadataTerrain;

                case Tile.MapLayer.Decoration:
                    return _metadataDecoration;

                case Tile.MapLayer.NPC:
                    return _metadataNPC;

                case Tile.MapLayer.Control:
                    return _metadataControl;
            }

            return null;
        }

        /// <summary>
        /// The width of the map in tiles.
        /// </summary>
        public int Width
        {
            get;
            private set;
        }

        /// <summary>
        /// The height of the map in tiles.
        /// </summary>
        public int Height
        {
            get;
            private set;
        }

        /// <summary>
        /// Static info about the map.
        /// </summary>
        public Information Info;

        public Map(int w, int h, Information info)
        {
            Width = w;
            Height = h;

            Info = info;

            _layerTerrain = new uint[w * h];
            _layerDecoration = new uint[w * h];
            _layerNPC = new uint[w * h];
            _layerControl = new uint[w * h];

            _idmapTerrain = new Tile[ushort.MaxValue];
            _idmapDecoration = new Tile[ushort.MaxValue];
            _idmapNPC = new Tile[ushort.MaxValue];
            _idmapControl = new Tile[ushort.MaxValue];

            foreach(Tile.MapLayer layer in Tile.CachedAllMapLayers)
                LayerToIDMap(layer)[0] = Tile.Find("", layer);

            _metadataTerrain = new List<SortedDictionary<string, string>>();
            _metadataDecoration = new List<SortedDictionary<string, string>>();
            _metadataNPC = new List<SortedDictionary<string, string>>();
            _metadataControl = new List<SortedDictionary<string, string>>();
        }

        public Tile this[Tile.MapLayer layer, int x, int y]
        {
            get
            {
                // & 0xFFFF = lower 16 bits
                return LayerToIDMap(layer)[(ushort)(LayerToTileArray(layer)[(y * Width) + x] & 0xFFFF)];
            }
            set
            {
                SetMetadata(layer, x, y, null);

                /*Tile oldTile = this[layer, x, y];

                bool oldTileExists = oldTile == value;
                if(!oldTileExists)
                {
                    int ix, iy;
                    for(ix = 0; ix < Width; ix++)
                    {
                        for(iy = 0; iy < Height; iy++)
                        {
                            if(ix != x || iy != y)
                            {
                                if(this[layer, ix, iy] == oldTile)
                                {
                                    oldTileExists = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                if(!oldTileExists)
                    LayerToIDMap(layer).Remove((ushort)(LayerToTileArray(layer)[(y * Width) + x] & 0xFFFF)); */
                // TODO: behövs det här???

                int newValue = Array.IndexOf(LayerToIDMap(layer), value);
                if(newValue == -1)
                {
                    for(int i = 0; i < ushort.MaxValue; i++)
                    {
                        if(LayerToIDMap(layer)[i] == null)
                        {
                            newValue = i;
                            break;
                        }
                    }

                    LayerToIDMap(layer)[newValue] = value;
                }

                LayerToTileArray(layer)[(y * Width) + x] = (ushort)newValue;
            }
        }

        /// <param name="layer">The layer the tile is on.</param>
        /// <param name="x">The X position of the tile.</param>
        /// <param name="y">The Y position of the tile.</param>
        /// <returns>If the tile has any metadata attached to it.</returns>
        public bool HasMetadata(Tile.MapLayer layer, int x, int y)
        {
            return GetMetadataSlot(layer, x, y) != 0;
        }

        public ushort GetMetadataSlot(Tile.MapLayer layer, int x, int y)
        {
            return (ushort)((LayerToTileArray(layer)[(y * Width) + x] & 0xFFFF0000) >> 16);
        }

        /// <summary>
        /// Gets the metadata for the specified tile. If the tile doesn't have any metadata an empty string is returned.
        /// </summary>
        /// <param name="layer">The layer the tile is on.</param>
        /// <param name="x">The X position of the tile.</param>
        /// <param name="y">The Y position of the tile.</param>
        /// <returns>The metadata or null.</returns>
        public SortedDictionary<string, string> GetMetadata(Tile.MapLayer layer, int x, int y)
        {
            // TODO: MetadataAccess class helper
            if(HasMetadata(layer, x, y))
            {
                if(LayerToMetadataArray(layer).Count <= GetMetadataSlot(layer, x, y) - 1)
                    return null;
                return LayerToMetadataArray(layer)[GetMetadataSlot(layer, x, y) - 1];
            }

            return null;
        }

        /// <summary>
        /// Sets the metadata for a certain tile. An empty string or null = remove metadata.
        /// </summary>
        /// <param name="layer">The layer the tile is on.</param>
        /// <param name="x">The X position of the tile.</param>
        /// <param name="y">The Y position of the tile.</param>
        /// <param name="metadata">The metadata string, or null.</param>
        public void SetMetadata(Tile.MapLayer layer, int x, int y, SortedDictionary<string, string> metadata)
        {
            if(HasMetadata(layer, x, y))
            {
                ushort slot = GetMetadataSlot(layer, x, y);
                LayerToMetadataArray(layer).RemoveAt(slot - 1);
                LayerToTileArray(layer)[(y * Width) + x] = LayerToTileArray(layer)[(y * Width) + x] & 0xff;

                for(int i = 0; i < Width * Height; i++)
                {
                    if((LayerToTileArray(layer)[i] & 0xFFFF0000) >> 16 >= slot)
                    {
                        ushort lastMetadataAttr = (ushort)((LayerToTileArray(layer)[i] & 0xFFFF0000) >> 16);
                        lastMetadataAttr--;

                        LayerToTileArray(layer)[i] = (uint)((LayerToTileArray(layer)[i] & 0xFFFF) | (lastMetadataAttr << 16));
                    }
                }
            }

            if(metadata != null)
            {
                LayerToMetadataArray(layer).Add(metadata);
                LayerToTileArray(layer)[(y * Width) + x] = (uint)((LayerToTileArray(layer)[(y * Width) + x] & 0xFFFF) | ((LayerToMetadataArray(layer).Count) << 16));
            }
        }

        /// <summary>
        /// Resizes this map.
        /// </summary>
        /// <param name="newWidth">The new width</param>
        /// <param name="newHeight">The new height</param>
        public void Resize(int newWidth, int newHeight, int xOffset = int.MinValue, int yOffset = int.MinValue)
        {
            int oldWidth = Width;
            int oldHeight = Height;

            int xOffset_ = xOffset != int.MinValue ? xOffset : (int)Math.Floor((newWidth - oldWidth) / 2.0);
            int yOffset_ = yOffset != int.MinValue ? yOffset : (int)Math.Floor((newHeight - oldHeight) / 2.0);

            Width = newWidth;
            Height = newHeight;

            foreach(Tile.MapLayer layer in Tile.CachedAllMapLayers)
            {
                uint[] tiles = LayerToTileArray(layer);
                uint[] oldTiles = new uint[oldWidth * oldHeight];
                Array.Copy(tiles, oldTiles, oldWidth * oldHeight);

                Array.Resize(ref tiles, newWidth * newHeight);

                switch(layer)
                {
                    case Tile.MapLayer.Terrain:
                        _layerTerrain = tiles;
                        break;
                    case Tile.MapLayer.Decoration:
                        _layerDecoration = tiles;
                        break;
                    case Tile.MapLayer.NPC:
                        _layerNPC = tiles;
                        break;
                    case Tile.MapLayer.Control:
                        _layerControl = tiles;
                        break;
                }

                for(int i = 0; i < tiles.Length; i++)
                    tiles[i] = 0;

                for(int x = 0; x < Math.Min(oldWidth, Width); x++)
                {
                    for(int y = 0; y < Math.Min(oldHeight, Height); y++)
                    {
                        int xPos = MathHelper.Clamp(x + xOffset_, 0, Width - 1);
                        int yPos = MathHelper.Clamp(y + yOffset_, 0, Height - 1);

                        LayerToTileArray(layer)[yPos * Width + xPos] = oldTiles[y * oldWidth + x];
                    }
                }
            }

            GC.Collect();
        }

        /// <summary>
        /// Loads a map from a stream.
        /// </summary>
        /// <param name="stream">The stream to load from.</param>
        public static Map Load(Stream stream)
        {
            // TODO
            return null;
        }
        
        /// <summary>
        /// Saves a map to a stream.
        /// </summary>
        /// <param name="stream">The stream to save to.</param>
        public void Save(Stream stream)
        {
            using(StreamWriter swriter = new StreamWriter(stream, Encoding.UTF8))
            {
                using(JsonWriter writer = new JsonTextWriter(swriter))
                {
                    writer.WriteStartObject();

                    writer.WritePropertyName("version");
                    writer.WriteValue(MAP_FORMAT_VERSION);

                    writer.WritePropertyName("info");
                    writer.WriteStartObject();
                    writer.WritePropertyName("name");
                    writer.WriteValue(Info.Name);
                    writer.WritePropertyName("environment");
                    writer.WriteValue(Info.Environment);
                    writer.WritePropertyName("weather");
                    writer.WriteValue(Info.Weather);
                    writer.WriteEndObject();

                    writer.WritePropertyName("width");
                    writer.WriteValue(Width);
                    writer.WritePropertyName("height");
                    writer.WriteValue(Height);

                    writer.WritePropertyName("layers");
                    writer.WriteStartObject();

                    foreach(Tile.MapLayer layer in Tile.CachedAllMapLayers)
                    {
                        writer.WritePropertyName(layer.ToString().ToLowerInvariant());
                        writer.WriteStartArray();

                        int x, y;
                        for(x = 0; x < Width; x++)
                        {
                            for(y = 0; y < Height; y++)
                            {
                                SortedDictionary<string, string> metadata = GetMetadata(layer, x, y);

                                if(metadata != null && metadata.Count > 0)
                                {
                                    writer.WriteStartObject();
                                    writer.WritePropertyName("id");
                                    writer.WriteValue(this[layer, x, y]);
                                    writer.WritePropertyName("metadata");
                                    writer.WriteStartObject();

                                    foreach(KeyValuePair<string, string> pair in metadata)
                                    {
                                        writer.WritePropertyName(pair.Key);
                                        writer.WriteValue(pair.Value);
                                    }

                                    writer.WriteEndObject();
                                    writer.WriteEndObject();
                                }
                                else
                                    writer.WriteValue(this[layer, x, y]);
                            }
                        }

                        writer.WriteEndArray();
                    }

                    writer.WriteEndObject();

                    writer.WriteEndObject();
                }
            }
        }

        /// <param name="point">Point to check.</param>
        /// <returns>true if the terrain, decoration or control layer has a solid tile at the specified point.</returns>
        public bool IsPointBlocked(Point point)
        {
            if(point.X < 0 || point.Y < 0 || point.X >= Width || point.Y >= Height)
                return true;

            if(this[Tile.MapLayer.Control, point.X, point.Y] == Tiles.Control.InvertedBarrier)
                return false;

            return this[Tile.MapLayer.Terrain, point.X, point.Y].Solid(GetMetadata(Tile.MapLayer.Terrain, point.X, point.Y)) ||
                this[Tile.MapLayer.Decoration, point.X, point.Y].Solid(GetMetadata(Tile.MapLayer.Decoration, point.X, point.Y)) ||
                this[Tile.MapLayer.Control, point.X, point.Y].Solid(GetMetadata(Tile.MapLayer.Control, point.X, point.Y));
        }

        /// <summary>
        /// The tile transition cache.
        /// </summary>
        public static TileTransitionCache TransitionCache { get; private set; }

        /// <summary>
        /// Draws this map.
        /// </summary>
        /// <param name="layer">Layer to draw.</param>
        /// <param name="game">Game class.</param>
        /// <param name="rectangle">The cropped part of the map to draw. null - draw whole map</param>
        public void Draw(Tile.MapLayer layer, Game game, Rectangle? rectangle = null)
        {
            if(TransitionCache != null && TransitionCache.Game != game)
            {
                TransitionCache.Dispose();
                TransitionCache = null;
            }

            if(TransitionCache == null)
                TransitionCache = new TileTransitionCache(game);
            
            int xMin = 0;
            int yMin = 0;
            int xMax = Width - 1;
            int yMax = Height - 1;

            if(rectangle.HasValue)
            {
                xMin = Math.Max(0, (int)Math.Floor(rectangle.Value.X / 16.0));
                yMin = Math.Max(0, (int)Math.Floor(rectangle.Value.Y / 16.0));
                xMax = Math.Min(Width - 1, (int)Math.Ceiling((rectangle.Value.X + rectangle.Value.Width - 1) / 16.0));
                yMax = Math.Min(Height - 1, (int)Math.Ceiling((rectangle.Value.Y + rectangle.Value.Height - 1) / 16.0));
            }

            // Add one more to allow for transitions outside the rectangle.
            if(xMin > 0)
                xMin--;
            if(yMin > 0)
                yMin--;

            if(xMax < Width - 1)
                xMax++;
            if(yMax < Height - 1)
                yMax++;
            
            Rectangle drawRectangle = new Rectangle(xMin, yMin, xMax - xMin, yMax - yMin);
            drawRectangle.X *= 16;
            drawRectangle.Y *= 16;
            drawRectangle.Width *= 16;
            drawRectangle.Height *= 16;
            drawRectangle.Width += 16;
            drawRectangle.Height += 16;

            if(layer == Tile.MapLayer.Terrain)
            {
                if(Info.Environment == Environment.Inside)
                    game.Batch.Rectangle(drawRectangle, Color.Black);
                else
                    game.Batch.Texture(drawRectangle, game.Assets.Get<Texture2D>("tiles/water/" + (int) ((game.Time * 2) % 4) + ".png"), Color.White, drawRectangle);
            }

            int x, y;

            for(x = xMin; x <= xMax; x++)
            {
                for(y = yMin; y <= yMax; y++)
                {
                    Tile tile = this[layer, x, y];
                    if(tile.ID != "") // water will be drawn by the map manually here
                        tile.Draw(game, new Point(x, y), this, GetMetadata(layer, x, y), Info.Environment);
                }
            }
            
            for(x = xMin; x <= xMax; x++)
            {
                for(y = yMin; y <= yMax; y++)
                {
                    Tile tile = this[layer, x, y];
                    if(layer == Tile.MapLayer.Terrain || tile.ID != "")
                    {
                        Point p1 = new Point(x - 1, y);
                        Point p2 = new Point(x + 1, y);
                        Point p3 = new Point(x, y - 1);
                        Point p4 = new Point(x, y + 1);

                        for(int i = 0; i < 4; i++)
                        {
                            Point point = i == 0 ? p1 :
                                i == 1 ? p2 :
                                i == 2 ? p3 :
                                p4;
                            
                            if(point.X >= 0 && point.Y >= 0 && point.X < Width && point.Y < Height &&
                               tile.UseTransition(
                                   new Point(x, y),
                                   point,
                                   this, 
                                   tile, 
                                   GetMetadata(layer, x, y), 
                                   GetMetadata(layer, point.X, point.Y)))
                            {
                                Point transitionPoint;
                                Texture2D transitionTexture = TransitionCache.TextureForTile(
                                    tile, 
                                    GetMetadata(layer, x, y), 
                                    Info.Environment, 
                                    out transitionPoint);
                                
                                float rotation = 0;
                                SpriteEffects effects = SpriteEffects.None;

                                if(point == new Point(x + 1, y))
                                    effects = SpriteEffects.FlipHorizontally;
                                if(point == new Point(x, y - 1))
                                    rotation = MathHelper.PiOver2;
                                if(point == new Point(x, y + 1))
                                    rotation = MathHelper.Pi + MathHelper.PiOver2;

                                Rectangle textureRectangle = new Rectangle(
                                    transitionPoint.X * 16,
                                    transitionPoint.Y * 16,
                                    16,
                                    16);
                                
                                game.Batch.Texture(
                                    new Vector2(
                                        point.X * 16 + (rotation == MathHelper.PiOver2 ? 16 : 0),
                                        point.Y * 16 + (rotation == MathHelper.Pi + MathHelper.PiOver2 ? 16 : 0)),
                                    transitionTexture,
                                    Color.White,
                                    Vector2.One,
                                    textureRectangle,
                                    rotation, 
                                    null, 
                                    effects);
                            }
                        }
                    }
                }
            }

            for(x = xMin; x <= xMax; x++)
            {
                for(y = yMin; y <= yMax; y++)
                {
                    Tile tile = this[layer, x, y];
                    if(layer == Tile.MapLayer.Terrain || tile.ID != "")
                        tile.DrawAfterTransition(game, new Point(x, y), this, GetMetadata(layer, x, y), Info.Environment);
                }
            }
        }
    }
}