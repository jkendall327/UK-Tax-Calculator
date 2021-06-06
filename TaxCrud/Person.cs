namespace TaxCrud
{
    public record Person
    {
        public int Id { get; init; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Name => FirstName + " " + LastName;

        public override string ToString() => $"[{Id}] {FirstName} {LastName}";
    }

    public record InvalidPerson : Person { }
}
