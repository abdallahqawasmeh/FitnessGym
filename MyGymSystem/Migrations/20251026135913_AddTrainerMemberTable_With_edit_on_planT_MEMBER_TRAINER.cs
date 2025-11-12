using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyGymSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddTrainerMemberTable_With_edit_on_planT_MEMBER_TRAINER : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description1",
                table: "Membershipplans",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description2",
                table: "Membershipplans",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description3",
                table: "Membershipplans",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description4",
                table: "Membershipplans",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MemberTrainer",
                columns: table => new
                {
                    MemberTrainerId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberId = table.Column<long>(type: "bigint", nullable: false),
                    TrainerId = table.Column<long>(type: "bigint", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UnassignedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberTrainer", x => x.MemberTrainerId);
                    table.ForeignKey(
                        name: "FK_MemberTrainer_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Memberid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MemberTrainer_Trainers_TrainerId",
                        column: x => x.TrainerId,
                        principalTable: "Trainers",
                        principalColumn: "Trainerid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MemberTrainer_MemberId_IsActive",
                table: "MemberTrainer",
                columns: new[] { "MemberId", "IsActive" },
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_MemberTrainer_TrainerId",
                table: "MemberTrainer",
                column: "TrainerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MemberTrainer");

            migrationBuilder.DropColumn(
                name: "Description1",
                table: "Membershipplans");

            migrationBuilder.DropColumn(
                name: "Description2",
                table: "Membershipplans");

            migrationBuilder.DropColumn(
                name: "Description3",
                table: "Membershipplans");

            migrationBuilder.DropColumn(
                name: "Description4",
                table: "Membershipplans");
        }
    }
}
