using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class Scales
    {
        public static Dictionary<string, string[]> scales = new Dictionary<string, string[]>
        {
            { "Cmajor", new string[] { "C", "D", "E", "F", "G", "A", "B" } },
            { "Gmajor", new string[] { "G", "A", "B", "C", "D", "E", "Fs" } },
            { "Dmajor", new string[] { "D", "E", "Fs", "G", "A", "B", "Cs" } },
            { "Amajor", new string[] { "A", "B", "Cs", "D", "E", "Fs", "Gs" } },
            { "Emajor", new string[] { "E", "Fs", "Gs", "A", "B", "Cs", "Ds" } },
            { "Bmajor", new string[] { "B", "Cs", "Ds", "E", "Fs", "Gs", "As" } },
            { "Fmajor", new string[] { "F", "G", "A", "As", "C", "D", "E" } },
            { "Cminor", new string[] { "C", "D", "Ds", "F", "G", "Gs", "As" } },
            { "Gminor", new string[] { "G", "A", "As", "C", "D", "Ds", "F" } },
            { "Dminor", new string[] { "D", "E", "F", "G", "A", "As", "C" } },
            { "Aminor", new string[] { "A", "B", "C", "D", "E", "F", "G" } },
            { "Eminor", new string[] { "E", "Fs", "G", "A", "B", "C", "D" } },
            { "Bminor", new string[] { "B", "Cs", "D", "E", "Fs", "G", "A" } },
            { "Fminor", new string[] { "F", "G", "Gs", "As", "C", "Cs", "Ds" } }
        };

        
        
    }
}
