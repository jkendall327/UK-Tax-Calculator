using System.Collections.Generic;
using System.Data.Common;

namespace TaxCrud
{
    internal interface IDatabaseHelper
    {
        DbConnection Connection { get; set; }
        void AddTransaction(int id, Transaction trans);
        void AddUser(string firstName, string lastName);
        void DeleteUser(int id);
        IEnumerable<Person> GetAllUsers();
        Person GetByID(int result);
        IEnumerable<Person> GetByName(string firstName, string lastName);
        IEnumerable<Transaction> GetTransactions(int id);
        void Initialize();
        void ResetDatabase();
        void UpdateName(int id, string firstName, string lastName);

        /// <summary>
        /// Parses user's input to see if it's a valid <see cref="Person.Id"/>.
        /// </summary>
        /// <returns>A <see cref="Person"/> if ID is valid; otherwise an <see cref="InvalidPerson"/> representing failure.</returns>
        Person GetPerson();
    }
}