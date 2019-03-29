using System;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SDL2;
using TeamStor.Engine;
using TeamStor.AOD.Gameplay.World;
using Game = TeamStor.Engine.Game;
using SpriteBatch = TeamStor.Engine.Graphics.SpriteBatch;

namespace TeamStor.AOD
{
	/// <summary>
	/// Debug state for opening a map from the maps/ folder.
	/// </summary>
	public class OpenMapState : GameState
	{
		private GameState _lastState;
		private string _targetMap = "";
        private string[] _targetMaps = new string[6];

		public string[] TargetMaps
		{
			get
			{
                _targetMaps[0] = Settings.SettingsDirectory + "/maps/" + _targetMap + ".json";
                _targetMaps[1] = AppDomain.CurrentDomain.BaseDirectory + "data/maps/" + _targetMap + ".json";
                _targetMaps[2] = _targetMap + ".json";
                _targetMaps[3] = Settings.SettingsDirectory + "/maps/" + _targetMap;
                _targetMaps[4] = AppDomain.CurrentDomain.BaseDirectory + "data/maps/" + _targetMap;
                _targetMaps[5] = _targetMap;

                return _targetMaps;
			}
		}
		
		public override void OnEnter(GameState previousState)
		{
			_lastState = previousState;
			TextInputEXT.TextInput += OnTextInput;
			TextInputEXT.StartTextInput();
		}

		private void OnTextInput(char c)
		{
			if(c == (char) 22)
				_targetMap += SDL.SDL_GetClipboardText();
			if(c == '\b')
			{
				if(_targetMap.Length > 0)
					_targetMap = _targetMap.Substring(0, _targetMap.Length - 1);
			}
			else if(Char.IsLetterOrDigit(c) || Char.IsPunctuation(c) || c == ' ')
				_targetMap += c == ' ' ? '_' : c;
		}

		public override void OnLeave(GameState nextState)
		{
			TextInputEXT.TextInput -= OnTextInput;
			TextInputEXT.StopTextInput();
		}

        private string FindClosestMatch()
        {
            foreach(string s in TargetMaps)
            {
                if(File.Exists(s))
                    return s;
            }

            return "";
        }

		public override void Update(double deltaTime, double totalTime, long count)
		{
			if(Input.KeyPressed(Keys.Escape))
			{
				typeof(Game).GetField("_state", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Game, _lastState);
				OnLeave(_lastState);
			}

			if(!string.IsNullOrEmpty(FindClosestMatch()) && Input.KeyPressed(Keys.Enter))
			{
				WorldState state = new WorldState(Map.Load(FindClosestMatch()), "Player");
				_lastState.OnLeave(state);

				Game.CurrentState = state;
			}
		}

		public override void FixedUpdate(long count)
		{
		}

		public override void Draw(SpriteBatch batch, Vector2 screenSize)
		{
			_lastState.Draw(batch, screenSize);
			batch.Rectangle(new Rectangle(0, 0, (int)screenSize.X, (int)screenSize.Y), Color.Black * 0.85f);
						
			Vector2 measure = Game.DefaultFonts.MonoBold.Measure(24, _targetMap + "|");
			batch.Text(SpriteBatch.FontStyle.MonoBold, 24, _targetMap + (Game.TotalFixedUpdates % 40 > 20 ? "|" : " "), new Vector2(screenSize.X / 2 - measure.X / 2, 180), Color.White);

            string match = FindClosestMatch();
            bool exists = File.Exists(match);
            string msg = match.Replace("\\", "/") + (Map.IsPreloaded(match) ? " (preloaded)" : "");
            if(!exists)
                msg = "No map with name \"" + _targetMap + "\" found in any folder";

            measure = Game.DefaultFonts.MonoBold.Measure(14, msg);
			batch.Text(SpriteBatch.FontStyle.MonoBold, 14, msg, new Vector2(screenSize.X / 2 - measure.X / 2, 210), exists ? Color.White * 0.7f : Color.Red * 0.7f);
        }
	}
}