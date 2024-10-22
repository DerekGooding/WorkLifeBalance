﻿namespace WorkLifeBalance.Models;

[Serializable]
public class AutoStateChangeData
{
    public ProcessActivityData[] Activities { get; set; } = Array.Empty<ProcessActivityData>();
    public string[] WorkingStateWindows { get; set; } = Array.Empty<string>();

    public Dictionary<string, TimeOnly> ActivitiesC = new();

    public void ConvertSaveDataToUsableData()
    {
        try
        {
            foreach (ProcessActivityData activity in Activities)
            {
                activity.ConvertSaveDataToUsableData();
                ActivitiesC.Add(activity.Process, activity.TimeSpentC);
            }
        }
        catch (Exception ex)
        {
            //MainWindow.ShowErrorBox("StateChangeData Error", "Failed to convert data to usable data", ex);
        }
    }
    public void ConvertUsableDataToSaveData()
    {
        try
        {
            List<ProcessActivityData> processActivities = new();


            foreach (KeyValuePair<string, TimeOnly> activity in ActivitiesC)
            {
                ProcessActivityData process = new()
                {
                    //process.DateC = DataStorageFeature.Instance.TodayData.DateC;
                    Process = activity.Key,
                    TimeSpentC = activity.Value
                };

                process.ConvertUsableDataToSaveData();

                processActivities.Add(process);
            }

            Activities = processActivities.ToArray();
        }
        catch (Exception ex)
        {
            //MainWindow.ShowErrorBox("StateChangeData Error", "Failed to convert usable data to save data", ex);
        }
    }
}
