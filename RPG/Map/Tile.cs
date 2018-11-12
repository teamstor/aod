using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using TeamStor.Engine;
using TeamStor.Engine.Graphics;
using TeamStor.RPG.Editor;
using SpriteBatch = TeamStor.Engine.Graphics.SpriteBatch;

namespace TeamStor.RPG
{
    public class Tile
    {
        public const string TRANSTION_GENERIC = "tiles/transitions/generic.png";

        public delegate string NameOverrideDelegate(string defaultValue, SortedDictionary<string, string> metadata, Map.Environment environment);
        public delegate string TextureNameOverrideDelegate(string defaultValue, SortedDictionary<string, string> metadata, Map.Environment environment);
        public delegate bool SolidOverrideDelegate(bool defaultValue, SortedDictionary<string, string> metadata);
        public delegate int TransitionPriorityOverrideDelegate(int defaultValue, SortedDictionary<string, string> metadata);
        public delegate bool UseTransitionOverrideDelegate(bool defaultValue, Point from, Point to, Map map, Tile other, SortedDictionary<string, string> metadata);
        public delegate string TransitionTextureOverrideDelegate(string defaultValue, SortedDictionary<string, string> metadata = null, Map.Environment environment = Map.Environment.Forest);
        public delegate TileAttributeEditor[] AttributeEditorsOverrideDelegate(MapEditorState state, ref int currentY);

        private NameOverrideDelegate _nameFunc;
        public Tile AttributeName(NameOverrideDelegate d) { _nameFunc = d; return this; }

        private TextureNameOverrideDelegate _tnameFunc;
        public Tile AttributeTextureName(TextureNameOverrideDelegate d) { _tnameFunc = d; return this; }

        private SolidOverrideDelegate _sFunc;
        public Tile AttributeSolid(SolidOverrideDelegate d) { _sFunc = d; return this; }

        private TransitionPriorityOverrideDelegate _tpriorityFunc;
        public Tile AttributeTransitionPriority(TransitionPriorityOverrideDelegate d) { _tpriorityFunc = d; return this; }

        private UseTransitionOverrideDelegate _useTransitionFunc;
        public Tile AttributeUseTransition(UseTransitionOverrideDelegate d) { _useTransitionFunc = d; return this; }

        private TransitionTextureOverrideDelegate _tTextureFunc;
        public Tile AttributeTransitionTexture(TransitionTextureOverrideDelegate d) { _tTextureFunc = d; return this; }

        private AttributeEditorsOverrideDelegate _attribEditorsFunc;
        public Tile AttributeTileAttributeEditors(AttributeEditorsOverrideDelegate d) { _attribEditorsFunc = d; return this; }

        public static Tile.MapLayer[] CachedAllMapLayers = Enum.GetValues(typeof(Tile.MapLayer)).Cast<Tile.MapLayer>().ToArray();

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
        /// The ID of the tile.
        /// </summary>
        public string ID { get; private set; }
        
        /// <summary>
        /// The layer this tile is on.
        /// </summary>
        public MapLayer Layer { get; private set; }
        
        private string _name;

        /// <param name="metadata">Metadata for the tile being accessed.</param>
        /// <returns>
        /// The name of the tile used in the map editor.
        /// </returns>
        public virtual string Name(SortedDictionary<string, string> metadata = null, Map.Environment environment = Map.Environment.Forest)
        {
            if(_nameFunc != null)
                return _nameFunc(_name, metadata, environment);
            return _name;
        }
        
        private string _textureName;

        /// <param name="metadata">Metadata for the tile being accessed.</param>
        /// <returns>
        /// The texture to use when drawing the tile.
        /// </returns>
        public virtual string TextureName(SortedDictionary<string, string> metadata = null, Map.Environment environment = Map.Environment.Forest)
        {
            if(_tnameFunc != null)
                return _tnameFunc(_textureName, metadata, environment);
            return _textureName;
        }
        
        private bool _solid;

        /// <param name="metadata">Metadata for the tile being accessed.</param>
        /// <returns>
        /// If this tile is solid or not (if the player can walk through it).
        /// </returns>
        public virtual bool Solid(SortedDictionary<string, string> metadata = null)
        {
            if(_sFunc != null)
                return _sFunc(_solid, metadata);
            return _solid;
        }

        private int _transitionPriority;

        /// <param name="metadata">Metadata for the tile being accessed.</param>
        /// <returns>
        /// The transition priority for the tile. Higher = will get transition.
        /// -1 = never use transition with this tile.
        /// </returns>
        public virtual int TransitionPriority(SortedDictionary<string, string> metadata = null)
        {
            if(_tpriorityFunc != null)
                return _tpriorityFunc(_transitionPriority, metadata);
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
        /// * The tile ID hash has a higher int value than the other tile
        /// * The tile doesn't have the same ID as the other tile
        /// OR
        /// * The tile is on the terrain layer and has a higher transition priority (and the other tile doesn't have -1).
        /// </returns>
        public virtual bool UseTransition(Point from, Point to, Map map, Tile other, SortedDictionary<string, string> metadata = null, SortedDictionary<string, string> otherMetadata = null)
        {
            bool result = false;

            if(Layer == MapLayer.Terrain && TransitionPriority() != -1 && other.TransitionPriority(otherMetadata) != -1)
            {
                if(other.TransitionPriority(otherMetadata) < TransitionPriority(metadata))
                    result = true;
                else if(other.TransitionPriority(otherMetadata) == TransitionPriority(metadata))
                    result = other.ID.GetHashCode() > ID.GetHashCode();
            }

            if(_useTransitionFunc != null)
                return _useTransitionFunc(result, from, to, map, other, metadata);

            return result;
        }
        
        /// <param name="metadata">Metadata for the tile being accessed.</param>
        /// <param name="environment">The environment the tile is in.</param>
        /// <returns>The transition texture to use when making a transition with other tiles.</returns>
        public virtual string TransitionTexture(SortedDictionary<string, string> metadata = null, Map.Environment environment = Map.Environment.Forest)
        {
            if(_tTextureFunc != null)
                return _tTextureFunc(TRANSTION_GENERIC, metadata, environment);
            return TRANSTION_GENERIC;
        }

        /// <summary>
        /// Draws this tile.
        /// </summary>
        /// <param name="batch">The batch to draw with.</param>
        /// <param name="mapPos">The position this tile is on.</param>
        /// <param name="map">The map the tile is in.</param>
        /// <param name="metadata">Metadata associated with this position.</param>
        /// <param name="environment">The environment the map is in.</param>
        public virtual void Draw(Engine.Game game, Point mapPos, Map map, SortedDictionary<string, string> metadata, Map.Environment environment, Color? color = null)
        {
            TileAtlas.Region region = Map.Atlas[TextureName(metadata, environment)];

            game.Batch.Texture(
                new Vector2(mapPos.X * 16, mapPos.Y * 16),
                region.Texture,
                color.HasValue ? color.Value : Color.White,
                Vector2.One,
                region.Rectangle);
        }

        /// <summary>
        /// A second draw pass. Called after transitions have been drawn.
        /// </summary>
        /// <param name="batch">The batch to draw with.</param>
        /// <param name="mapPos">The position this tile is on.</param>
        /// <param name="map">The map the tile is in.</param>
        /// <param name="metadata">Metadata associated with this position.</param>
        /// <param name="environment">The environment the map is in.</param>
        public virtual void DrawAfterTransition(Engine.Game game, Point mapPos, Map map, SortedDictionary<string, string> metadata, Map.Environment environment, Color? color = null)
        {
        }

        /// <summary>
        /// If this tile has any editable attributes.
        /// </summary>
        public virtual bool HasEditableAttributes
        {
            get { return _attribEditorsFunc != null;  }
        }

        /// <param name="state">The map editor state to create the editors in.</param>
        /// <param name="currentY">The current Y position the editor is placing elements in.</param>
        /// <returns>Tile attribute editors for this tile.</returns>
        public virtual TileAttributeEditor[] AttributeEditors(MapEditorState state, ref int currentY)
        {
            if(_attribEditorsFunc != null)
                return _attribEditorsFunc(state, ref currentY);
            return null;
        }

        /// <summary>
        /// Events for this tile in the world. Can return null.
        /// </summary>
        public virtual TileEventBase Events
        {
            get; set;
        }

        private static Dictionary<string, Tile> _tilesTerrain = new Dictionary<string, Tile>();
        private static Dictionary<string, Tile> _tilesDecoration = new Dictionary<string, Tile>();
        private static Dictionary<string, Tile> _tilesNPC = new Dictionary<string, Tile>();
        private static Dictionary<string, Tile> _tilesControl = new Dictionary<string, Tile>();

        private static Dictionary<string, Tile> LayerToDictionary(MapLayer layer)
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

        public Tile(string id, MapLayer layer, string name, string textureName, bool solid = false, int transitionPriority = 1000, bool createGlobally = true)
        {            
            ID = id;
            Layer = layer;
            _name = name;
            _textureName = textureName;
            _solid = solid;
            _transitionPriority = transitionPriority;

            if(createGlobally)
            {
                if(LayerToDictionary(layer).ContainsKey(ID))
                    throw new ArgumentException("ID (" + id + "," + layer + ") already in use by another tile.");

                LayerToDictionary(layer).Add(ID, this);
            }
        }

        /// <summary>
        /// Generates a unique identifer for this tile based on the metadata and environment.
        /// </summary>
        /// <param name="metadata">Metadata for the tile being accessed.</param>
        /// <param name="environment">The environment the tile is in.</param>
        /// <returns>A unique identifier.</returns>
        public long UniqueIdentity(SortedDictionary<string, string> metadata, Map.Environment environment)
        {
            int dhash = 0;
            if(metadata == null)
                dhash = "".GetHashCode();
            else
            {
                foreach(KeyValuePair<string, string> pair in metadata)
                    dhash ^= pair.Key.GetHashCode() ^ pair.Value.GetHashCode();
            }

            long id = dhash & 0xFFFF;
            id |= (long)ID.GetHashCode() << 16;
            id |= (long)environment << 48;
            id |= (long)Layer << 56;
            
            return id;
        }

        /// <summary>
        /// Finds a tile by ID.
        /// </summary>
        /// <param name="id">The ID of the tile.</param>
        /// <param name="layer">The layer the tile belongs to.</param>
        /// <returns>The tile, or an exception.</returns>
        public static Tile Find(string id, MapLayer layer)
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
        public static bool Exists(string id, MapLayer layer)
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
