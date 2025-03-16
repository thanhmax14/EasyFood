using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class migrationsv13 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Time",
                table: "BalanceChanges",
                newName: "StartTime");

            migrationBuilder.AddColumn<DateTime>(
                name: "DueTime",
                table: "BalanceChanges",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DueTime",
                table: "BalanceChanges");

            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "BalanceChanges",
                newName: "Time");
        }
    }
}
