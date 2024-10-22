﻿namespace WorkLifeBalance.Models;

[Serializable]
public class ProcessActivityData
{
    public string Date { get; set; } = "06062023";
    public string Process { get; set; } = "da";
    public string TimeSpent { get; set; } = "000000";

    public DateOnly DateC { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    public TimeOnly TimeSpentC { get; set; } = new TimeOnly(0, 0, 0);

    public void ConvertSaveDataToUsableData()
    {
        try
        {
            if (string.IsNullOrEmpty(Date))
            {
                return;
            }

            DateC = new DateOnly
                (
                    int.Parse(Date.Substring(4, 4)),
                    int.Parse(Date[..2]),
                    int.Parse(Date.Substring(2, 2))
                );

            TimeSpentC = new TimeOnly
                (
                    int.Parse(TimeSpent[..2]),
                    int.Parse(TimeSpent.Substring(2, 2)),
                    int.Parse(TimeSpent.Substring(4, 2))
                );
        }
        catch { }
    }
    public void ConvertUsableDataToSaveData()
    {
        try
        {
            Date = DateC.ToString("MMddyyyy");

            TimeSpent = TimeSpentC.ToString("HHmmss");
        }
        catch { }
    }
}
