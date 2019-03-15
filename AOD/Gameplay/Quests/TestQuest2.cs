namespace TeamStor.AOD.Gameplay.Quests
{
	public class TestQuest2 : Quest
	{
		public override string Name
		{
			get { return "Test Quest #2"; }
		}
		
		public override string ID
		{
			get { return "testquest2";  }
		}
		
		public override string Objective
		{
			get { return "Find the hidden sword\nof Thunderfury and\nbring it to Thrall.";  }
		}
		
		public override bool Fulfilled
		{
			get { return false; }
		}
		
		public override QuestPriority Priority
		{
			get { return QuestPriority.Main;  }
		}
		
		public override void OnQuestAdded(Player player)
		{
		}

		public override void OnQuestFinished(Player player)
		{
		}

		public override void OnQuestTick(Player player)
		{
		}
	}
}