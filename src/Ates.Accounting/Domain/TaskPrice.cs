namespace Ates.Accounting.Domain;

public class TaskPrice
{
    public int Fee { get; private set; }
    public int Reward { get; private set; }
    
    public static TaskPrice Generate()
    {
        return new TaskPrice
        {
            Fee = new Random().Next(10, 20),
            Reward = new Random().Next(20, 40)
        };
    }
}