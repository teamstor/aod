using System;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TeamStor.Engine;
using TeamStor.RPG.Gameplay.World;
using Game = TeamStor.Engine.Game;
using SpriteBatch = TeamStor.Engine.Graphics.SpriteBatch;

namespace TeamStor.RPG
{
	/// <summary>
	/// Debug state for opening a map from the maps/ folder.
	/// </summary>
	public class OpenMapState : GameState
	{
		private GameState _lastState;
		private string _targetMap = "";
		
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
				_targetMap += e.Character == ' ' ? '_' : Char.ToLower(e.Character);
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

			if(File.Exists("data/maps/" + _targetMap + ".map") && Input.KeyPressed(Keys.Enter))
			{
				WorldState state = new WorldState(Map.Load(File.OpenRead("data/maps/" + _targetMap + ".map")));
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
			
			batch.Texture(new Vector2(screenSize.X / 2 - 72 / 2, 60), Assets.Get<Texture2D>("ui/load_map.png"), Color.White);
			
			Vector2 measure = Game.DefaultFonts.MonoBold.Measure(24, _targetMap + "|");
			batch.Text(SpriteBatch.FontStyle.MonoBold, 24, _targetMap + (Game.TotalFixedUpdates % 40 > 20 ? "|" : " "), new Vector2(screenSize.X / 2 - measure.X / 2, 180), Color.White);

			bool exists = File.Exists("data/maps/" + _targetMap + ".map");
			measure = Game.DefaultFonts.MonoBold.Measure(14, "data/maps/" + _targetMap + ".map");
			batch.Text(SpriteBatch.FontStyle.MonoBold, 14, "data/maps/" + _targetMap + ".map", new Vector2(screenSize.X / 2 - measure.X / 2, 210), exists ? Color.White * 0.7f : Color.Red * 0.7f);
		}
	}
}