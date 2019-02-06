using System;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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

		public string TargetMap
		{
			get
			{
				if(_targetMap.StartsWith("@"))
					return _targetMap.Remove(0, 1);

				if(_targetMap.StartsWith("!"))
					return Settings.SettingsDirectory + "/maps/" + _targetMap.Remove(0, 1);

				return "data/maps/" + _targetMap;
			}
		}
		
		public override void OnEnter(GameState previousState)
		{
			_lastState = previousState;
			Game.Window.TextInput += OnTextInput;
		}

		private void OnTextInput(object sender, TextInputEventArgs e)
		{
			if(e.Character == '\b')
			{
				if(_targetMap.Length > 0)
					_targetMap = _targetMap.Substring(0, _targetMap.Length - 1);
			}
			else if(Char.IsLetterOrDigit(e.Character) || Char.IsPunctuation(e.Character) || e.Character == ' ')
				_targetMap += e.Character == ' ' ? '_' : e.Character;
		}

		public override void OnLeave(GameState nextState)
		{
			Game.Window.TextInput -= OnTextInput;
		}

		public override void Update(double deltaTime, double totalTime, long count)
		{
			if(Input.KeyPressed(Keys.Escape))
			{
				typeof(Game).GetField("_state", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Game, _lastState);
				OnLeave(_lastState);
			}

			if(File.Exists(TargetMap) && Input.KeyPressed(Keys.Enter))
			{
				WorldState state = new WorldState(Map.Load(TargetMap));
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

			bool exists = File.Exists(TargetMap);
			measure = Game.DefaultFonts.MonoBold.Measure(14, TargetMap);
			batch.Text(SpriteBatch.FontStyle.MonoBold, 14, TargetMap, new Vector2(screenSize.X / 2 - measure.X / 2, 210), exists ? Color.White * 0.7f : Color.Red * 0.7f);
			
			measure = Game.DefaultFonts.Bold.Measure(18, "@ = No prefix\n! = User/settings directory");
			batch.Text(SpriteBatch.FontStyle.Bold, 18, "@ = No prefix\n! = User/settings directory", new Vector2(screenSize.X / 2 - measure.X / 2, 280), Color.White);
		}
	}
}