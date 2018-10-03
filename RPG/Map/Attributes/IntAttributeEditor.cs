using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamStor.Engine.Tween;
using TeamStor.RPG.Editor;

namespace TeamStor.RPG.Attributes
{
    public class IntAttributeEditor : TileAttributeEditor
    {
        public ChoiceField ChoiceField;

        public override string ValueAsString
        {
            get
            {
                return ChoiceField.Choice.ToString();
            }
        }

        public override bool IsDefaultValue
        {
            get
            {
                return ChoiceField.Choice == ChoiceField.MinValue;
            }
        }

        public IntAttributeEditor(string name, MapEditorState state, int minValue, int maxValue, ref int currentY) : base(name, state, ref currentY)
        {
            State.ChoiceFields.Add("editor-" + GetType().Name + "-" + Name, ChoiceField = new ChoiceField()
            {
                Game = state.Game,
                Label = name + ": ",
                // https://stackoverflow.com/questions/4138454/elegant-way-to-transform-arrays-in-c
                Choices = null,
                Choice = minValue,
                MinValue = minValue,
                MaxValue = maxValue,
                Position = new TweenedVector2(state.Game, new Vector2(state.Game.GraphicsDevice.Viewport.Bounds.Width / 2 - 300 / 2, currentY)),
                Font = state.Game.DefaultFonts.Bold,
                Width = 300
            });

            currentY += ChoiceField.Rectangle.Height + 4;
        }

        public override void ValueFromExistingTile(string value)
        {
            ChoiceField.Choice = int.Parse(value);
        }

        public override void Dispose()
        {
            State.ChoiceFields.Remove("editor-" + GetType().Name + "-" + Name);
        }
    }
}
