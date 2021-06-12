This is a simple .NET 5 console app where UK sole traders (self-employed workers) can log monetary transactions and calculate the income tax they owe to the government. 
I started this project so I could get some practical experience of working with databases - and, of course, because I wanted to procrastinate filing my own taxes :)
I chose SQLite for its simplicity but have used DI in case I want to use a more fully-featured solution instead.

## Features

- Persistant data storage
- Page-based navigation system
- Pleasant aesthetic touches, like presenting data in tables
- Fuzzy search system for user names

## Technology used

- Data is stored persistently through SQLite and managed with Dapper
- Console menu navigation is crafted with [splttingatms' EasyConsole package](https://github.com/splttingatms/EasyConsole)
- Aesthetic effects, like coloured text and presenting user data in tables, is done with [SpectreConsole](https://spectreconsole.net/)
- Strings are made presentable to the user with [Humanizer]https://humanizr.net/)
- Testing is done with [XUnit](https://xunit.net/) and [FluentAssertions](https://fluentassertions.com/)
- Fuzzy search is done by [FuzzySharp](https://github.com/JakeBayer/FuzzySharp)

## Known issues and plans

- Improve the tax calculation system using the information on [the UK government website](https://www.gov.uk/)
- Add authentication system so someone else can't see your financial data!
- Add more unit-tests
