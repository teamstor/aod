using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using TeamStor.Engine;
using System.IO;
using TeamStor.AOD.Gameplay.World;
using Microsoft.Xna.Framework.Graphics;
using TeamStor.AOD.Gameplay.Behavior;

namespace TeamStor.AOD.Gameplay
{
    /// <summary>
    /// Template for creating an NPC or an NPCTile.
    /// NPCTemplate.AsTile -> NPC.Tile -> NPCTemplate.FromTile 
    /// </summary>
    public class NPCTemplate
    {
        /// <summary>
        /// NPC tile 
        /// </summary>
        private class Tile : AOD.Tile
        {
            public Tile(string id, NPCTemplate npcTemplate) : 
                base(id, MapLayer.NPC, npcTemplate.Name, NPC.DOWN_TEXTURE.Replace("{name}", npcTemplate.ID).Replace("{i}", "0"), true, -1)
            {
                Template = npcTemplate;
            }

            public override void Draw(Engine.Game game, Point mapPos, Map map, TileMetadata metadata, Map.Environment environment, Color? color = null)
            {
                game.Batch.Texture(
                    new Vector2(mapPos.X * 16, mapPos.Y * 16),
                    Template.TextureForDirection(game, Direction.Down),
                    color.HasValue ? color.Value : Color.White);
            }

            /// <summary>
            /// Template this tile originates from.
            /// </summary>
            public NPCTemplate Template
            {
                get; private set;
            }
        }

        private Dictionary<Direction, int> _frameCounts = new Dictionary<Direction, int>();

        /// <summary>
        /// Name of the NPC that will be shown in dialogue.
        /// </summary>
        public string Name
        {
            get; private set;
        }
        
        /// <summary>
        /// ID of the NPC used for things such as texture names.
        /// E.g. ID = pig => Texture = data/npc/pig
        /// </summary>
        public string ID
        {
            get; private set;
        }

        /// <summary>
        /// This NPC as a tile template.
        /// </summary>
        public AOD.Tile TileTemplate
        {
            get; private set;
        }

        /// <summary>
        /// NPC behavior.
        /// Will be NullNPCBehavior if not overriden.
        /// </summary>
        public INPCBehavior Behavior
        {
            get; private set;
        }

        public NPCTemplate(string name, string id, INPCBehavior behavior)
        {
            Name = name;
            ID = id;

            TileTemplate = new Tile(ID, this);

            Behavior = behavior != null ? behavior : new NullNPCBehavior();

            foreach(Direction direction in Enum.GetValues(typeof(Direction)))
            {
                int i = 0;
                string textureTemplate = "";

                switch(direction)
                {
                    case Direction.Up:
                        textureTemplate = NPC.UP_TEXTURE;
                        break;

                    case Direction.Down:
                        textureTemplate = NPC.DOWN_TEXTURE;
                        break;

                    case Direction.Left:
                        textureTemplate = NPC.LEFT_TEXTURE;
                        break;

                    case Direction.Right:
                        textureTemplate = NPC.RIGHT_TEXTURE;
                        break;
                }

                while(File.Exists("data/" + textureTemplate.Replace("{name}", ID).Replace("{i}", i.ToString())))
                    i++;

                _frameCounts.Add(direction, i);
            }
        }

        /// <param name="direction">The direction to get a texture for.</param>
        /// <param name="frame">The frame of the animation to get. This will wrap around if you exceed the maximum frame count.</param>
        /// <returns>The texture for the selected direction.</returns>
        public Texture2D TextureForDirection(Engine.Game game, Direction direction, int frame = 0)
        {
            string textureTemplate = "";

            switch(direction)
            {
                case Direction.Up:
                    textureTemplate = NPC.UP_TEXTURE;
                    break;

                case Direction.Down:
                    textureTemplate = NPC.DOWN_TEXTURE;
                    break;

                case Direction.Left:
                    textureTemplate = NPC.LEFT_TEXTURE;
                    break;

                case Direction.Right:
                    textureTemplate = NPC.RIGHT_TEXTURE;
                    break;
            }

            return game.Assets.Get<Texture2D>(textureTemplate.Replace("{name}", ID).Replace("{i}", (frame % _frameCounts[direction]).ToString()));
        }

        /// <summary>
        /// Gets a template from a tile on the NPC layer.
        /// This function will fail if an invalid tile is passed.
        /// </summary>
        /// <param name="tile">A tile on the NPC layer.</param>
        /// <returns>The template.</returns>
        public static NPCTemplate FromTile(AOD.Tile tile)
        {
            if(tile.GetType() != typeof(Tile))
                throw new Exception("Cannot access NPCTemplate from a tile that doesn't have one!");

            return (tile as Tile).Template;
        }
    }
}
