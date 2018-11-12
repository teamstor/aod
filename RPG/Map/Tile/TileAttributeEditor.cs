using System;
using Microsoft.Xna.Framework;
using TeamStor.RPG.Editor;

namespace TeamStor.RPG
{
	/// <summary>
	/// Editor for a tile attribute.
	/// </summary>
	public abstract class TileAttributeEditor : IDisposable
	{
		/// <summary>
		/// Name/key of the tile attribute.
		/// </summary>
		public virtual string Name { get; protected set; }
		
		/// <summary>
		/// The value of this editor as a string.
		/// </summary>
		public abstract string ValueAsString { get; }
		
		/// <summary>
		/// State this editor is editing in.
		/// </summary>
		public MapEditorState State { get; protected set; }

		/// <summary>
		/// If this editor currently has the default value.
		/// If this is true then the map editor can remove this key from the dictionary.
		/// If all editors have their default value the metadata slot will be removed for the tile being edited.
		/// </summary>
		public abstract bool IsDefaultValue { get; }

		protected TileAttributeEditor(string name, MapEditorState state, ref int currentY)
		{
			Name = name;
			State = state;
		}
		
		/// <summary>
		/// Sets this editor to a value an existing tile has.
		/// </summary>
		public abstract void ValueFromExistingTile(string value);
		
		/// <summary>
		/// Should destory elements in the map editor.
		/// </summary>
		public abstract void Dispose();
	}
}