using Microsoft.EntityFrameworkCore.Migrations;

namespace ShireBank.Migrations
{
    public partial class addForeignKeyConstraintToAccountHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                PRAGMA foreign_keys=off;

                ALTER TABLE AccountHistories RENAME TO _AccountHistories_old;

                CREATE TABLE AccountHistories (
                    Id TEXT NOT NULL CONSTRAINT PK_AccountHistories PRIMARY KEY,
                    AccountId INTEGER NOT NULL,
                    FirstName TEXT NULL,
                    LastName TEXT NULL,
                    DebtLimit REAL NOT NULL,
                    Balance REAL NOT NULL,
                    EntryDate TEXT NOT NULL,
                    CONSTRAINT FK_Accounts
                        FOREIGN KEY (AccountId)
                        REFERENCES Accounts(Id)
                        ON DELETE CASCADE
                );

                INSERT INTO AccountHistories SELECT * FROM _AccountHistories_old;
                
                DROP TABLE _AccountHistories_old;

                PRAGMA foreign_keys=on;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
