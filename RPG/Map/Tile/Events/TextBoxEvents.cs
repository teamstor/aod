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

		private void DoTextBox(TileMetadata metadata, WorldState world, Point mapPos)
		{
            string text = metadata != null && metadata.IsKeySet("value") ? metadata["value"] :
                "ERROR!!! VA FAN DET ÄR ERROR!!! FÖR HELVETE ERROR!!!\n(du måste sätta värdet av textboxen i map editorn)";

            TextBox.Show(world, new TextBoxContent
            {
                Speaker = metadata != null && metadata.IsKeySet("speaker") ? metadata["speaker"] : "Unknown",
                Text = text
            });
        }

        public override void OnInteract(TileMetadata metadata, WorldState world, Point mapPos)
		{
			if(metadata == null || !metadata.IsKeySet("needs-user-interaction") || metadata["needs-user-interaction"] == "True")
				DoTextBox(metadata, world, mapPos);
		}
		
		public override void OnWalkEnter(TileMetadata metadata, WorldState world, Point mapPos)
		{
			if(metadata != null && metadata["needs-user-interaction"] == "False")
				DoTextBox(metadata, world, mapPos);
		}

		public override void OnStandingOn(TileMetadata metadata, WorldState world, Point mapPos, long tickCount)
		{
		}

		public override void OnWalkLeave(TileMetadata metadata, WorldState world, Point mapPos)
		{
		}
	}
}