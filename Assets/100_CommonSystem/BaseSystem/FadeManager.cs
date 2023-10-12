using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

namespace BaseSystem
{
    /// <summary>
    /// フェードイン管理用マネージャ
    /// </summary>
    public class FadeManager : SingletonBase<FadeManager>
    {
        protected override void Init()
        {
            Base.Instance.AddToBase(this);
        }

        /// <summary>
        /// コントロールのフェード操作
        /// </summary>
        /// <param name="control">フェードするコントロール</param>
        /// <param name="is_fadein">フェードインするか?</param>
        /// <param name="fadeTime">待機時間</param>
        /// <typeparam name="T">コントロール要素</typeparam>
        private IEnumerator Fade<T>(T control, bool is_fadein = false, float fadeTime = 1.0f, UnityAction action = null)
            where T : VisualElement
        {
            float time = 0.0f;
            while (time <= fadeTime)
            {
                time += Time.deltaTime;
                if (is_fadein) control.style.opacity = time / fadeTime;
                else if (!is_fadein) control.style.opacity = 1.0f - (time / fadeTime);
                yield return null;
            }

            action?.Invoke();
        }

        /// <summary>
        /// フェードインラッパ
        /// </summary>
        public IEnumerator ControlFadeIn<T>(T control, float fadeTime = 1.0f, UnityAction action = null)
            where T : VisualElement
        {
            yield return StartCoroutine(Fade(control, true, fadeTime, action));
        }

        /// <summary>
        ///　フェードアウトラッパ 
        /// </summary>
        public IEnumerator ControlFadeOut<T>(T control, float fadeTime = 1.0f, UnityAction action = null)
            where T : VisualElement
        {
            yield return StartCoroutine(Fade(control, false, fadeTime, action));
        }
    }
}