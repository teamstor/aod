﻿using Microsoft.Xna.Framework;
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
    public class Player : LivingEntity
    {
        private double _landWhen = 0;
        private double _keyHeld;
        private Keys _keyQueued = Keys.None;

        public Player(WorldState world) : base(world)
        {
            Vitality = 10;
            Spirit = 10;

            Health = 100;
            Magicka = 100;

            Speed = 2.5;
            MoveInstantly(new Point(0, 0));

            for(int x = 0; x < world.Map.Width; x++)
            {
                for(int y = 0; y < world.Map.Height; y++)
                {
                    if(world.Map[Tile.MapLayer.Control, x, y] == ControlTiles.Spawnpoint)
                        MoveInstantly(new Point(x, y));
                }
            }

            NextPosition = Position;
        }

        public void Update(double deltaTime, double totalTime, long count)
        {
            if(!IsWalking)
            {
                Speed = World.Game.Input.Key(Keys.LeftShift) ? 3.5 : 2.5;
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

                if(_keyQueued != Keys.None)
                {
                    if(_keyQueued == Keys.Up)
                        Heading = Direction.Up;
                    if(_keyQueued == Keys.Down)
                        Heading = Direction.Down;
                    if(_keyQueued == Keys.Left)
                        Heading = Direction.Left;
                    if(_keyQueued == Keys.Right)
                        Heading = Direction.Right;

                    _keyHeld = 0.1f;
                    _keyQueued = Keys.None;
                }

                if(_lastHeading != Heading && _keyHeld < 0.1)
                    _landWhen = World.Game.Time + 0.1;

                if(_keyHeld >= 0.1)
                {
                    if(!World.IsPointBlocked(Position + Heading.ToPoint()))
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

                    foreach(NPC npc in World.NPCs)
                    {
                        if(npc.Position == interactionPoint)
                        {
                            bool isFacingPlayer =
                                Heading == Direction.Left && npc.Heading == Direction.Right ||
                                Heading == Direction.Right && npc.Heading == Direction.Left ||
                                Heading == Direction.Up && npc.Heading == Direction.Down ||
                                Heading == Direction.Down && npc.Heading == Direction.Up;

                            npc.Template.Behavior.OnInteract(npc, this, isFacingPlayer);
                        }
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
