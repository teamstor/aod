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

        // TODO: byt ut med en kamera som smooth följer efter spelaren men om det går för snabbt så hoppar den direkt
        public Vector2 Camera
        {
            get; private set;
        }

        public Vector2 SmoothCamera;

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
        }

        public override void OnLeave(GameState nextState)
        {
        }

        public override void Update(double deltaTime, double totalTime, long count)
        {
            Vector2 camera = Camera;
            camera.X -= Input.Key(Keys.Left) ? -520 * (float)deltaTime : Input.Key(Keys.Right) ? 520 * (float)deltaTime : 0;
            camera.Y -= Input.Key(Keys.Up) ? -520 * (float)deltaTime : Input.Key(Keys.Down) ? 520 * (float)deltaTime : 0;
            Camera = camera;

            SmoothCamera = Vector2.LerpPrecise(SmoothCamera, Camera, (float)deltaTime * 20f);
        }

        public override void FixedUpdate(long count)
        {
        }

        public override void Draw(SpriteBatch batch, Vector2 screenSize)
        {
            screenSize = Program.ScaleBatch(batch);

            batch.SamplerState = SamplerState.PointWrap;
            batch.Transform = Matrix.CreateTranslation(SmoothCamera.X, SmoothCamera.Y, 0) * batch.Transform;
            
            // TODO: följ kameran
            Rectangle drawRectangle = new Rectangle(-1000, -1000, Map.Width * 16 + 2000, Map.Height * 16 + 2000);
            batch.Texture(drawRectangle, Assets.Get<Texture2D>("tiles/water/" + (int)((Game.Time * 2) % 4) + ".png"), Color.White, drawRectangle);

            Map.Draw(Tile.MapLayer.Terrain, Game, new Rectangle((int)-SmoothCamera.X, (int)-SmoothCamera.Y, (int)screenSize.X, (int)screenSize.Y));

            Player.Draw(batch);

            Map.Draw(Tile.MapLayer.Decoration, Game, new Rectangle((int)-Camera.X, (int)-Camera.Y, (int)screenSize.X, (int)screenSize.Y));

            Program.BlackBorders(batch);
        }
    }
}
