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
            Player.Update(deltaTime, totalTime, count);
        }

        public override void FixedUpdate(long count)
        {
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

            Player.Draw(batch);

            Map.Draw(Tile.MapLayer.Decoration, Game, new Rectangle((int)-Camera.Offset.X, (int)-Camera.Offset.Y, (int)screenSize.X, (int)screenSize.Y));

            Program.BlackBorders(batch);
        }
    }
}
