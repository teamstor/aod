using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamStor.Engine.Graphics;
using TeamStor.RPG.Gameplay.World;
using static TeamStor.Engine.Graphics.SpriteBatch;
using SpriteBatch = TeamStor.Engine.Graphics.SpriteBatch;

namespace TeamStor.RPG.Gameplay
{
    /// <summary>
    /// Player class for WorldState and CombatState.
    /// </summary>
    public class Player : PositionedEntity
    {
        private double _landWhen = 0;
        private double _keyHeld;

        public Player(WorldState world) : base(world)
        {
            Speed = 3;
            MoveInstantly(new Point(0, 0));

            for(int x = 0; x < world.Map.Width; x++)
            {
                for(int y = 0; y < world.Map.Height; y++)
                {
                    /*if(world.Map[Tile.MapLayer.Control, x, y] == Tiles.Control.SpawnPoint)
                        MoveInstantly(new Point(x, y)); TODO*/
                }
            }

            NextPosition = Position;
        }

        public void Update(double deltaTime, double totalTime, long count)
        {
            if(!IsWalking)
            {
                Direction _lastHeading = Heading;

                if(World.Game.Input.Key(Keys.Up))
                {
                    if(Heading != Direction.Up)
                        Heading = Direction.Up;
                    else if(_keyHeld == 0)
                        _keyHeld = 0.1;

                    _keyHeld += deltaTime;
                }
                else if(World.Game.Input.Key(Keys.Down))
                {
                    if(Heading != Direction.Down)
                        Heading = Direction.Down;
                    else if(_keyHeld == 0)
                        _keyHeld = 0.1;

                    _keyHeld += deltaTime;
                }
                else if(World.Game.Input.Key(Keys.Left))
                {
                    if(Heading != Direction.Left)
                        Heading = Direction.Left;
                    else if(_keyHeld == 0)
                        _keyHeld = 0.1;

                    _keyHeld += deltaTime;
                }
                else if(World.Game.Input.Key(Keys.Right))
                {
                    if(Heading != Direction.Right)
                        Heading = Direction.Right;
                    else if(_keyHeld == 0)
                        _keyHeld = 0.1;

                    _keyHeld += deltaTime;
                }
                else
                    _keyHeld -= deltaTime * 2;

                _keyHeld = MathHelper.Clamp((float)_keyHeld, 0.0f, 0.1f);

                if(_lastHeading != Heading && _keyHeld < 0.1)
                    _landWhen = World.Game.Time + 0.1;

                if(_keyHeld >= 0.1)
                {
                    if(!World.Map.IsPointBlocked(Position + Heading.ToPoint()))
                        MoveTo(Position + Heading.ToPoint());
                }

                if(World.Game.Input.KeyPressed(Keys.E))
                {
                    Point interactionPoint = Position + Heading.ToPoint();
                    interactionPoint.X = MathHelper.Clamp(interactionPoint.X, 0, World.Map.Width - 1);
                    interactionPoint.Y = MathHelper.Clamp(interactionPoint.Y, 0, World.Map.Height - 1);

                    foreach(Tile.MapLayer layer in Tile.CachedAllMapLayers)
                    {
                        TileEventBase events = World.Map[layer, interactionPoint.X, interactionPoint.Y].Events;
                        events?.OnInteract(World.Map.GetMetadata(layer, interactionPoint.X, interactionPoint.Y), World, interactionPoint);
                    }
                }
            }
        }

        public void Draw(SpriteBatch batch)
        {
            string texture = "front";
            if(Heading == Direction.Left)
                texture = "left";
            if(Heading == Direction.Right)
                texture = "right";
            if(Heading == Direction.Up)
                texture = "back";

            int frame = 0;
            if(IsWalking)
                frame = ((int)World.Game.TotalFixedUpdates / 10) % (Heading == Direction.Up || Heading == Direction.Down ? 3 : 2);

            batch.Texture(WorldPosition + new Vector2(0, _landWhen > World.Game.Time ? -1 : 0), World.Game.Assets.Get<Texture2D>("npc/pig/" + texture + frame + ".png"), Color.White);
        }
    }
}
