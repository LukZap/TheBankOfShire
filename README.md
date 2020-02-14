# TheBankOfShire
## Setup and running project (using Visual Studio)
1. Clone solution
2. Change 'connection strings' in ShireBank/App.config and ShireBank/Utils/BankContextFactory.cs
3. Build solution
4. Open Package Manager Console, change Default Project to ShireBank and run 'update-database' command to create Sqlite .db file locally
5. Right-click on ShireBank solution, click on Setup StartUp projects, choose Multiple Startup Projects and set CustometTest, InspectorTest, ShireBank action to Start
6. Press Start to run application
