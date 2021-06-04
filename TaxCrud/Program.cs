using System;
using System.Collections.Generic;

namespace TaxCrud
{
    // store Person record
    // store Entry record for each financial transaction
    // determine total tax to pay

    class Program
    {
        static void Main(string[] args) => new App().Run();
    }

    class App
    {
        readonly Dictionary<int, Action> Actions = new();

        public void Run()
        {
            Console.WriteLine("Tax Simulator 2021");

            FillDictionary();

            while (true)
            {
                Console.Write("> ");

                var response = Console.ReadLine();

                // parse response
                if (int.TryParse(response, out int result) == false) { Actions[0].Invoke(); continue; }

                // match response with action
                if (Actions.TryGetValue(result, out var action) == false) { Actions[0].Invoke(); continue; }

                action.Invoke();
            }
        }

        private void FillDictionary()
        {
            Actions.Add(0, InvalidAction);
            Actions.Add(1, CreateUser);
            Actions.Add(2, DeleteUser);
        }

        private void InvalidAction() => Console.WriteLine("Invalid response. Please try again.");
        private void CreateUser() => Console.WriteLine("Added user");
        private void DeleteUser() => Console.WriteLine("Deleted user");
    }
}
