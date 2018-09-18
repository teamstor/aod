using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamStor.Engine.Graphics;
using TeamStor.RPG.Gameplay.World;

using SpriteBatch = TeamStor.Engine.Graphics.SpriteBatch;

namespace TeamStor.RPG.Gameplay
{
    /// <summary>
    /// Player class for WorldState and CombatState.
    /// </summary>
    public class Player : PositionedEntity
    {
        public Player(WorldState world) : base(world)
        {
            MoveInstantly(new Point(0, 0));

            for(int x = 0; x < world.Map.Width; x++)
            {
                for(int y = 0; y < world.Map.Height; y++)
                {
                    if(world.Map[Tile.MapLayer.Control, x, y] == Tiles.Control.SpawnPoint.ID)
                        MoveInstantly(new Point(x, y));
                }
            }

            NextPosition = Position;
        }

        public void Draw(SpriteBatch batch)
        {
            batch.Texture(WorldPosition, World.Game.Assets.Get<Texture2D>("npc/pig/front0.png"));
        }
    }
}
