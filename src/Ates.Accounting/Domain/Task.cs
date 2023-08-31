using Ates.Accounting.SeedWork;

namespace Ates.Accounting.Domain;

public class Task : Entity
{
    public Task(Guid publicId, string title, int fee, int reward)
    {
        PublicId = publicId;
        Title = title;
        Fee = fee;
        Reward = reward;
    }

    public Guid PublicId { get; set; }
    public string Title { get; set; }
    public int Fee { get; set; }
    public int Reward { get; set; }
}