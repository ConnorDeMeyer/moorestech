using System.Collections.Generic;
using Client.Skit.SkitTrack;
using DG.Tweening;
using UnityEngine;

namespace Client.Skit
{
    public class SkitCharacter : MonoBehaviour
    {
        [SerializeField] private AudioSource voiceAudioSource;
        [SerializeField] private SkinnedMeshRenderer faceSkinnedMeshRenderer;
        [SerializeField] private Animator animator;

        public void Initialize(Transform parent, string name)
        {
            gameObject.name = name + " (StoryCharacter)";
            transform.SetParent(parent);
        }

        public void SetTransform(Vector3 position, Vector3 rotation)
        {
            transform.position = position;
            transform.eulerAngles = rotation;
        }

        public void PlayAnimation(string animationName)
        {
            animator.SetTrigger(animationName);
        }

        public void PlayVoice(AudioClip voiceClip)
        {
            voiceAudioSource.clip = voiceClip;
            voiceAudioSource.Play();
        }

        public void StopVoice()
        {
            voiceAudioSource.Stop();
        }

        public void SetEmotion(EmotionType emotion, float duration)
        {
            Dictionary<int, float> blendShapeData = ToBlendShapeData(emotion);

            // Tween BlendShape
            foreach (var (key, value) in blendShapeData)
            {
                DOTween.To(
                    () => faceSkinnedMeshRenderer.GetBlendShapeWeight(key),
                    x => faceSkinnedMeshRenderer.SetBlendShapeWeight(key, x),
                    value,
                    duration);
            }

            #region Internal

            Dictionary<int, float> ToBlendShapeData(EmotionType emotionType)
            {
                return emotionType switch
                {
                    EmotionType.Normal => new Dictionary<int, float> { { 11, 0f } },
                    EmotionType.Happy => new Dictionary<int, float> { { 11, 100 } },
                };
            }

            #endregion
        }
    }
}