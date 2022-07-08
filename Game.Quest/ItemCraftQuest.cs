using System;
using System.Collections.Generic;
using Core.Item;
using Core.Util;
using Game.Quest.Interface;

namespace Game.Quest
{
    public class ItemCraftQuest : IQuest
    {
        public QuestConfigData Quest { get; }
        public string QuestType { get; }
        public bool IsCompleted { get; }
        public bool AcquiredReward { get; }
        public event Action OnQuestCompleted;
    }
}