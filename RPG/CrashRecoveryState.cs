using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using TeamStor.Engine;
using TeamStor.Engine.Graphics;

using Game = TeamStor.Engine.Game;
using Microsoft.Xna.Framework.Graphics;
using SpriteBatch = TeamStor.Engine.Graphics.SpriteBatch;
using static TeamStor.Engine.Graphics.SpriteBatch;
using TeamStor.RPG.Editor;
using System.IO;

namespace TeamStor.RPG
{
    /// <summary>
    /// State that runs when an exception occurs in the game.
    /// Can recover maps from the map editor.
    /// </summary>
    public class CrashRecoveryState : GameState
    {
        private enum RecoveryState
        {
            Nothing,
            Recovering,
            RecoveredMapData
        }

        private RecoveryState _recoveryState = RecoveryState.Nothing;
        private string _mapTempSaveName;

        private float _monkeyFrame = 0;

        /// <summary>
        /// Game that crashed.
        /// </summary>
        public Game RecoveryGame
        {
            get; private set;
        }

        /// <summary>
        /// Exception that occured.
        /// </summary>
        public Exception Exception
        {
            get; private set;
        }

        /// <summary>
        /// Current frame of the angry monkey.
        /// </summary>
        public int MonkeyAnimationFrame
        {
            get
            {
                return (int)Math.Floor(_monkeyFrame);
            }
        }

        public CrashRecoveryState(Game game, Exception e)
        {
            RecoveryGame = game;
            Exception = e;

            if(game.CurrentState != null && game.CurrentState.GetType() == typeof(MapEditorState))
            {
                _recoveryState = RecoveryState.Recovering;
                Task.Run(() =>
                {
                    try
                    {
                        _mapTempSaveName = Environment.GetFolderPath(Environment.SpecialFolder.Desktop).Replace("\\", "/") + "/" + new Random().Next() + ".map";
                        ((MapEditorState)game.CurrentState).Map.Save(File.OpenWrite(_mapTempSaveName));
                        _recoveryState = RecoveryState.RecoveredMapData;
                    }
                    catch
                    {
                        _recoveryState = RecoveryState.Nothing;
                    }
                });
            }
        }

        public override void OnEnter(GameState previousState)
        {
        }

        public override void OnLeave(GameState nextState)
        {
        }

        public override void FixedUpdate(long count)
        {
        }

        public override void Update(double deltaTime, double totalTime, long count)
        {
            _monkeyFrame += (float)deltaTime * 10;

            while(MonkeyAnimationFrame >= 7)
                _monkeyFrame -= 7;
        }

        public override void Draw(SpriteBatch batch, Vector2 screenSize)
        {
            batch.Texture(new Vector2(screenSize.X / 2 - 160 / 2, screenSize.Y / 4 - 80), Assets.Get<Texture2D>("crash_recovery/angrymonkey" + MonkeyAnimationFrame + ".png"), Color.White);

            Vector2 measure = Game.DefaultFonts.Bold.Measure(20, Exception.GetType().Name + " occured in " + RecoveryGame.CurrentState.GetType().Name);
            batch.Text(FontStyle.Bold, 20, Exception.GetType().Name + " occured in " + RecoveryGame.CurrentState.GetType().Name, new Vector2(screenSize.X / 2 - measure.X / 2, screenSize.Y / 4 + 120), Color.White);

            measure = Game.DefaultFonts.Normal.Measure(16, "\"" + Exception.Message + "\"");
            batch.Text(FontStyle.Normal, 16, "\"" + Exception.Message + "\"", new Vector2(screenSize.X / 2 - measure.X / 2, screenSize.Y / 4 + 150), Color.LightGray);

            measure = Game.DefaultFonts.Mono.Measure(10, Exception.StackTrace);
            batch.Text(FontStyle.Mono, 10, Exception.StackTrace, new Vector2(screenSize.X / 2 - measure.X / 2, screenSize.Y / 4 + 180), Color.LightGray);

            string text = "Nothing was recovered from the game";
            if(_recoveryState == RecoveryState.Recovering)
                text = "Attempting to recover save/map data from the game...";
            if(_recoveryState == RecoveryState.RecoveredMapData)
                text = "Recovered map data from the map editor (to \"" + _mapTempSaveName + "\")";

            measure = Game.DefaultFonts.Bold.Measure(20, text);
            batch.Text(FontStyle.Bold, 20, text, new Vector2(screenSize.X / 2 - measure.X / 2, screenSize.Y - measure.Y - 20), Color.White);
        }
    }
}
