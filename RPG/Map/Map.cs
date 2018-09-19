using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        public const int MAP_FORMAT_VERSION = 1;
        
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

        // The lower 8 bits of the layer arrays are the ID of the tile. The higher 8 bits are the position into the metadata array.
        // If the higher 8 bits are all 0 the tile doesn't have any metadata.
        private int[] _layerTerrain, _layerDecoration, _layerNPC, _layerControl;
        private List<string> _metadataTerrain, _metadataDecoration, _metadataNPC, _metadataControl;

        private int[] LayerToTileArray(Tile.MapLayer layer)
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

        private List<string> LayerToMetadataArray(Tile.MapLayer layer)
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

            _layerTerrain = new int[w * h];
            _layerDecoration = new int[w * h];
            _layerNPC = new int[w * h];
            _layerControl = new int[w * h];

            _metadataTerrain = new List<string>();
            _metadataDecoration = new List<string>();
            _metadataNPC = new List<string>();
            _metadataControl = new List<string>();
        }

        public byte this[Tile.MapLayer layer, int x, int y]
        {
            get
            {
                return (byte)(LayerToTileArray(layer)[(y * Width) + x] & 0xff);
            }
            set
            {
                SetMetadata(layer, x, y, null);
                LayerToTileArray(layer)[(y * Width) + x] = value;
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

        private byte GetMetadataSlot(Tile.MapLayer layer, int x, int y)
        {
            // TODO något är fortfarande fel
            return (byte)((LayerToTileArray(layer)[(y * Width) + x] & 0xff00) >> 8);
        }

        /// <summary>
        /// Gets the metadata for the specified tile. If the tile doesn't have any metadata an empty string is returned.
        /// </summary>
        /// <param name="layer">The layer the tile is on.</param>
        /// <param name="x">The X position of the tile.</param>
        /// <param name="y">The Y position of the tile.</param>
        /// <returns>The metadata or an empty string.</returns>
        public string GetMetadata(Tile.MapLayer layer, int x, int y)
        {
            if(HasMetadata(layer, x, y))
            {
                if(LayerToMetadataArray(layer).Count <= GetMetadataSlot(layer, x, y) - 1)
                    return "";
                return LayerToMetadataArray(layer)[GetMetadataSlot(layer, x, y) - 1];
            }

            return "";
        }

        /// <summary>
        /// Sets the metadata for a certain tile. An empty string or null = remove metadata.
        /// </summary>
        /// <param name="layer">The layer the tile is on.</param>
        /// <param name="x">The X position of the tile.</param>
        /// <param name="y">The Y position of the tile.</param>
        /// <param name="metadata">The metadata string, or null.</param>
        public void SetMetadata(Tile.MapLayer layer, int x, int y, string metadata)
        {
            if(HasMetadata(layer, x, y))
            {
                byte slot = GetMetadataSlot(layer, x, y);
                LayerToMetadataArray(layer).RemoveAt(slot);
                LayerToTileArray(layer)[(y * Width) + x] = (byte)(LayerToTileArray(layer)[(y * Width) + x] & 0xff);

                for(int i = 0; i < Width * Height; i++)
                {
                    if((LayerToTileArray(layer)[i] & 0xff00) >> 8 > slot)
                    {
                        byte lastMetadataAttr = (byte)((LayerToTileArray(layer)[i] & 0xff00) >> 8);
                        lastMetadataAttr--;

                        LayerToTileArray(layer)[i] = (int)(
                            (LayerToTileArray(layer)[i] & 0xff) |
                            (lastMetadataAttr << 8));
                    }
                }
            }

            if(!String.IsNullOrEmpty(metadata))
            {
                LayerToMetadataArray(layer).Add(metadata);
                LayerToTileArray(layer)[(y * Width) + x] = (int)(
                    (LayerToTileArray(layer)[(y * Width) + x] & 0xff) |
                    ((LayerToMetadataArray(layer).Count) << 8));
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

            foreach(Tile.MapLayer layer in Enum.GetValues(typeof(Tile.MapLayer)))
            {
                int[] tiles = LayerToTileArray(layer);
                int[] oldTiles = new int[oldWidth * oldHeight];
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
            Information info = new Information("???", Environment.Forest, Weather.Sunny);
            int width = 0, height = 0;

            using(BinaryReader reader = new BinaryReader(stream, Encoding.UTF8))
            {
                if(reader.ReadString() != "rpg:map")
                    throw new Exception("Not a valid map stream");

                int readVersion;
                if((readVersion = reader.ReadInt32()) != MAP_FORMAT_VERSION)
                    throw new Exception("Map loaded from " + 
                                        (readVersion > MAP_FORMAT_VERSION ? "a newer" : "an older") + 
                                        " version (" + MAP_FORMAT_VERSION + " != " + readVersion + ")");
                
                info.Name = reader.ReadString();
                info.Environment = (Environment)reader.ReadByte();
                info.Weather = (Weather)reader.ReadByte();

                width = reader.ReadInt32();
                height = reader.ReadInt32();
				
                Map map = new Map(width, height, info);
                for(int i = 0; i < width * height; i++)
                {
                    map._layerTerrain[i] = reader.ReadInt32();
                    map._layerDecoration[i] = reader.ReadInt32();
                    map._layerNPC[i] = reader.ReadInt32();
                    map._layerControl[i] = reader.ReadInt32();
                }

                int count = reader.ReadByte();
                for(int i = 0; i < count; i++)
                    map._metadataTerrain.Add(reader.ReadString());
                
                count = reader.ReadByte();
                for(int i = 0; i < count; i++)
                    map._metadataDecoration.Add(reader.ReadString());
                
                count = reader.ReadByte();
                for(int i = 0; i < count; i++)
                    map._metadataNPC.Add(reader.ReadString());
                
                count = reader.ReadByte();
                for(int i = 0; i < count; i++)
                    map._metadataControl.Add(reader.ReadString());

                GC.Collect();

                return map;
            }
        }
        
        /// <summary>
        /// Saves a map to a stream.
        /// </summary>
        /// <param name="stream">The stream to save to.</param>
        public void Save(Stream stream)
        {
            using(BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8))
            {
                writer.Write("rpg:map");
                writer.Write(MAP_FORMAT_VERSION);
                
                writer.Write(Info.Name);
                writer.Write((byte)Info.Environment);
                writer.Write((byte)Info.Weather);
                
                writer.Write(Width);
                writer.Write(Height);

                for(int i = 0; i < Width * Height; i++)
                {
                    writer.Write(_layerTerrain[i]);
                    writer.Write(_layerDecoration[i]);
                    writer.Write(_layerNPC[i]);
                    writer.Write(_layerControl[i]);
                }

                writer.Write(_metadataTerrain.Count);
                foreach(string s in _metadataTerrain)
                    writer.Write(s);
                
                writer.Write(_metadataDecoration.Count);
                foreach(string s in _metadataDecoration)
                    writer.Write(s);
                
                writer.Write(_metadataNPC.Count);
                foreach(string s in _metadataNPC)
                    writer.Write(s);
                
                writer.Write(_metadataControl.Count);
                foreach(string s in _metadataControl)
                    writer.Write(s);
            }
        }

        /// <param name="point">Point to check.</param>
        /// <returns>true if the terrain, decoration or control layer has a solid tile at the specified point.</returns>
        public bool IsPointBlocked(Point point)
        {
            return Tile.Find(this[Tile.MapLayer.Terrain, point.X, point.Y], Tile.MapLayer.Terrain).Solid(GetMetadata(Tile.MapLayer.Terrain, point.X, point.Y)) ||
                Tile.Find(this[Tile.MapLayer.Decoration, point.X, point.Y], Tile.MapLayer.Decoration).Solid(GetMetadata(Tile.MapLayer.Decoration, point.X, point.Y)) ||
                Tile.Find(this[Tile.MapLayer.Control, point.X, point.Y], Tile.MapLayer.Control).Solid(GetMetadata(Tile.MapLayer.Control, point.X, point.Y));
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

            int x, y;

            for(x = xMin; x <= xMax; x++)
            {
                for(y = yMin; y <= yMax; y++)
                {
                    byte tile = this[layer, x, y];
                    if(layer == Tile.MapLayer.Terrain || (tile & 0xff) != 0)
                        Tile.Find(tile, layer).Draw(game, new Point(x, y), this, GetMetadata(layer, x, y), Info.Environment);
                }
            }
            
            for(x = xMin; x <= xMax; x++)
            {
                for(y = yMin; y <= yMax; y++)
                {
                    byte tile = this[layer, x, y];
                    if(layer == Tile.MapLayer.Terrain || (tile & 0xff) != 0)
                    {
                        Point[] points = new Point[]
                        {
                            new Point(x - 1, y),
                            new Point(x + 1, y),
                            new Point(x, y - 1),
                            new Point(x, y + 1)
                        };

                        for(int i = 0; i < points.Length; i++)
                        {
                            Point point = points[i];
                            
                            if(point.X >= 0 && point.Y >= 0 && point.X < Width && point.Y < Height &&
                               Tile.Find(tile, layer).UseTransition(
                                   new Point(x, y),
                                   point,
                                   this, 
                                   Tile.Find(this[layer, point.X, point.Y], layer), 
                                   GetMetadata(layer, x, y), 
                                   GetMetadata(layer, point.X, point.Y)))
                            {
                                Point transitionPoint;
                                Texture2D transitionTexture = TransitionCache.TextureForTile(
                                    Tile.Find(tile, layer), 
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
        }
    }
}