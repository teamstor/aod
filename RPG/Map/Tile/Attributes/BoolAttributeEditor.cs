using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamStor.RPG.Editor;

namespace TeamStor.RPG.Attributes
{
    public class BoolAttributeEditor : EnumAttributeEditor<BoolAttributeEditor.BoolEnum>
    {
        // kinda dumb but allows you to set default to true or false
        public bool DefaultIsTrue = false;

        public enum BoolEnum
        {
            False,
            True
        }

        public override bool IsDefaultValue
        {
            get
            {
                return DefaultIsTrue ? !base.IsDefaultValue : base.IsDefaultValue;
            }
        }

        public BoolAttributeEditor(string name, MapEditorState state, bool defaultIsTrue, ref int currentY) : base(name, state, ref currentY)
        {
            DefaultIsTrue = defaultIsTrue;

            if(DefaultIsTrue)
                ChoiceField.Choice = 1;
        }
    }
}
