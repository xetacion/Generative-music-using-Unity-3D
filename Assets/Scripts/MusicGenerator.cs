using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class MusicGenerator : MonoBehaviour
{
    public static List<string> scale;
    public static List<string> bassProgression;
    public static List<Melody> melodyList = new List<Melody>();
    public static List<Coroutine> coroutinesList = new List<Coroutine>();
    public static List<string> standardScaleList;
    public static List<Melody> usableInstruments;
    public static bool somethingPlaying;
    public static bool breakedPrematurely;
    public static int bassNoteDur = 2;
    public static float transitionDivider = 3.0f;
    public static float tempo = 1.0f;
    public static int tempoMultiplier;
    public static int standardSpeed;
    public static Percussion percussion;
    public static System.Random rnd = new System.Random();
    public static Boolean usingDynamic;
    //private int progressionCounter = 0;
    public enum StandardScale { Cmajor, Gmajor, Dmajor, Amajor, Emajor, Bmajor, Fmajor, Cminor, Gminor, Dminor, Aminor, Eminor, Bminor, Fminor };
    public StandardScale standardScale;
    [Range(1, 10)]
    public int standardMusicSpeed = 6;
    [Range(1, 5)]
    public int bassNoteDuration = 2;
    public Boolean useDynamicInstruments = true;
    public Boolean usePercussions = true;
    public Boolean usePiano = true;
    [Range(1, 3)]
    public int pianoAmount = 1;
    public Boolean useDoubleBass = true;
    [Range(1, 3)]
    public int doubleBassAmount = 3;
    public Boolean useViolin = true;
    [Range(1, 3)]
    public int violinAmount = 3;
    public Boolean useGuitar = true;
    [Range(1, 3)]
    public int guitarAmount = 2;
    public Boolean useFlute = true;
    [Range(1, 3)]
    public int fluteAmount = 2;


    void Start()
    {
        this.gameObject.AddComponent<ZoneChange>();
        bassNoteDur = bassNoteDuration;
        tempoMultiplier = standardMusicSpeed;
        standardSpeed = standardMusicSpeed;
        string chosenScale = Enum.GetName(typeof(StandardScale), (int)standardScale);
        scale = new List<string>(Scales.scales[chosenScale]);
        standardScaleList = scale;
        bassProgression = generateBassProgression();
        usingDynamic = useDynamicInstruments;


        if(usePercussions)
        {
            AudioSource a = this.gameObject.AddComponent<AudioSource>();
            percussion = this.gameObject.AddComponent<Percussion>();
            percussion.initiatePercussion(a);
        }
        if (useDoubleBass)
        {
            createInstrumentXTimes(doubleBassAmount, "double-bass");
        }
        if (useViolin)
        {
            createInstrumentXTimes(violinAmount, "violin");
        }
        if (useGuitar)
        {
            createInstrumentXTimes(guitarAmount, "guitar");
        }
        if (usePiano)
        {
            createInstrumentXTimes(pianoAmount, "piano");
        }
        if (useFlute)
        {
            createInstrumentXTimes(fluteAmount, "flute");
        }

        usableInstruments = combineUsableInstruments();

        StartCoroutine(startMusic());

    }


    private IEnumerator startMusic()
    {
        int randomBassInt = rnd.Next(bassProgression.Count);
        string bassNoteName = bassProgression[randomBassInt];
        Dictionary<string, List<float>> noteLengths = findNoteLengths(bassNoteName);
        while (true)
        {
            breakedPrematurely = false;
            foreach(Melody m in usableInstruments)
            {
                StartCoroutine(m.playMelody(noteLengths[m.getInstrument()]));
            }
            if(usePercussions)
            {
                StartCoroutine(percussion.playPercussion());
            }
            if (bassNoteName == scale[scale.Count - 1])
            {
                bassNoteName = bassProgression[0];
            }
            else
            {
                randomBassInt = rnd.Next(bassProgression.Count);
                bassNoteName = bassProgression[randomBassInt];
            }
            noteLengths.Clear();
            noteLengths = findNoteLengths(bassNoteName);
            somethingPlaying = true;
            while(somethingPlaying)
            {
                foreach (Melody m in usableInstruments)
                {
                    if(m.playing)
                    {
                        somethingPlaying = true;
                        break;
                    } else
                    {
                        somethingPlaying = false;
                    }
                }
                yield return new WaitForSeconds(0.005f);
            }
            if(breakedPrematurely)
            {
                randomBassInt = rnd.Next(bassProgression.Count);
                bassNoteName = bassProgression[randomBassInt];
                noteLengths.Clear();
                noteLengths = findNoteLengths(bassNoteName);
            }
        }
    }

    private Dictionary<string, List<float>> findNoteLengths(string bassNoteName)
    {
        Dictionary<string, List<float>> genDict = new Dictionary<string, List<float>>();
        foreach (Melody m in usableInstruments)
        {
            if (!genDict.ContainsKey(m.getInstrument()))
            {
                List<float> genList = m.generateAllLists(bassNoteName);
                genDict.Add(m.getInstrument(), genList);
            }
            else
            {
                m.generateAllLists(bassNoteName);
            }
        }
        return genDict;
    }

    public static List<string> generateBassProgression()
    {
        List<string> generatedProgression = new List<string>();
        int[] progression = { 0, 3, 4 };
        foreach(int i in progression)
        {
            for(int j = 0; j < 3; j++)
            {
                generatedProgression.Add(scale[i]);
            }
        }
        generatedProgression.Add(scale[6]);

        return generatedProgression;
    }

    public static List<Melody> combineUsableInstruments()
    {
        List<Melody> combinedList = new List<Melody>();
        Dictionary<string, int> instrumentCount = new Dictionary<string, int>();
        foreach (Melody m in melodyList)
        {
            if (instrumentCount.ContainsKey(m.getInstrument()))
            {
                instrumentCount[m.getInstrument()]++;
            }
            else
            {
                instrumentCount.Add(m.getInstrument(), 1);
            }
        }
        if (usingDynamic)
        {
            if (tempoMultiplier >= 1 && tempoMultiplier <= 3)
            {
                combinedList = melodyList;
            } else if(tempoMultiplier >= 4 && tempoMultiplier <= 7)
            {
                foreach (string name in instrumentCount.Keys)
                {
                    if(instrumentCount[name] >= 2)
                    {
                        int counter = 0;
                        foreach(Melody m in melodyList)
                        {
                            if(m.getInstrument() == name)
                            {
                                combinedList.Add(m);
                                counter++;
                                if(counter == instrumentCount[name] - 1)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (string name in instrumentCount.Keys)
                {
                    if (instrumentCount[name] >= 3)
                    {
                        foreach (Melody m in melodyList)
                        {
                            if (m.getInstrument() == name)
                            {
                                combinedList.Add(m);
                                break;
                            }
                        }
                    }
                }
            }
        } else
        {
            combinedList = melodyList;
        }
        return combinedList;
    }

    private void createInstrumentXTimes(int amount, string instrument)
    {
        for (int i = 0; i < amount; i++)
        {
            AudioSource a = this.gameObject.AddComponent<AudioSource>();
            AudioSource b = this.gameObject.AddComponent<AudioSource>();
            AudioSource c = this.gameObject.AddComponent<AudioSource>();
            Melody melody = this.gameObject.AddComponent<Melody>();
            melody.initiateMelody(instrument, a, b, c);
            melodyList.Add(melody);
        }
    }
}

