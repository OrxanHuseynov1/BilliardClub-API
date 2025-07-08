using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddTableActiveSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ActiveSessionId",
                table: "Tables",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tables_ActiveSessionId",
                table: "Tables",
                column: "ActiveSessionId",
                unique: true,
                filter: "[ActiveSessionId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Tables_TableSessions_ActiveSessionId",
                table: "Tables",
                column: "ActiveSessionId",
                principalTable: "TableSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tables_TableSessions_ActiveSessionId",
                table: "Tables");

            migrationBuilder.DropIndex(
                name: "IX_Tables_ActiveSessionId",
                table: "Tables");

            migrationBuilder.DropColumn(
                name: "ActiveSessionId",
                table: "Tables");
        }
    }
}
