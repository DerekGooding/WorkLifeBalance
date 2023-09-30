﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WorkLifeBalance.Data
{
    public class WLBSettings
    {
        public string LastTimeOpened { get; set; } = "";
        public int StartWithWindows { get; set; } = 0;
        public int StartUpCorner { get; set; } = 0;
        public int SaveInterval { get; set; } = 5;

        public DateTime LastTimeOpenedC = new();
        public bool StartWithWindowsC = false;
        public AnchorCorner StartUpCornerC = AnchorCorner.BootomLeft;
        
        public void ConvertSaveDataToUsableData()
        {
            LastTimeOpenedC = new DateTime
                (
                    int.Parse(LastTimeOpened.Substring(8, 4)),
                    int.Parse(LastTimeOpened.Substring(4, 2)),
                    int.Parse(LastTimeOpened.Substring(6, 2)),
                    int.Parse(LastTimeOpened.Substring(0, 2)),
                    int.Parse(LastTimeOpened.Substring(2, 2)),
                    0 
                );
            StartWithWindowsC = StartWithWindows == 1;

            StartUpCornerC = (AnchorCorner)StartUpCorner;

        }

        public void ConvertUsableDataToSaveData()
        {
            LastTimeOpenedC = DateTime.Now;
            LastTimeOpened = LastTimeOpenedC.ToString("HHmmMMddyyyy");

            StartWithWindows = StartWithWindowsC ? 1 : 0;

            StartUpCorner = (int)StartUpCornerC;
        }
    }
    public enum AnchorCorner
    {
        TopLeft = 0,
        TopRight = 1,
        BootomLeft = 2,
        BottomRight = 3
    }
}