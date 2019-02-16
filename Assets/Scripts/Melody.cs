using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Assets.Scripts
{
    public class Melody : MonoBehaviour
    {
        private AudioSource audioSource_A;
        private AudioSource audioSource_B;
        private AudioSource audioSource_C;
        private AudioClip randomNote = null;
        private int lastNoteOctave = 0;
        private List<AudioClip> allAudioClips;
        private string instrument;
        private string chordOctave = null;
        public bool playing;
        private bool breakPlaying;
        private float pause = 45.0f;

        private List<float> notesOuter = new List<float>();
        private List<string> triadOuter = new List<string>();
        private List<AudioClip> allPossibleNotesOuter = new List<AudioClip>();

        public void initiateMelody(string instrument, AudioSource A, AudioSource B, AudioSource C)
        {
            audioSource_A = A;
            audioSource_B = B;
            audioSource_C = C;
            audioSource_B.volume = 0.0f;
            this.instrument = instrument;

            readAudioClips(instrument);
        }

        public List<float> generateAllLists(string bassNote)
        {
            notesOuter = generateNoteList();
            triadOuter = generateTriad(bassNote);
            allPossibleNotesOuter = generateAllPossibleNotes(triadOuter);
            List <float> generatedNoteLengthsOuter = generateNoteLengths(notesOuter);
            return generatedNoteLengthsOuter;
        }


        public IEnumerator playMelody(List<float> noteLengths)
        {
            playing = true;
            breakPlaying = false;
            List<AudioClip> allPossibleNotes = allPossibleNotesOuter;
            List<float> generatedNoteLengths = noteLengths;
            if (instrument == "piano" || instrument == "guitar")
            {
                playChords();
                yield return new WaitForSeconds(MusicGenerator.bassNoteDur);
            }
            else
            {
                for (int i = 0; i < generatedNoteLengths.Count; i++)
                {
                    findNextRandomNote(allPossibleNotes);
                    float noteLength = generatedNoteLengths[i];
                    float multiplier = Mathf.Floor(noteLength / MusicGenerator.tempo);
                    float remainder = noteLength % MusicGenerator.tempo;
                    if (noteLength == pause)
                    {
                        yield return new WaitForSeconds(MusicGenerator.tempo / 2);
                    }
                    else if (noteLength > MusicGenerator.tempo && remainder == 0)
                    {
                        for (int count = 0; count < multiplier; count++)
                        {
                            if (breakPlaying)
                            {
                                playing = false;
                                yield break;
                            }
                            StartCoroutine(switchTrack(audioSource_A, audioSource_B, randomNote, MusicGenerator.tempo / MusicGenerator.transitionDivider));
                            yield return new WaitForSeconds(MusicGenerator.tempo);
                        }
                    }
                    else
                    {
                        StartCoroutine(switchTrack(audioSource_A, audioSource_B, randomNote, noteLength / MusicGenerator.transitionDivider));
                        yield return new WaitForSeconds(noteLength);
                    }
                    if (breakPlaying)
                    {
                        playing = false;
                        yield break;
                    }
                }
            }
            playing = false;
        }

        private void playChords()
        {
            List<AudioClip> chordNotes = new List<AudioClip>();
            int lastOctave;
            if (chordOctave != null)
            {
                lastOctave = Int32.Parse(chordOctave);
            }
            else
            {
                string firstNote = allPossibleNotesOuter[0].name;
                string lastNote = allPossibleNotesOuter[allPossibleNotesOuter.Count - 1].name;
                int firstNoteOctave = (int)System.Char.GetNumericValue(firstNote[firstNote.Length - 1]);
                int lastNoteOctave = (int)System.Char.GetNumericValue(lastNote[firstNote.Length - 1]);
                lastOctave = MusicGenerator.rnd.Next(firstNoteOctave, lastNoteOctave + 1);
            }
            
            int octaveUp = lastOctave + 1;
            int octaveDown = lastOctave - 1;
            List<int> octaveChooser = new List<int>(new int[] { octaveDown, lastOctave, octaveUp });
            while (true)
            {
                int randomOctave = MusicGenerator.rnd.Next(octaveChooser.Count);
                string nextOctave = octaveChooser[randomOctave].ToString();
                Regex regex = new Regex(instrument + @"_[" + triadOuter[0] + triadOuter[1] + triadOuter[2] + @"]" + nextOctave);
                for (int i = 0; i < allAudioClips.Count; i++)
                {
                    Match match = regex.Match(allAudioClips[i].name);
                    if (match.Success)
                    {
                        chordNotes.Add(allAudioClips[i]);
                    }
                }
                if(chordNotes.Count >= 2)
                {
                    break;
                } else
                {
                    chordNotes.Clear();
                }
            }
            /*audioSource_A.clip = chordNotes[0];
            audioSource_B.clip = chordNotes[1];
            audioSource_A.Play();
            audioSource_B.Play();*/
            //audioSource_C.clip = chordNotes[2];

            audioSource_A.PlayOneShot(chordNotes[0]);
            audioSource_B.PlayOneShot(chordNotes[1]);
            if (chordNotes.Count > 2)
            {
                audioSource_C.clip = chordNotes[2];
                audioSource_C.Play();
                audioSource_C.PlayOneShot(chordNotes[2]);
            }
          
        }

        private List<float> generateNoteList()
        {
            List<float> generatedList = new List<float>();
            float quarterNote = MusicGenerator.tempo / 4;
            float halfNote = MusicGenerator.tempo / 2;
            float wholeNote = MusicGenerator.tempo;
            float wholeAndHalfNote = MusicGenerator.tempo * 1.5f;
            float doubleNote = MusicGenerator.tempo * 2;
            if (MusicGenerator.tempoMultiplier >= 1 && MusicGenerator.tempoMultiplier <= 3)
            {
                double halfNoteCount = Math.Pow(2, 1);
                double quarterNoteCount = Math.Pow(2, 4 - MusicGenerator.tempoMultiplier);
                generatedList = addXTimes(generatedList, halfNote, halfNoteCount);
                generatedList = addXTimes(generatedList, quarterNote, quarterNoteCount);
            }
            else if (MusicGenerator.tempoMultiplier >= 4 && MusicGenerator.tempoMultiplier <= 7)
            {
                double wholeNoteCount = Math.Pow(2, MusicGenerator.tempoMultiplier - 2);
                double halfNoteCount = Math.Pow(2, 8 - MusicGenerator.tempoMultiplier);
                double wholeAndHalfNoteCount;
                double quarterNoteCount;
                if (MusicGenerator.tempoMultiplier >= 6)
                {
                    wholeAndHalfNoteCount = Math.Pow(2, 2);
                    quarterNoteCount = Math.Pow(2, 1);
                } else
                {
                    quarterNoteCount = Math.Pow(2, 2);
                    wholeAndHalfNoteCount = Math.Pow(2, 1);
                }
                generatedList = addXTimes(generatedList, halfNote, halfNoteCount);
                generatedList = addXTimes(generatedList, quarterNote, quarterNoteCount);
                generatedList = addXTimes(generatedList, wholeNote, wholeNoteCount);
                generatedList = addXTimes(generatedList, doubleNote, wholeNoteCount);
                generatedList = addXTimes(generatedList, wholeAndHalfNote, wholeAndHalfNoteCount);
            }
            else
            {
                double wholeNoteCount = Math.Pow(2, 1); ;
                double wholeAndHalfNoteCount = Math.Pow(2, MusicGenerator.tempoMultiplier - 7);
                generatedList = addXTimes(generatedList, wholeNote, wholeNoteCount);
                generatedList = addXTimes(generatedList, doubleNote, wholeNoteCount);
                generatedList = addXTimes(generatedList, wholeAndHalfNote, wholeAndHalfNoteCount);
            }
            return generatedList;
        }

        private List<float> addXTimes(List<float> list, float element, double amount)
        {
            List<float> addedList = list;
            for(int i = 0; i < amount; i++)
            {
                addedList.Add(element);
            }
            return addedList;
        }

        private List<string> generateTriad(string bassNote)
        {
            int bassPosition = 0;
            List<string> triadNotes = new List<string>();
            for (int i = 0; i < MusicGenerator.scale.Count; i++)
            {
                if (bassNote == MusicGenerator.scale[i])
                {
                    bassPosition = i;
                    triadNotes.Add(MusicGenerator.scale[i]);
                    break;
                }
            }
            for (int i = 1; i < 3; i++)
            {
                bassPosition += 2;
                if (bassPosition > MusicGenerator.scale.Count - 1)
                {
                    bassPosition -= MusicGenerator.scale.Count;
                }
                triadNotes.Add(MusicGenerator.scale[bassPosition]);
            }
            return triadNotes;
        }

        private List<AudioClip> generateAllPossibleNotes(List<string> triad)
        {
            List<AudioClip> notes = new List<AudioClip>();
            Regex regex = new Regex(instrument + @"_[" + triad[0] + triad[1] + triad[2] + @"]\d");
            for (int i = 0; i < allAudioClips.Count; i++)
            {
                Match match = regex.Match(allAudioClips[i].name);
                if (match.Success)
                {
                    notes.Add(allAudioClips[i]);
                }
            }
            return notes;
        }

        private List<float> generateNoteLengths(List<float> notes)
        {
            List<float> genLength = new List<float>();
            float generatedLength = 0.0f;
            while (true)
            {
                List<float> availableLengths = new List<float>();
                for (int i = 0; i < notes.Count; i++)
                {
                    if (notes[i] + generatedLength <= MusicGenerator.bassNoteDur)
                    {
                        availableLengths.Add(notes[i]);
                    }
                }
                if(availableLengths.Count == 0)
                {
                    genLength.Add(pause);
                    break;
                }
                int randomLength = MusicGenerator.rnd.Next(availableLengths.Count);
                float noteLength = availableLengths[randomLength];
                if (noteLength + generatedLength <= MusicGenerator.bassNoteDur)
                {
                    genLength.Add(noteLength);
                    generatedLength += noteLength;
                }
                if (generatedLength == MusicGenerator.bassNoteDur)
                {
                    break;
                }

            }
            return genLength;
        }

        private void findNextRandomNote(List<AudioClip> allPossibleNotes)
        {
            List<AudioClip> usableNotes = new List<AudioClip>();
            int randomNoteInt;
            if (randomNote != null)
            {
                lastNoteOctave = (int)System.Char.GetNumericValue(randomNote.name[randomNote.name.Length - 1]);
                float oneOctaveDown = lastNoteOctave - 1;
                float oneOctaveUp = lastNoteOctave + 1;
                Regex usableRegex = new Regex(instrument + @"_\w[" + oneOctaveDown + lastNoteOctave + oneOctaveUp + @"]");
                for (int count = 0; count < allPossibleNotes.Count; count++)
                {
                    Match usableMatch = usableRegex.Match(allPossibleNotes[count].name);
                    if (usableMatch.Success)
                    {
                        usableNotes.Add(allPossibleNotes[count]);
                    }
                }
                randomNoteInt = MusicGenerator.rnd.Next(usableNotes.Count);
                randomNote = usableNotes[randomNoteInt];
            }
            else
            {
                randomNoteInt = MusicGenerator.rnd.Next(allPossibleNotes.Count);
                randomNote = allPossibleNotes[randomNoteInt];
            }
        }

        public void EndPrematurely()
        {
            breakPlaying = true;
        }

        private IEnumerator CrossFade(AudioSource audioS_a, AudioSource audioS_b, float seconds)
        {
            float step_interval = seconds / 10.0f;
            float vol_interval = 1.0f / 10.0f;

            audioS_b.Play();

            for (int i = 0; i < 10; i++)
            {
                audioS_a.volume -= vol_interval;
                audioS_b.volume += vol_interval;

                yield return new WaitForSeconds(step_interval);
            }

            audioS_a.Stop();
        }

        public IEnumerator switchTrack(AudioSource audioS_a, AudioSource audioS_b, AudioClip note, float transitionDuration)
        {
            bool play_a = true;
            if (audioS_b.volume == 0.0f)
            {
                play_a = false;
            }
            if (play_a)
            {
                audioS_a.clip = note;
                yield return StartCoroutine(CrossFade(audioS_b, audioS_a, transitionDuration));
                
            }
            else
            {
                audioS_b.clip = note;
                yield return StartCoroutine(CrossFade(audioS_a, audioS_b, transitionDuration));
            }
        }

        protected void readAudioClips(string instrument)
        {
            System.Object[] inst;
            inst = Resources.LoadAll(instrument, typeof(AudioClip));
            allAudioClips = new List<AudioClip>();

            for (int i = 0; i < inst.Length; i++)
            {
                AudioClip audioC = inst[i] as AudioClip;
                if (audioC != null)
                {
                    allAudioClips.Add(audioC);
                }
            }
        }

        public string getInstrument()
        {
            return instrument;
        }
    }
}
