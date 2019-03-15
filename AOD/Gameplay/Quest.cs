namespace TeamStor.AOD.Gameplay
{
	public enum QuestPriority
	{
		Main,
		Side
	}
	
	/// <summary>
	/// A quest for the player to fulfill.
	/// </summary>
	public abstract class Quest
	{
		/// <summary>
		/// The name of the quest, shown in the UI.
		/// </summary>
		public abstract string Name { get; }
		
		/// <summary>
		/// ID of the quest.
		/// </summary>
		public abstract string ID { get; }
		
		/// <summary>
		/// The objective of the quest.
		/// </summary>
		public abstract string Objective { get;  }
		
		/// <summary>
		/// If this quest is completely fulfilled.
		/// </summary>
		public abstract bool Fulfilled { get; }
		
		/// <summary>
		/// Priority in the quest list.
		/// </summary>
		public abstract QuestPriority Priority { get;  }
		
		/// <summary>
		/// Called when the quest is added to the players quest list.
		/// </summary>
		/// <param name="player">The player. The one and only.</param>
		public abstract void OnQuestAdded(Player player);
		
		/// <summary>
		/// Called when the player finishes the quest.
		/// </summary>
		/// <param name="player">The player. Wow.</param>
		public abstract void OnQuestFinished(Player player);
		
		/// <summary>
		/// Called every tick that the player has the quest active.
		/// </summary>
		/// <param name="player">The player.</param>
		public abstract void OnQuestTick(Player player);
	}
}