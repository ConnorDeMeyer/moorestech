using System;
using Core.Item.Config;
using Game.Quest.Interface;
using MainGame.Basic.Quest;
using MainGame.UnityView.UI.Inventory.Element;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MainGame.UnityView.UI.Quest
{
    public class QuestElement : MonoBehaviour
    {
        [SerializeField] private TMP_Text itemName;
        [SerializeField] private Button questButton;
        
        [SerializeField] private GameObject questCompleted;
        [SerializeField] private GameObject completedAndNotReadrded;

        private QuestProgressData _questProgressData = new QuestProgressData(false, false, false);
        
        public void SetQuest(QuestConfigData questConfigData,Action<QuestConfigData,QuestProgressData> onClick)
        {
            //もうちょっとちゃんとしたUI設定を行うようにする
            var rectTransform = GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(questConfigData.UiPosition.X, questConfigData.UiPosition.Y);
            itemName.text = questConfigData.QuestName;
            
            gameObject.name = questConfigData.QuestId;
            
            questButton.onClick.AddListener(() =>
            {
                onClick?.Invoke(questConfigData,_questProgressData);
            });
        }

        public void SetProgress(QuestProgressData questProgressData)
        {
            _questProgressData = questProgressData; 
            
            questCompleted.SetActive(questProgressData.IsComplete);
            completedAndNotReadrded.SetActive(questProgressData.IsRewardEarnbable);
        }
    }
}