﻿using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace MainGame.UnityView.UI.Inventory.View.HotBar
{
    public class ProgressBarView : MonoBehaviour
    {
        private const float FadeTime = 0.1f;

        [SerializeField] private Slider slider;
        [SerializeField] private CanvasGroup canvasGroup;

        private float _lastSetTime;
        
        /// <summary>
        /// TODO 採掘中　みたいなステートをちゃんと管理して、同時に採掘しないようにする
        /// </summary>
        /// <param name="progress"></param>
        public void SetProgress(float progress)
        {
            _lastSetTime = Time.time;
            slider.value = progress;
        }

        private void Update()
        {
            canvasGroup.alpha = _lastSetTime + FadeTime < Time.time ? 0 : 1;
        }
    }
}