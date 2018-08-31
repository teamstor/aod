using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace TeamStor.RPG
{
    public class Tile
    {
        public const string TILE_TEXTURE_LAYER_TERRAIN = "tiles/terrain.png";
        public const string TILE_TEXTURE_LAYER_DECORATION = "tiles/decoration.png";
        public const string TILE_TEXTURE_LAYER_NPC = "tiles/npc.png";
        public const string TILE_TEXTURE_LAYER_CONTROL = "tiles/control.png";

        /// <param name="layer">The layer.</param>
        /// <returns>The texture used by the specified layer.</returns>
        public static string LayerToTexture(MapLayer layer)
        {
            switch(layer)
            {
                case MapLayer.Terrain:
                    return TILE_TEXTURE_LAYER_TERRAIN;
                
                case MapLayer.Decoration:
                    return TILE_TEXTURE_LAYER_DECORATION;
                
                case MapLayer.NPC:
                    return TILE_TEXTURE_LAYER_NPC;
                
                case MapLayer.Control:
                    return TILE_TEXTURE_LAYER_CONTROL;
            }

            return "";
        }

        /// <summary>
        /// Layer that a tile is on. 
        /// Layers are stacked on top of each other with "terrain" being the lowest and "control" being the highest.
        /// </summary>
        public enum MapLayer
        {
            /// <summary>
            /// The actual ground layer of the map. Tiles on this layer can be walked upon and no tile can be empty on this layer.
            /// Terrain tiles blend together with each other.
            /// </summary>
            Terrain = 0,
            /// <summary>
            /// Decoration tiles. These can be either solid or walk-through.
            /// </summary>
            Decoration = 1,
            /// <summary>
            /// Other characters in the world.
            /// </summary>
            NPC = 2,
            /// <summary>
            /// Control tiles are invisible tiles that control things such as enemy encounters or text boxes.
            /// </summary>
            Control = 3
        }
        
        /// <summary>
        /// The ID of the texture to use in map data.
        /// </summary>
        public byte ID { get; private set; }
        
        /// <summary>
        /// The layer this tile is on.
        /// </summary>
        public MapLayer Layer { get; private set; }
        
        private string _name;

        /// <param name="metadata">Metadata for the tile being accessed.</param>
        /// <returns>
        /// The name of the tile used in the map editor.
        /// </returns>
        public virtual string Name(string metadata = "")
        {
            return _name;
        }
        
        private Point _textureSlot;

        /// <param name="metadata">Metadata for the tile being accessed.</param>
        /// <returns>
        /// The texture slot to use when drawing the tile.
        /// </returns>
        public virtual Point TextureSlot(string metadata = "")
        {
            return _textureSlot;
        }
        
        private bool _solid;

        /// <param name="metadata">Metadata for the tile being accessed.</param>
        /// <returns>
        /// If this tile is solid or not (if the player can walk through it).
        /// </returns>
        public virtual bool Solid(string metadata = "")
        {
            return _solid;
        }
        
        /// <param name="other">The tile to transition with.</param>
        /// <param name="metadata">Metadata for the tile being accessed.</param>
        /// <param name="otherMetadata">Metadata for the other tile being accessed.</param>
        /// <returns>
        /// If this tile should create a transition to the other tile.
        /// By default this will happen if:
        /// * The tile is on the terrain layer
        /// * The tile has a higher ID than the other tile
        /// * The tile doesn't have the same ID as the other tile
        /// </returns>
        public virtual bool UseTransition(Tile other, string metadata = "", string otherMetadata = "")
        {
            return other.ID < ID && Layer == MapLayer.Terrain;
        }
        
        private static SortedDictionary<byte, Tile> _tilesTerrain = new SortedDictionary<byte, Tile>();
        private static SortedDictionary<byte, Tile> _tilesDecoration = new SortedDictionary<byte, Tile>();
        private static SortedDictionary<byte, Tile> _tilesNPC = new SortedDictionary<byte, Tile>();
        private static SortedDictionary<byte, Tile> _tilesControl = new SortedDictionary<byte, Tile>();

        private static SortedDictionary<byte, Tile> LayerToDictionary(MapLayer layer)
        {
            switch(layer)
            {
                case MapLayer.Terrain:
                    return _tilesTerrain;
                
                case MapLayer.Decoration:
                    return _tilesDecoration;
                
                case MapLayer.NPC:
                    return _tilesNPC;
                
                case MapLayer.Control:
                    return _tilesControl;
            }

            return null;
        }

        public Tile(byte id, MapLayer layer, string name, Point textureSlot, bool solid = false)
        {
            ID = id;
            Layer = layer;
            _name = name;
            _textureSlot = textureSlot;
            _solid = solid;
            
            if(LayerToDictionary(layer).ContainsKey(id))
                throw new ArgumentException("ID (" + id + "," + layer + ") already in use by another tile.");
            
            LayerToDictionary(layer).Add(id, this);
        }

        /// <summary>
        /// Finds a tile by ID.
        /// </summary>
        /// <param name="id">The ID of the tile.</param>
        /// <param name="layer">The layer the tile belongs to.</param>
        /// <returns>The tile, or an exception.</returns>
        public static Tile Find(byte id, MapLayer layer)
        {
            return LayerToDictionary(layer)[id];
        }
        
        /// <summary>
        /// Finds a tile by name.
        /// </summary>
        /// <param name="name">The name of the tile without metadata.</param>
        /// <param name="layer">The layer the tile belongs to.</param>
        /// <returns>The tile, or an exception.</returns>
        public static Tile FindByName(string name, MapLayer layer)
        {
            foreach(Tile tile in LayerToDictionary(layer).Values)
            {
                if(tile.Name() == name)
                    return tile;
            }

            throw new ArgumentException("Tile with name " + name + " not found.");
        }
        
        /// <param name="name">The ID of the tile.</param>
        /// <param name="layer">The layer the tile belongs to.</param>
        /// <returns>true if a tile with the specified ID exists.</returns>
        public static bool Exists(byte id, MapLayer layer)
        {
            return LayerToDictionary(layer).ContainsKey(id);
        }

        /// <param name="layer">The layer to get tiles from.</param>
        /// <returns>All tiles on a specified layer.</returns>
        public static IEnumerable<Tile> Values(MapLayer layer)
        {
            return LayerToDictionary(layer).Values;
        }
    }
}
