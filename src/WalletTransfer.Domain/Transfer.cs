namespace WalletTransfer.Domain;

public enum TransferStatus { Pending, Validating, WalletValidating, RuleChecking, BalanceChecking, Blocking, Blocked, Transferring, Transferred, Finalizing, Completed, Failed, RetryPending, NeedManualReview }
public enum TransferStep { None, RequestValidation, WalletValidation, RuleCheck, BalanceCheck, Block, Transfer, Finalize, Completed }

public sealed class Transfer
{
    public Guid Id { get; private set; }
    public string SourceWalletId { get; private set; } = default!;
    public string DestinationWalletId { get; private set; } = default!;
    public string SourceProvider { get; private set; } = default!;
    public string DestinationProvider { get; private set; } = default!;
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = default!;
    public TransferStatus Status { get; private set; }
    public TransferStep CurrentStep { get; private set; }
    public int RetryCount { get; private set; }
    public DateTimeOffset? RetryAt { get; private set; }
    public string? LastErrorCode { get; private set; }
    public string? BlockReferenceId { get; private set; }
    public string? TransferReferenceId { get; private set; }
    public string ClientIdempotencyKey { get; private set; } = default!;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static Transfer Create(Guid id, string sourceWalletId, string destinationWalletId, string sourceProvider, string destinationProvider, decimal amount, string currency, string clientIdempotencyKey, DateTimeOffset now)
        => new() { Id = id, SourceWalletId = sourceWalletId, DestinationWalletId = destinationWalletId, SourceProvider = sourceProvider, DestinationProvider = destinationProvider, Amount = amount, Currency = currency, ClientIdempotencyKey = clientIdempotencyKey, Status = TransferStatus.Pending, CurrentStep = TransferStep.None, CreatedAt = now, UpdatedAt = now };

    public bool IsStepCompleted(TransferStep step) => (int)CurrentStep >= (int)step;
    public void MarkRequestValidated() => Set(TransferStatus.Validating, TransferStep.RequestValidation);
    public void MarkWalletsValidated() => Set(TransferStatus.WalletValidating, TransferStep.WalletValidation);
    public void MarkRulesChecked() => Set(TransferStatus.RuleChecking, TransferStep.RuleCheck);
    public void MarkBalanceChecked() => Set(TransferStatus.BalanceChecking, TransferStep.BalanceCheck);
    public void MarkAmountBlocked(string r) { BlockReferenceId = r; Set(TransferStatus.Blocked, TransferStep.Block); }
    public void MarkMoneyTransferred(string r) { TransferReferenceId = r; Set(TransferStatus.Transferred, TransferStep.Transfer); }
    public void MarkFinalized() => Set(TransferStatus.Finalizing, TransferStep.Finalize);
    public void MarkCompleted() => Set(TransferStatus.Completed, TransferStep.Completed);
    public void MarkRetryableFailure(TransferStep step, string errorCode, DateTimeOffset retryAt) { Status = TransferStatus.RetryPending; CurrentStep = step; LastErrorCode = errorCode; RetryAt = retryAt; RetryCount++; UpdatedAt = DateTimeOffset.UtcNow; }
    public void MarkPermanentFailure(TransferStep step, string errorCode) { Status = TransferStatus.Failed; CurrentStep = step; LastErrorCode = errorCode; UpdatedAt = DateTimeOffset.UtcNow; }
    public void MarkNeedManualReview(string errorCode) { Status = TransferStatus.NeedManualReview; LastErrorCode = errorCode; UpdatedAt = DateTimeOffset.UtcNow; }
    void Set(TransferStatus status, TransferStep step){ Status=status; CurrentStep=step; RetryAt=null; LastErrorCode=null; UpdatedAt=DateTimeOffset.UtcNow; }
}
