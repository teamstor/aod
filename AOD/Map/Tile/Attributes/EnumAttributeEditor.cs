using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamStor.Engine.Tween;
using TeamStor.AOD.Editor;

namespace TeamStor.AOD.Attributes
{
    /// <summary>
    /// An attribute that switches between enum values.
    /// </summary>
    public class EnumAttributeEditor<T> : TileAttributeEditor where T : struct, IComparable, IConvertible, IFormattable
        // https://stackoverflow.com/questions/6438352/using-enum-as-generic-type-parameter-in-c-sharp
    {
        public ChoiceField ChoiceField;
        public List<T> Values;

        public T Value
        {
            get
            {
                return Values[ChoiceField.Choice];
            }
        }

        public override bool IsDefaultValue
        {
            get
            {
                return Value.Equals(default(T));
            }
        }

        public override string ValueAsString
        {
            get
            {
                return Value.ToString();
            }
        }

        public EnumAttributeEditor(string name, MapEditorState state, ref int currentY) : base(name, state, ref currentY)
        {
            Values = Enum.GetValues(typeof(T)).Cast<T>().ToList();

            State.ChoiceFields.Add("editor-" + GetType().Name + "-" + Name, ChoiceField = new ChoiceField()
            {
                Game = state.Game,
                Label = name + ": ",
                // https://stackoverflow.com/questions/4138454/elegant-way-to-transform-arrays-in-c
                Choices = Values.Select(e => e.ToString()).ToArray(),
                Choice = 0,
                Position = new TweenedVector2(state.Game, new Vector2(state.Game.GraphicsDevice.Viewport.Bounds.Width / 2 - 400 / 2, currentY)),
                Font = state.Game.DefaultFonts.Bold,
                Width = 400
            });

            currentY += ChoiceField.Rectangle.Height + 4;
        }

        public override void Dispose()
        {
            State.ChoiceFields.Remove("editor-" + GetType().Name + "-" + Name);
        }

        public override void ValueFromExistingTile(string value)
        {
            T v;
            Enum.TryParse(value, out v);

            for(int i = 0; i < ChoiceField.Choices.Length; i++)
            {
                if(Values[i].Equals(v))
                    ChoiceField.Choice = i;
            }
        }
    }
}
