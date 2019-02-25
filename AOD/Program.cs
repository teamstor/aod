using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamStor.Engine;
using Game = TeamStor.Engine.Game;
using SpriteBatch = TeamStor.Engine.Graphics.SpriteBatch;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using TeamStor.Engine.Coroutine;
using TeamStor.AOD.Editor;
using TeamStor.AOD.Gameplay;
using TeamStor.Engine.Internal;

namespace TeamStor.AOD
{
    static class Program
    {

        public const string VERSION = "1.0 (beta)";
        
        [STAThreadAttribute]
        private static void Main()
        {            
            Settings.Load();

            Game game = Game.Run(new PreloaderState(), "data", true);
            
            if(game.Fullscreen != Settings.Fullscreen)
                game.Fullscreen = Settings.Fullscreen;

            MediaPlayer.Volume = MathHelper.Clamp((float)(Settings.MasterVolume * Settings.MusicVolume), 0.0f, 1.0f);
            
            // makes sure all tiles and npcs are available at startup
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(DefaultTiles).TypeHandle);
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(WaterTiles).TypeHandle);
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(GroundTiles).TypeHandle);
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(FoliageTiles).TypeHandle);
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(ControlTiles).TypeHandle);
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(InsideTiles).TypeHandle);
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(CityTiles).TypeHandle);
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(NPCs).TypeHandle);

            game.OnUpdateAfterState += OnUpdate;

            if(System.Diagnostics.Debugger.IsAttached)
            {
                game.Run();
                game.OnUpdateAfterState -= OnUpdate;
                game.Dispose();
            }
            else
            {
                try
                {
                    game.Run();
                    game.OnUpdateAfterState -= OnUpdate;
                    game.Dispose();
                }
                catch(Exception e)
                {
                    game.OnUpdateAfterState -= OnUpdate;
                    game.Dispose();

                    Game recoveryGame = Game.Run(new CrashRecoveryState(game, e), "data", false);
                    recoveryGame.Run();
                    recoveryGame.Dispose();
                }
            }

            Settings.Save();
        }

        private static void OnUpdate(object sender, Game.UpdateEventArgs e)
        {
            Game game = sender as Game;

            if(game.Window.Title != "Age of Darkness")
                game.Window.Title = "Age of Darkness";

            if(Settings.Fullscreen != game.Fullscreen)
                Settings.Fullscreen = game.Fullscreen;
            
            MediaPlayer.Volume = MathHelper.Clamp((float)(Settings.MasterVolume * Settings.MusicVolume), 0.0f, 1.0f);
            
            if(game.Input.Key(Keys.LeftShift) && 
                game.Input.KeyPressed(Keys.F8) && 
                game.CurrentState.GetType() != typeof(MapEditorState)&& 
                game.CurrentState.GetType() != typeof(PreloaderState) && 
                game.CurrentState.GetType() != typeof(TeamStorLogoState))
                game.CurrentState = new MapEditorState();

            if(game.Input.Key(Keys.LeftShift) &&
               game.Input.KeyPressed(Keys.F9) && 
               game.CurrentState.GetType() != typeof(OpenMapState) && 
               game.CurrentState.GetType() != typeof(PreloaderState) && 
               game.CurrentState.GetType() != typeof(TeamStorLogoState))
            {
                OpenMapState state = new OpenMapState();
                state.Game = game;
                GameState prevState = game.CurrentState;
                
                // do it manually so we don't call OnLeave in the other state
                typeof(Game).GetField("_state", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(game, state);
                state.OnEnter(prevState);
            }
        }

        public static Vector2 ScaleBatch(SpriteBatch batch)
        {
            double scale = Math.Min((double)batch.Device.Viewport.Width / (1920 / 4), (double)batch.Device.Viewport.Height / (1080 / 4));

            if(scale > 1)
                scale = Math.Floor(scale);
                        
            Point size = new Point((int)((1920 / 4) * scale), (int)((1080 / 4) * scale));
            Rectangle rectangle = new Rectangle(batch.Device.Viewport.Width / 2 - size.X / 2, batch.Device.Viewport.Height / 2 - size.Y / 2, size.X, size.Y);

            batch.Transform = Matrix.CreateScale((float)scale) * Matrix.CreateTranslation(rectangle.X, rectangle.Y, 0);
            batch.SamplerState = SamplerState.PointClamp;
            
            return new Vector2(1920 / 4, 1080 / 4);
        }
        
        public static void BlackBorders(SpriteBatch batch)
        {
            double scale = Math.Min((double)batch.Device.Viewport.Width / (1920 / 4), (double)batch.Device.Viewport.Height / (1080 / 4));

            if(scale > 1)
                scale = Math.Floor(scale);
                        
            Point size = new Point((int)((1920 / 4) * scale), (int)((1080 / 4) * scale));
            Rectangle rectangle = new Rectangle(batch.Device.Viewport.Width / 2 - size.X / 2, batch.Device.Viewport.Height / 2 - size.Y / 2, size.X, size.Y);
            
            batch.Scissor = null;
            batch.Transform = Matrix.Identity;
            
            batch.Rectangle(new Rectangle(0, 0, batch.Device.Viewport.Width, rectangle.Y), Color.Black);
            batch.Rectangle(new Rectangle(0, rectangle.Y + rectangle.Height, batch.Device.Viewport.Width, batch.Device.Viewport.Height), Color.Black);
            batch.Rectangle(new Rectangle(0, 0, rectangle.X, batch.Device.Viewport.Height), Color.Black);
            batch.Rectangle(new Rectangle(rectangle.X + rectangle.Width, 0, batch.Device.Viewport.Width, batch.Device.Viewport.Height), Color.Black);
        }
    }

    public static class Methods
    {
        public static Point ToPoint(this Vector2 vector)
        {
            return new Point((int)vector.X, (int)vector.Y);
        }
        
        public static Vector2 ToVector2(this Point point)
        {
            return new Vector2(point.X, point.Y);
        }
        
        public static bool Contains(this Rectangle rectangle, Vector2 vec)
        {
            return rectangle.Contains(vec.ToPoint());
        }
    }
}
