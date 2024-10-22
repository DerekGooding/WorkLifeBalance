namespace WorkLifeBalance.Models;

[Serializable]
public class AutoStateChangeData
{
    public ProcessActivityData[] Activities { get; set; } = [];
    public string[] WorkingStateWindows { get; set; } = [];

    public Dictionary<string, TimeOnly> ActivitiesC = [];

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
        catch { }
    }
    public void ConvertUsableDataToSaveData()
    {
        try
        {
            List<ProcessActivityData> processActivities = [];

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

            Activities = [.. processActivities];
        }
        catch { }
    }
}
