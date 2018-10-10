using System.Collections.Generic;
using Microsoft.Xna.Framework;
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
		}

		private IEnumerator<ICoroutineOperation> TextBoxCoroutine(WorldState world)
		{
			world.Paused = true;
			world.DrawHook += DrawHook;

			yield return null;
			
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