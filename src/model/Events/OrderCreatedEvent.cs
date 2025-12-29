namespace model.Events
{
    public record OrderCreatedEvent(
        int OrderId,
        string Product,
        DateTime CreatedAt
    );
}
