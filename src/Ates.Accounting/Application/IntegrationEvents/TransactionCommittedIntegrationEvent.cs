namespace Ates.Accounting.Application.IntegrationEvents;

internal class TransactionCommittedIntegrationEvent
{
    public TransactionCommittedIntegrationEvent(Guid publicId, int debit, int credit, string message)
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