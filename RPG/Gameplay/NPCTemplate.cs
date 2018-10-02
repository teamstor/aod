using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using TeamStor.Engine;
using System.IO;
using TeamStor.RPG.Gameplay.World;
using Microsoft.Xna.Framework.Graphics;

namespace TeamStor.RPG.Gameplay
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
        private class Tile : RPG.Tile
        {
            public Tile(byte id, NPCTemplate npcTemplate) : 
                base(id, MapLayer.NPC, npcTemplate.Name, Point.Zero, true, -1)
            {
                Template = npcTemplate;
            }

            public override void Draw(Engine.Game game, Point mapPos, Map map, Dictionary<string, string> metadata, Map.Environment environment, Color? color = null)
            {
                game.Batch.Texture(
                    new Vector2(mapPos.X * 16, mapPos.Y * 16),
                    Template.TextureForDirection(game, Direction.Down),
                    color.HasValue ? color.Value : Color.White);
            }

            public override bool Filter(Map.Environment environment)
            {
                return true;
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

        public NPCTemplate(string name, string id)
        {
            Name = name;
            ID = id;

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
        /// Creates a tile on the NPC layer from this template.
        /// You can access this template with the function FromTile.
        /// </summary>
        /// <param name="id">The ID to give the tile.</param>
        /// <returns>The newly created tile.</returns>
        public RPG.Tile AsTile(byte id)
        {
            return new Tile(id, this);
        }

        /// <summary>
        /// Gets a template from a tile on the NPC layer.
        /// This function will fail if an invalid tile is passed.
        /// </summary>
        /// <param name="tile">A tile on the NPC layer.</param>
        /// <returns>The template.</returns>
        public static NPCTemplate FromTile(RPG.Tile tile)
        {
            if(tile.GetType() != typeof(Tile))
                throw new Exception("Cannot access NPCTemplate from a tile that doesn't have one!");

            return (tile as Tile).Template;
        }
    }
}
