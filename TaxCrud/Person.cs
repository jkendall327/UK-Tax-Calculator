namespace TaxCrud
{
    internal record Person
    {
        internal int Id { get; init; }

        internal string FirstName { get; set; }
        internal string LastName { get; set; }
        internal string Name => FirstName + " " + LastName;

        public override string ToString() => $"[{Id}] {FirstName} {LastName}";
    }

    internal record InvalidPerson : Person { }
}
