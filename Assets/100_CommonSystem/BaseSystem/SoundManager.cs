using UnityEngine;

namespace BaseSystem
{
    public class SoundManager : SingletonBase<SoundManager>
    {
        AudioListener m_audioListener = null;
        AudioSource m_bgmAudioSource = null;
        AudioSource m_seAudioSource = null;
        AudioSource m_LoopSeAudioSource = null;

        protected override void Init()
        {
            Base.Instance.AddToBase(this);

            m_audioListener = gameObject.AddComponent<AudioListener>();
            m_bgmAudioSource = gameObject.AddComponent<AudioSource>();
            m_seAudioSource = gameObject.AddComponent<AudioSource>();
            m_LoopSeAudioSource = gameObject.AddComponent<AudioSource>();

            m_bgmAudioSource.volume = 0.5f;
        }

        public void PlayBGM(AudioClip audioClip, float fadeInSeconds = 0, bool onLoop = true)
        {
            m_bgmAudioSource.clip = audioClip;
            m_bgmAudioSource.loop = onLoop;
            m_bgmAudioSource.Play();
        }

        public void StopBGM(float fadeOutSeconds = 0)
        {
            m_bgmAudioSource.Stop();
            m_bgmAudioSource.clip = null;
        }

        public void PlayOneShotSE(AudioClip audioClip)
        {
            m_seAudioSource.PlayOneShot(audioClip);
        }

        public void PlaySE(AudioClip audioClip)
        {
            m_seAudioSource.clip = audioClip;
            m_seAudioSource.Play();
        }

        public void StopSE()
        {
            m_seAudioSource.Stop();
            m_seAudioSource.clip = null;
        }

        public void PlayLoopSE(AudioClip audioClip, float pitch = 1.0f)
        {
            m_LoopSeAudioSource.clip = audioClip;
            m_LoopSeAudioSource.loop = true;
            m_LoopSeAudioSource.pitch = pitch;
            m_LoopSeAudioSource.Play();
        }

        public void StopLoopSE()
        {
            m_LoopSeAudioSource.Stop();
            m_LoopSeAudioSource.clip = null;
        }
    }
}