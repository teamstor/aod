using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamStor.RPG.Map
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
            /// <summary>
            /// Name of the map that will be shown when you enter it.
            /// </summary>
            public string Name;

            /// <summary>
            /// Environment the map is in.
            /// </summary>
            public Environment Environment;
        }

        // The lower 8 bits of the layer arrays are the ID of the tile. The higher 8 bits are the position into the metadata array.
        // If the higher 8 bits are all 0 the tile doesn't have any metadata.
        private short[] _layerTerrain, _layerDecoration, _layerNPC, _layerControl;
        private List<string> _metadataTerrain, _metadataDecoration, _metadataNPC, _metadataControl;

        private short[] LayerToTileArray(Tile.Layer layer)
        {
            switch(layer)
            {
                case Tile.Layer.Terrain:
                    return _layerTerrain;

                case Tile.Layer.Decoration:
                    return _layerDecoration;

                case Tile.Layer.NPC:
                    return _layerNPC;

                case Tile.Layer.Control:
                    return _layerControl;
            }

            return null;
        }

        private List<string> LayerToMetadataArray(Tile.Layer layer)
        {
            switch(layer)
            {
                case Tile.Layer.Terrain:
                    return _metadataTerrain;

                case Tile.Layer.Decoration:
                    return _metadataDecoration;

                case Tile.Layer.NPC:
                    return _metadataNPC;

                case Tile.Layer.Control:
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
        }

        /// <summary>
        /// The height of the map in tiles.
        /// </summary>
        public int Height
        {
            get;
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

        public byte this[Tile.Layer layer, int x, int y]
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
        public bool HasMetadata(Tile.Layer layer, int x, int y)
        {
            return GetMetadataSlot(layer, x, y) != 0;
        }

        private byte GetMetadataSlot(Tile.Layer layer, int x, int y)
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
        public string GetMetadata(Tile.Layer layer, int x, int y)
        {
            if(HasMetadata(layer, x, y))
                return LayerToMetadataArray(layer)[GetMetadataSlot(layer, x, y)];

            return "";
        }

        /// <summary>
        /// Sets the metadata for a certain tile. An empty string or null = remove metadata.
        /// </summary>
        /// <param name="layer">The layer the tile is on.</param>
        /// <param name="x">The X position of the tile.</param>
        /// <param name="y">The Y position of the tile.</param>
        /// <param name="metadata">The metadata string, or null.</param>
        public void SetMetadata(Tile.Layer layer, int x, int y, string metadata)
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
                            (LayerToTileArray(layer)[i] & 0xff) + 
                            (LayerToTileArray(layer)[i] & 0xff00) - 1);
                    }
                }
            }

            if(!String.IsNullOrEmpty(metadata))
            {
                LayerToMetadataArray(layer).Add(metadata);
                LayerToTileArray(layer)[(y * Width) + x] = (short)(
                    (LayerToTileArray(layer)[(y * Width) + x & 0xff) +
                    (LayerToTileArray(layer)[(y * Width) + x] & 0xff00) - 1));
            }
        }
    }
}