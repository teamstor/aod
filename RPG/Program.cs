using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamStor.Engine;
using Game = TeamStor.Engine.Game;
using SpriteBatch = TeamStor.Engine.Graphics.SpriteBatch;
using Microsoft.Xna.Framework.Input;
using TeamStor.RPG.Editor;

namespace TeamStor.RPG
{
    static class Program
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        public const string VERSION = "1.0 (beta)";

        [STAThreadAttribute]
        private static void Main()
        {
            // http://crsouza.com/2015/04/13/how-to-fix-blurry-windows-forms-windows-in-high-dpi-settings/
            if(Environment.OSVersion.Version.Major >= 6)
                SetProcessDPIAware();

            using(Game game = Game.Run(new Menu.MenuState(), "data", false))
            {
                System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(Tiles.Terrain).TypeHandle);
                System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(Tiles.Decoration).TypeHandle);
                System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(Tiles.NPC).TypeHandle);
                System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(Tiles.Control).TypeHandle);
                
                game.OnUpdateAfterState += OnUpdate;
                game.Run();
            }
        }

        private static void OnUpdate(object sender, Game.UpdateEventArgs e)
        {
            Game game = sender as Game;

            if(game.Input.Key(Keys.LeftShift) && 
                game.Input.KeyPressed(Keys.F8) && 
                game.CurrentState.GetType() != typeof(MapEditorState))
                game.CurrentState = new MapEditorState();
        }

        public static Vector2 ScaleBatch(SpriteBatch batch)
        {
            double scale = Math.Min((double)batch.Device.Viewport.Width / (1920 / 4), (double)batch.Device.Viewport.Height / (1080 / 4));

            if(scale > 1)
                scale = Math.Floor(scale);
                        
            Point size = new Point((int)((1920 / 4) * scale), (int)((1080 / 4) * scale));
            Rectangle rectangle = new Rectangle(new Point(batch.Device.Viewport.Width / 2 - size.X / 2, batch.Device.Viewport.Height / 2 - size.Y / 2), size);

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
            Rectangle rectangle = new Rectangle(new Point(batch.Device.Viewport.Width / 2 - size.X / 2, batch.Device.Viewport.Height / 2 - size.Y / 2), size);
            
            batch.Scissor = null;
            batch.Transform = Matrix.Identity;
            
            batch.Rectangle(new Rectangle(0, 0, batch.Device.Viewport.Width, rectangle.Y), Color.Black);
            batch.Rectangle(new Rectangle(0, rectangle.Y + rectangle.Height, batch.Device.Viewport.Width, batch.Device.Viewport.Height), Color.Black);
            batch.Rectangle(new Rectangle(0, 0, rectangle.X, batch.Device.Viewport.Height), Color.Black);
            batch.Rectangle(new Rectangle(rectangle.X + rectangle.Width, 0, batch.Device.Viewport.Width, batch.Device.Viewport.Height), Color.Black);
        }
    }
}
