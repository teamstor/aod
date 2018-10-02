﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using TeamStor.Engine;
using TeamStor.Engine.Graphics;

using SpriteBatch = TeamStor.Engine.Graphics.SpriteBatch;

namespace TeamStor.RPG
{
    public class Tile
    {
        public const string TILE_TEXTURE_LAYER_TERRAIN = "tiles/terrain.png";
        public const string TILE_TEXTURE_LAYER_DECORATION = "tiles/decoration.png";
        public const string TILE_TEXTURE_LAYER_NPC = "tiles/template.png"; // NPCs draw their own textures.
        public const string TILE_TEXTURE_LAYER_CONTROL = "tiles/control.png";
        
        public const string TRANSTION_GENERIC = "tiles/transitions/generic.png";

        /// <param name="layer">The layer.</param>
        /// <returns>The texture name used by the specified layer.</returns>
        public static string LayerToTextureName(MapLayer layer)
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

        /// <param name="layer">The layer.</param>
        /// <returns>The texture used by the specified layer.</returns>
        public static Texture2D LayerToTexture(Engine.Game game, MapLayer layer)
        {
            return game.Assets.Get<Texture2D>(LayerToTextureName(layer), true);
        }

        /// <summary>
        /// Layer that a tile is on. 
        /// Layers are stacked on top of each other with "terrain" being the lowest and "control" being the highest.
        /// </summary>
        public enum MapLayer : byte
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
        public virtual string Name(Dictionary<string, string> metadata = null, Map.Environment environment = Map.Environment.Forest)
        {
            return _name;
        }
        
        private Point _textureSlot;

        /// <param name="metadata">Metadata for the tile being accessed.</param>
        /// <returns>
        /// The texture slot to use when drawing the tile.
        /// </returns>
        public virtual Point TextureSlot(Dictionary<string, string> metadata = null, Map.Environment environment = Map.Environment.Forest)
        {
            return _textureSlot;
        }
        
        private bool _solid;

        /// <param name="metadata">Metadata for the tile being accessed.</param>
        /// <returns>
        /// If this tile is solid or not (if the player can walk through it).
        /// </returns>
        public virtual bool Solid(Dictionary<string, string> metadata = null)
        {
            return _solid;
        }

        private int _transitionPriority;

        /// <param name="metadata">Metadata for the tile being accessed.</param>
        /// <returns>
        /// The transition priority for the tile. Higher = will get transition.
        /// -1 = never use transition with this tile.
        /// </returns>
        public virtual int TransitionPriority(Dictionary<string, string> metadata = null)
        {
            return _transitionPriority;
        }


        /// <param name="from">The point the transition is coming from.</param>
        /// <param name="to">The point the transition is going to.</param>
        /// <param name="map">The map.</param>
        /// <param name="other">The tile to transition with.</param>
        /// <param name="metadata">Metadata for the tile being accessed.</param>
        /// <param name="otherMetadata">Metadata for the other tile being accessed.</param>
        /// <returns>
        /// If this tile should create a transition to the other tile.
        /// By default this will happen if:
        /// * The tile is on the terrain layer
        /// * The tile has a higher ID than the other tile
        /// * The tile doesn't have the same ID as the other tile
        /// OR
        /// * The tile is on the terrain layer and has a higher transition priority (and the other tile doesn't have -1).
        /// </returns>
        public virtual bool UseTransition(Point from, Point to, Map map, Tile other, Dictionary<string, string> metadata = null, Dictionary<string, string> otherMetadata = null)
        {
            if(TransitionPriority() == -1)
                return false;
            if(other.TransitionPriority(otherMetadata) == -1)
                return false;

            if(Layer == MapLayer.Terrain)
            {
                if(other.TransitionPriority(otherMetadata) < TransitionPriority(metadata))
                    return true;
                else if(other.TransitionPriority(otherMetadata) == TransitionPriority(metadata))
                    return other.ID < ID;
            }

            return false;
        }

        /// <param name="environment">The environment to filter by.</param>
        /// <returns>If this tile is allowed in a specified environment.</returns>
        public virtual bool Filter(Map.Environment environment)
        {
            // A tile with ID 0 always exists
            if(ID == 0 || Layer == MapLayer.Control)
                return true;
            return environment != Map.Environment.Inside;
        }
        
        /// <param name="metadata">Metadata for the tile being accessed.</param>
        /// <param name="environment">The environment the tile is in.</param>
        /// <returns>The transition texture to use when making a transition with other tiles.</returns>
        public virtual string TransitionTexture(Dictionary<string, string> metadata = null, Map.Environment environment = Map.Environment.Forest)
        {
            return TRANSTION_GENERIC;
        }

        /// <summary>
        /// Draws this tile.
        /// </summary>
        /// <param name="batch">The batch to draw with.</param>
        /// <param name="time"></param>
        /// <param name="mapPos"></param>
        /// <param name="map"></param>
        /// <param name="metadata"></param>
        public virtual void Draw(Engine.Game game, Point mapPos, Map map, Dictionary<string, string> metadata, Map.Environment environment, Color? color = null)
        {
            game.Batch.Texture(
                new Vector2(mapPos.X * 16, mapPos.Y * 16),
                LayerToTexture(game, Layer),
                color.HasValue ? color.Value : Color.White,
                Vector2.One,
                new Rectangle(TextureSlot(metadata, environment).X * 16, TextureSlot(metadata, environment).Y * 16, 16, 16));
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

        public Tile(byte id, MapLayer layer, string name, Point textureSlot, bool solid = false, int transitionPriority = 1000)
        {            
            ID = id;
            Layer = layer;
            _name = name;
            _textureSlot = textureSlot;
            _solid = solid;
            _transitionPriority = transitionPriority;
            
            if(LayerToDictionary(layer).ContainsKey(id))
                throw new ArgumentException("ID (" + id + "," + layer + ") already in use by another tile.");
            
            LayerToDictionary(layer).Add(id, this);
        }

        /// <summary>
        /// Generates a unique identifer for this tile based on the metadata and environment.
        /// </summary>
        /// <param name="metadata">Metadata for the tile being accessed.</param>
        /// <param name="environment">The environment the tile is in.</param>
        /// <returns>A unique identifier.</returns>
        public long UniqueIdentity(Dictionary<string, string> metadata, Map.Environment environment)
        {
            // TODO HASH CODE FÖR EN DICTIONARY????
            long id = metadata.GetHashCoaaaaaaaaaaaaaaaae();
            id |= (long)ID << 36;
            id |= (long)environment << 44;
            id |= (long)Layer << 52;
            
            // TODO: can have 16 more bits here

            return id;
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
        public static Tile FindByName(string name, MapLayer layer, Map.Environment environment = Map.Environment.Forest)
        {
            foreach(Tile tile in LayerToDictionary(layer).Values)
            {
                if(tile.Name(null, environment) == name)
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
