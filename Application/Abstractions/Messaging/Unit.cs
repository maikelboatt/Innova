namespace Application.Abstractions.Messaging
{
    public sealed record Unit
    {
        public static readonly Unit Value = new();
    }
}
