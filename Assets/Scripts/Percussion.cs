using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Assets.Scripts
{
    public class Percussion : MonoBehaviour
    {
        private AudioSource audioSource_A;
        private AudioClip bassDrum;
        private Boolean breakPlaying;

        public void initiatePercussion(AudioSource A)
        {
            audioSource_A = A;

            readAudioClips();
        }

        public IEnumerator playPercussion()
        {
            breakPlaying = false;
            if(MusicGenerator.tempoMultiplier <= 3)
            {
                int bassHits = 0;
                if (MusicGenerator.tempoMultiplier == 3)
                {
                    bassHits = 1;
                } else if(MusicGenerator.tempoMultiplier == 2)
                {
                    bassHits = 2;
                } else if(MusicGenerator.tempoMultiplier == 1) {
                    bassHits = 4;
                }
                float timeBetweenHits = MusicGenerator.tempo / bassHits;
                bassHits *= MusicGenerator.bassNoteDur;
                

                for(int i = 0; i < bassHits; i++)
                {
                    if(breakPlaying)
                    {
                        yield break;
                    }
                    audioSource_A.clip = bassDrum;
                    audioSource_A.Play();
                    yield return new WaitForSeconds(timeBetweenHits);
                }

            } else
            {
                yield break;
            }
        }

        public void EndPrematurely()
        {
            breakPlaying = true;
        }

        private void readAudioClips()
        {
            System.Object[] inst;
            inst = Resources.LoadAll("percussions", typeof(AudioClip));

            for (int i = 0; i < inst.Length; i++)
            {
                AudioClip audioC = inst[i] as AudioClip;
                if (audioC.name == "bass-drum")
                {
                    bassDrum = audioC;
                }
            }
        }
    }
}
