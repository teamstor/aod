using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using TeamStor.Engine;
using TeamStor.Engine.Graphics;
using Microsoft.Xna.Framework.Graphics;

using SpriteBatch = TeamStor.Engine.Graphics.SpriteBatch;
using Microsoft.Xna.Framework.Input;

namespace TeamStor.RPG.Gameplay.World
{
    /// <summary>
    /// Gameplay state the game is in while the player is walking around in the world.
    /// </summary>
    public class WorldState : GameState
    {
        /// <summary>
        /// The player.
        /// </summary>
        public Player Player
        {
            get; private set;
        }

        /// <summary>
        /// The camera following the player.
        /// </summary>
        public Camera Camera
        {
            get; private set;
        }

        private List<NPC> _npcs = new List<NPC>();

        /// <summary>
        /// NPCs in the world.
        /// </summary>
        public IReadOnlyList<NPC> NPCs
        {
            get
            {
                return _npcs;
            }
        }

        /// <summary>
        /// Spawns a new NPC in the world.
        /// </summary>
        /// <param name="template">The template to create the NPC from.</param>
        /// <param name="position">The position to spawn the NPC at.</param>
        /// <returns>The newly spawned NPC.</returns>
        public NPC SpawnNPC(NPCTemplate template, Point position)
        {
            NPC npc = new NPC(this, template);
            npc.MoveInstantly(position);
            return npc;
        }

        /// <summary>
        /// Map of the world.
        /// </summary>
        public Map Map
        {
            get; private set;
        }

        public WorldState(Map map)
        {
            Map = map;

            if(Map.TransitionCache != null)
                Map.TransitionCache.Clear();
        }

        public override void OnEnter(GameState previousState)
        {
            Player = new Player(this);
            Camera = new Camera(this);

            Game.IsMouseVisible = false;
        }

        public override void OnLeave(GameState nextState)
        {
        }

        public override void Update(double deltaTime, double totalTime, long count)
        {
            Point lastPlayerPos = Player.Position;
            Player.Update(deltaTime, totalTime, count);

            foreach(Tile.MapLayer layer in Enum.GetValues(typeof(Tile.MapLayer)))
            {
                TileEventBase oldEvents = Tile.Find(Map[layer, lastPlayerPos.X, lastPlayerPos.Y], layer).Events;
                TileEventBase newEvents = Tile.Find(Map[layer, Player.Position.X, Player.Position.Y], layer).Events;

                if(Player.Position != lastPlayerPos)
                {
                    oldEvents?.OnWalkLeave(Map.GetMetadata(layer, lastPlayerPos.X, lastPlayerPos.Y), this, lastPlayerPos);
                    newEvents?.OnWalkEnter(Map.GetMetadata(layer, Player.Position.X, Player.Position.Y), this, Player.Position);
                }
            }
        }

        public override void FixedUpdate(long count)
        {
            foreach(Tile.MapLayer layer in Enum.GetValues(typeof(Tile.MapLayer)))
            {
                TileEventBase events = Tile.Find(Map[layer, Player.Position.X, Player.Position.Y], layer).Events;
                events?.OnStandingOn(Map.GetMetadata(layer, Player.Position.X, Player.Position.Y), this, Player.Position, count);
            }
        }

        public override void Draw(SpriteBatch batch, Vector2 screenSize)
        {
            screenSize = Program.ScaleBatch(batch);

            batch.SamplerState = SamplerState.PointWrap;
            batch.Transform = Matrix.CreateTranslation(Camera.Offset.X, Camera.Offset.Y, 0) * batch.Transform;
            
            // TODO: följ kameran
            Rectangle drawRectangle = new Rectangle(-1000, -1000, Map.Width * 16 + 2000, Map.Height * 16 + 2000);
            batch.Texture(drawRectangle, Assets.Get<Texture2D>("tiles/water/" + (int)((Game.Time * 2) % 4) + ".png"), Color.White, drawRectangle);

            Map.Draw(Tile.MapLayer.Terrain, Game, new Rectangle((int)-Camera.Offset.X, (int)-Camera.Offset.Y, (int)screenSize.X, (int)screenSize.Y));

            foreach(NPC npc in NPCs)
                npc.Draw(batch);
            Player.Draw(batch);

            Map.Draw(Tile.MapLayer.Decoration, Game, new Rectangle((int)-Camera.Offset.X, (int)-Camera.Offset.Y, (int)screenSize.X, (int)screenSize.Y));

            Program.BlackBorders(batch);
        }
    }
}
