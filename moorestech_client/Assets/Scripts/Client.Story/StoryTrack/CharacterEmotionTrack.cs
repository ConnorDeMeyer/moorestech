using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using static System.Enum;

namespace Client.Story.StoryTrack
{
    public class CharacterEmotionTrack : IStoryTrack
    {
        
        public async UniTask<string> ExecuteTrack(StoryContext storyContext, List<string> parameters)
        {
            var characterKey = parameters[0];
            var emotion = (EmotionType)Parse(typeof(EmotionType), parameters[1]);
            var duration = float.Parse(parameters[2]);
            
            var character = storyContext.GetCharacter(characterKey);
            character.SetEmotion(emotion, duration);
            
            return null;
        }
    }
}