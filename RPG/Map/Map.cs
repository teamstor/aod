using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace TeamStor.RPG
{
    /// <summary>
    /// Map data.
    /// </summary>
    public class Map
    {
        /// <summary>
        /// Map environment.
        /// </summary>
        public enum Environment
        {
            Forest = 0,
            SnowMountain = 1,
            Swamp = 2,

            Inside = 10
        }

        /// <summary>
        /// Map weather.
        /// </summary>
        public enum Weather
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
        private short[] _layerTerrain, _layerDecoration, _layerNPC, _layerControl;
        private List<string> _metadataTerrain, _metadataDecoration, _metadataNPC, _metadataControl;

        private short[] LayerToTileArray(Tile.MapLayer layer)
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
            Info = info;

            _layerTerrain = new short[w * h];
            _layerDecoration = new short[w * h];
            _layerNPC = new short[w * h];
            _layerControl = new short[w * h];

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
            return (byte)(LayerToTileArray(layer)[(y * Width) + x] & 0x00ff);
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
                return LayerToMetadataArray(layer)[GetMetadataSlot(layer, x, y) - 1];

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
                LayerToMetadataArray(layer).RemoveAt((y * Width) + x);
                LayerToTileArray(layer)[(y * Width) + x] = (byte)(LayerToTileArray(layer)[(y * Width) + x] & 0xff);

                for(int i = 0; i < Width * Height; i++)
                {
                    if((LayerToTileArray(layer)[i] & 0xff00) > slot)
                    {
                        LayerToTileArray(layer)[i] = (short)(
                            (LayerToTileArray(layer)[i] & 0xff) | 
                            (LayerToTileArray(layer)[i] & 0xff00) - 1);
                    }
                }
            }

            if(!String.IsNullOrEmpty(metadata))
            {
                LayerToMetadataArray(layer).Add(metadata);
                LayerToTileArray(layer)[(y * Width) + x] = (short)(
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
		        short[] tiles = LayerToTileArray(layer);
		        short[] oldTiles = new short[oldWidth * oldHeight];
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

		        for(int x = 0; x < Math.Min(oldWidth, Width); x++)
		        {
		            for(int y = 0; y < Math.Min(oldHeight, Height); y++)
		            {
		                int xPos = MathHelper.Clamp(x + xOffset_, 0, Width - 1);
		                int yPos = MathHelper.Clamp(y + yOffset_, 0, Height - 1);

                        // TODO ÄR FEL
			            LayerToTileArray(layer)[yPos * Width + xPos] = oldTiles[yPos * oldWidth + xPos];
		            }
		        }
		    }
		}
    }
}