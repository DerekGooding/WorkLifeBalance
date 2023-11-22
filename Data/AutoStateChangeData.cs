﻿using System;
using System.Collections.Generic;
using WorkLifeBalance.Handlers.Feature;

namespace WorkLifeBalance.Data
{
    [Serializable]
    public class AutoStateChangeData
    {
        public ProcessActivity[] Activities { get; set; } = new ProcessActivity[0];
        public string[] WorkingStateWindows { get; set; } = new string[0];

        public Dictionary<string,TimeOnly> ActivitiesC = new();

        public void ConvertSaveDataToUsableData()
        {
            foreach(ProcessActivity activity in Activities)
            {
                activity.ConvertSaveDataToUsableData();
                ActivitiesC.Add(activity.Process,activity.TimeSpentC);
            }

        }
        public void ConvertUsableDataToSaveData()
        {
            List<ProcessActivity> processActivities = new();


            foreach (KeyValuePair<string,TimeOnly> activity in ActivitiesC)
            {
                ProcessActivity process = new();
                process.DateC = DataHandler.Instance.TodayData.DateC;
                process.Process = activity.Key;
                process.TimeSpentC = activity.Value;

                process.ConvertUsableDataToSaveData();

                processActivities.Add(process);
            }

            Activities = processActivities.ToArray();
        }
    }
}
