using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Prioritize.Data.Migrations
{
    /// <inheritdoc />
    public partial class CompletedDateTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CompletedDateTime",
                table: "Tasks",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedDateTime",
                table: "Tasks");
        }
    }
}
