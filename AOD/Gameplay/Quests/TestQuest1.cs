namespace TeamStor.AOD.Gameplay.Quests
{
	public class TestQuest1 : Quest
	{
		public override string Name
		{
			get { return "Test Quest #1"; }
		}
		
		public override string ID
		{
			get { return "testquest1";  }
		}
		
		public override string Objective
		{
			get { return "Kill 50 penguins";  }
		}
		
		public override bool Fulfilled
		{
			get { return true; }
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