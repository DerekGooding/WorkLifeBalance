namespace WorkLifeBalance.Models;

[Serializable]
public class DayData
{
    public string Date { get; set; } = "";
    public string WorkedAmmount { get; set; } = "";
    public string RestedAmmount { get; set; } = "";
    public string IdleAmmount { get; set; } = "";

    public DateOnly DateC { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    public TimeOnly WorkedAmmountC { get; set; } = new TimeOnly(0, 0, 0);
    public TimeOnly RestedAmmountC { get; set; } = new TimeOnly(0, 0, 0);
    public TimeOnly IdleAmmountC { get; set; } = new TimeOnly(0, 0, 0);

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

            WorkedAmmountC = new TimeOnly
                (
                    int.Parse(WorkedAmmount[..2]),
                    int.Parse(WorkedAmmount.Substring(2, 2)),
                    int.Parse(WorkedAmmount.Substring(4, 2))
                );

            RestedAmmountC = new TimeOnly
                (
                    int.Parse(RestedAmmount[..2]),
                    int.Parse(RestedAmmount.Substring(2, 2)),
                    int.Parse(RestedAmmount.Substring(4, 2))
                );

            IdleAmmountC = new TimeOnly
                (
                    int.Parse(IdleAmmount[..2]),
                    int.Parse(IdleAmmount.Substring(2, 2)),
                    int.Parse(IdleAmmount.Substring(4, 2))
                );
        }
        catch { }
    }
    public void ConvertUsableDataToSaveData()
    {
        try
        {
            Date = DateC.ToString("MMddyyyy");

            WorkedAmmount = WorkedAmmountC.ToString("HHmmss");

            RestedAmmount = RestedAmmountC.ToString("HHmmss");

            IdleAmmount = IdleAmmountC.ToString("HHmmss");
        }
        catch { }
    }
}
