using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TeamStor.Engine.Coroutine;
using TeamStor.Engine.Graphics;
using TeamStor.RPG.Gameplay.World;

namespace TeamStor.RPG
{
	public class TextBoxEvents : TileEventBase
	{
		public TextBoxEvents(TextBoxTile tile) : base(tile)
		{
		}

		private void DrawHook(object sender, WorldState.DrawEventArgs args)
		{
			SpriteBatch batch = args.Batch;
			Vector2 screenSize = args.ScreenSize;

			batch.Text(SpriteBatch.FontStyle.Bold, 16, "AAAAAAAAAAAA", new Vector2(4, 4), Color.White);
		}

		private IEnumerator<ICoroutineOperation> TextBoxCoroutine(object worldObj)
		{
			WorldState world = worldObj as WorldState;
			
			world.Paused = true;
			world.DrawHook += DrawHook;

			while(!world.Game.Input.KeyPressed(Keys.Enter))
			{
				yield return null;
			}
			
			world.DrawHook -= DrawHook;
			world.Paused = false;
		}
		
		private void DoTextBox(SortedDictionary<string, string> metadata, WorldState world, Point mapPos)
		{
			world.Coroutine.Start(TextBoxCoroutine, world);
		}
		
		public override void OnInteract(SortedDictionary<string, string> metadata, WorldState world, Point mapPos)
		{
			if(metadata == null || !metadata.ContainsKey("needs-user-interaction") || metadata["needs-user-interaction"] == "True")
				DoTextBox(metadata, world, mapPos);
		}
		
		public override void OnWalkEnter(SortedDictionary<string, string> metadata, WorldState world, Point mapPos)
		{
			if(metadata != null && metadata.ContainsKey("needs-user-interaction") && metadata["needs-user-interaction"] == "False")
				DoTextBox(metadata, world, mapPos);
		}

		public override void OnStandingOn(SortedDictionary<string, string> metadata, WorldState world, Point mapPos, long tickCount)
		{
		}

		public override void OnWalkLeave(SortedDictionary<string, string> metadata, WorldState world, Point mapPos)
		{
		}
	}
}