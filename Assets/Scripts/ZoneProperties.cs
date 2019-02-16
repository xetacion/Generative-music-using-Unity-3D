using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneProperties : MonoBehaviour {

    [Range(1, 10)]
    public int zoneMusicSpeed = 5;

    public enum ZoneScale { Cmajor, Gmajor, Dmajor, Amajor, Emajor, Bmajor, Fmajor, Cminor, Gminor, Dminor, Aminor, Eminor, Bminor, Fminor };
    public ZoneScale zoneScale;


    public string getMusicScale()
    {
        string chosenScale = Enum.GetName(typeof(ZoneScale), (int)zoneScale);
        return chosenScale;
    }

    public int getZoneMusicSpeed()
    {
        return zoneMusicSpeed;
    }
	
}
