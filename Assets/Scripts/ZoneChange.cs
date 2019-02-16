using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;
using System;

public class ZoneChange : MonoBehaviour {

    private List<Collider> collisions = new List<Collider>();

    void OnTriggerEnter(Collider col)
    {
        ZoneProperties zone = col.gameObject.GetComponent<ZoneProperties>();
        if (zone != null)
        {
            MusicGenerator.scale = new List<string>(Scales.scales[zone.getMusicScale()]);
            MusicGenerator.bassProgression = MusicGenerator.generateBassProgression();
            collisions.Add(col);
            zoneChangeActions();
        }
    }

    private void OnTriggerExit(Collider col)
    {
        ZoneProperties zone = col.gameObject.GetComponent<ZoneProperties>();
        if (zone != null)
        {
            collisions.Remove(col);
            if(collisions.Count == 0)
            {
                MusicGenerator.scale = MusicGenerator.standardScaleList;
                MusicGenerator.bassProgression = MusicGenerator.generateBassProgression();
            } else if(collisions.Count == 1)
            {
                MusicGenerator.scale = new List<string>(Scales.scales[collisions[0].gameObject.GetComponent<ZoneProperties>().getMusicScale()]);
                MusicGenerator.bassProgression = MusicGenerator.generateBassProgression();
            }
            zoneChangeActions();
        }
    }

    private void zoneChangeActions()
    {
        MusicGenerator.tempoMultiplier = findMusicSpeed();

        foreach (Melody m in MusicGenerator.usableInstruments)
        {
            m.EndPrematurely();
        }
        if(MusicGenerator.percussion != null)
        {
            MusicGenerator.percussion.EndPrematurely();
        }
        MusicGenerator.usableInstruments = MusicGenerator.combineUsableInstruments();
        MusicGenerator.breakedPrematurely = true;
    }

    private int findMusicSpeed()
    {
        int calculatedSpeed;

        if(collisions.Count == 0)
        {
            calculatedSpeed = MusicGenerator.standardSpeed;
        } else if(collisions.Count == 1)
        {
            calculatedSpeed = collisions[0].gameObject.GetComponent<ZoneProperties>().getZoneMusicSpeed();
        } else
        {
            int sumOfColliders = 0;
            foreach(Collider c in collisions)
            {
                sumOfColliders += c.gameObject.GetComponent<ZoneProperties>().getZoneMusicSpeed();
            }
            calculatedSpeed = sumOfColliders / collisions.Count;
        }

        return calculatedSpeed;
    }
}
