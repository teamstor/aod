using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TeamStor.Engine.Coroutine;
using TeamStor.Engine.Graphics;
using TeamStor.RPG.Gameplay.World;
using System;
using TeamStor.Engine.Tween;
using TeamStor.RPG.Gameplay.World.UI;

namespace TeamStor.RPG
{
	public class TextBoxEvents : TileEventBase
	{
		public TextBoxEvents(Tile tile) : base(tile)
		{
		}

		private void DoTextBox(SortedDictionary<string, string> metadata, WorldState world, Point mapPos)
		{
            string text = metadata != null && metadata.ContainsKey("value") ? metadata["value"] :
                "ERROR!!! VA FAN DET �R ERROR!!! F�R HELVETE ERROR!!!\n(du m�ste s�tta v�rdet av textboxen i map editorn)";

            TextBox.Show(world, new TextBoxContent
            {
                Speaker = metadata != null && metadata.ContainsKey("speaker") ? metadata["speaker"] : "Unknown",
                Text = text
            });
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