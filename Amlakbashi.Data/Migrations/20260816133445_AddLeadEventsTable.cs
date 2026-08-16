using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Amlakbashi.Data.Migrations
{
    public partial class AddLeadEventsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LeadEvents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ResidenceId = table.Column<long>(type: "bigint", nullable: false),
                    HostUserId = table.Column<int>(type: "int", nullable: false),
                    GuestUserId = table.Column<int>(type: "int", nullable: true),
                    EventType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeduplicationKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientIp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeadEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeadEvents_Residences_ResidenceId",
                        column: x => x.ResidenceId,
                        principalTable: "Residences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LeadEvents_Users_GuestUserId",
                        column: x => x.GuestUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LeadEvents_Users_HostUserId",
                        column: x => x.HostUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LeadEvents_GuestUserId",
                table: "LeadEvents",
                column: "GuestUserId");

            migrationBuilder.CreateIndex(
                name: "IX_LeadEvents_HostUserId",
                table: "LeadEvents",
                column: "HostUserId");

            migrationBuilder.CreateIndex(
                name: "IX_LeadEvents_ResidenceId",
                table: "LeadEvents",
                column: "ResidenceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LeadEvents");
        }
    }
}
