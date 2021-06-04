using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.Linq;

namespace TaxCrud
{
    // store Person record
    // store Entry record for each financial transaction
    // determine total tax to pay

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var bs = new DatabaseBootstrap(new DatabaseConfig() { Name = "test" });
            bs.Setup();
            bs.Read();
        }
    }

    public interface IDatabaseBootstrap
    {
        void Setup();
    }

    public class DatabaseConfig
    {
        public string Name { get; set; }
    }

    public class DatabaseBootstrap : IDatabaseBootstrap
    {
        private readonly DatabaseConfig databaseConfig;

        public DatabaseBootstrap(DatabaseConfig databaseConfig)
        {
            this.databaseConfig = databaseConfig;
        }

        readonly SqliteConnection x;

        public void Setup()
        {
            using var x = new SqliteConnection(databaseConfig.Name);

            var table = x.Query<string>("SELECT name FROM sqlite_master WHERE type='table' AND name = 'Product';");
            var tableName = table.FirstOrDefault();
            if (!string.IsNullOrEmpty(tableName) && tableName == "Product")
                return;

            x.Execute("Create Table Product (" +
                "Name VARCHAR(100) NOT NULL," +
                "Description VARCHAR(1000) NULL);");
        }

        internal void Read()
        {
            var table = x.Query<string>("SELECT name FROM sqlite_master WHERE type='table' AND name = 'Product';");
            var tableName = table.FirstOrDefault();
            Console.WriteLine(tableName);
        }
    }
}
