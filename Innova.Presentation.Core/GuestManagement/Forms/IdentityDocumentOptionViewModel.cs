namespace Innova.Presentation.Core.GuestManagement.Forms
{
    /// <summary>One entry of the identity-document dropdown: Value is the enum member name, Label is what the user reads.</summary>
    public sealed record IdentityDocumentOptionViewModel
    {
        public IdentityDocumentOptionViewModel( string Value, string Label )
        {
            this.Value = Value;
            this.Label = Label;
        }

        public string Value { get; init; }
        public string Label { get; init; }

        public override string ToString() => Label;

        public void Deconstruct( out string Value, out string Label )
        {
            Value = this.Value;
            Label = this.Label;
        }
    }
}
