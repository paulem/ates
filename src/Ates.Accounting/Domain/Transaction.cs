using Ates.Accounting.SeedWork;

namespace Ates.Accounting.Domain;

public class Transaction : Entity
{
    // Debit - deposit money
    // Credit - charge off money
    public Transaction(Guid publicId, int debit, int credit, string message)
    {
        PublicId = publicId;
        Debit = debit;
        Credit = credit;
        Message = message;

    }

    public Guid PublicId { get; }
    public int Debit { get; }
    public int Credit { get; }
    public string Message { get; }

}