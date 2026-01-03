namespace NexusProcure.Application.Interfaces;

public interface IApprovalDelegationResolver
{
    Task<Guid> ResolveApproverAsync(Guid originalApproverId);
}
