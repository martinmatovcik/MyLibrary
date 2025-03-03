namespace MyLibrary.Domain.Abstraction;

public abstract class StatusEntity
{
    public Status Status { get; private set; } = Status.AVAILABLE;

    public bool IsAvailable() => IsStatus(Status.AVAILABLE);

    public void SetAvailable() => SetStatus(Status.AVAILABLE);

    public void SetNotAvailable() => SetStatus(Status.NOT_AVAILABLE);

    public bool IsReserved() => IsStatus(Status.RESERVED);

    public void SetReserved() => SetStatus(Status.RESERVED);

    private bool IsStatus(Status status) => Status == status;

    private void SetStatus(Status newStatus) => Status = newStatus;
}