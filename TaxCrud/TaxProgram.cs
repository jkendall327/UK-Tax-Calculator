using EasyConsole;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TaxCrudTests")]

namespace TaxCrud
{
    // TODO: password protect user details

    class TaxProgram
    {
        static void Main() => new App().Run();
    }

    internal class App : Program
    {
        public App() : base(title: "Tax Simulator 2021", breadcrumbHeader: true)
        {
            var connection = new SqliteHelper();

            AddPage(new MainPage(this));
            AddPage(new UserPage(this, connection));
            AddPage(new ViewUsersPage(this, connection));
            AddPage(new CreateUserPage(this, connection));
            AddPage(new SearchPage(this, connection));
            AddPage(new ClearDatabasePage(this, connection));

            SetPage<MainPage>();
        }
    }

    internal class MainPage : MenuPage
    {
        public MainPage(Program program) : base
            (
                "Tax Simulator 2021", program,
                new Option("View users", () =>
                    program.NavigateTo<ViewUsersPage>()),
                new Option("View user details", () =>
                    program.NavigateTo<UserPage>()),
                new Option("Create user", () =>
                    program.NavigateTo<CreateUserPage>()),
                new Option("Search users", () =>
                    program.NavigateTo<SearchPage>()),
                new Option("Clear database", () =>
                    program.NavigateTo<ClearDatabasePage>())
            )
        { }
    }
}
