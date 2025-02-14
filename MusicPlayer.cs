using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RGSK
{
    public class MusicPlayer : MonoBehaviour
    {
        private AudioSource musicAudioSource;
        public AudioClip[] musicTracks;
        public bool randomize;
        public bool autoStartMusic;
        private int index = 0;
        private int lastIndex;

        void Start()
        {
            musicAudioSource = Helper.CreateAudioSource(gameObject, null, "Music", 0, 1, musicTracks.Length == 1, false);

            if (musicTracks.Length <= 1)
                randomize = false;

            if (autoStartMusic)
                StartMusic();
        }


        public void StartMusic()
        {
            if (musicAudioSource == null || musicAudioSource.isPlaying || musicTracks.Length == 0)
                return;

            index = !randomize ? 0 : Random.Range(0, musicTracks.Length);
            PlayTrack(index);
        }


        public void PlayNext()
        {
            index++;

            index = index % musicTracks.Length;
            PlayTrack(index);
        }


        public void PlayRandom()
        {
            int temp = 0;

            Init:
            while (true)
            {
                temp = Random.Range(0, musicTracks.Length);

                if (temp == lastIndex)
                {
                    goto Init;
                }

                goto Done;
            }
            Done:
            PlayTrack(temp);
        }


        public void OverrideMusicClip(AudioClip clip, bool loop)
        {
            if (musicAudioSource == null)
                return;

            musicAudioSource.clip = clip;
            musicAudioSource.loop = loop;
            musicAudioSource.Play();
        }


        void PlayTrack(int i)
        {
            musicAudioSource.clip = musicTracks[i];
            musicAudioSource.Play();
            lastIndex = i;

            StartCoroutine(WaitForMusic(musicAudioSource.clip.length));
        }


        IEnumerator WaitForMusic(float duration)
        {
            float time = 0;

            while (time < duration + 0.5f)
            {
                time += Time.deltaTime;
                yield return null;
            }

            TrackFinished();
        }


        void TrackFinished()
        {
            if (musicAudioSource.isPlaying)
                return;

            if (!randomize)
            {
                PlayNext();
            }
            else
            {
                PlayRandom();
            }
        }


        public void Pause()
        {
            if (musicAudioSource != null)
            {
                musicAudioSource.Pause();
            }
        }


        public void UnPause()
        {
            if (musicAudioSource != null)
            {
                musicAudioSource.UnPause();
            }
        }
    }
}