using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyGymSystem.Migrations
{
    /// <inheritdoc />
    public partial class Dynamic_Pages_Edited_and_added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HeroButtonText",
                table: "Homepages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HeroButtonUrl",
                table: "Homepages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HeroSubtitle",
                table: "Homepages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HeroTitle",
                table: "Homepages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sliderimagepath1",
                table: "Homepages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sliderimagepath2",
                table: "Homepages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sliderimagepath3",
                table: "Homepages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IntroText",
                table: "Contactuspages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PageTitle",
                table: "Contactuspages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WorkingHours",
                table: "Contactuspages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IntroText",
                table: "Aboutuspages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PageTitle",
                table: "Aboutuspages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Value1Text",
                table: "Aboutuspages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Value1Title",
                table: "Aboutuspages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Value2Text",
                table: "Aboutuspages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Value2Title",
                table: "Aboutuspages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Value3Text",
                table: "Aboutuspages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Value3Title",
                table: "Aboutuspages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Membershipplanspages",
                columns: table => new
                {
                    Membershipplanspageid = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PageTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IntroText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Bannerimagepath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Updatedbyadminid = table.Column<long>(type: "bigint", nullable: true),
                    Lastupdated = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Membershipplanspages", x => x.Membershipplanspageid);
                    table.ForeignKey(
                        name: "FK_Membershipplanspages_Admins_Updatedbyadminid",
                        column: x => x.Updatedbyadminid,
                        principalTable: "Admins",
                        principalColumn: "Adminid",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Testimonialspages",
                columns: table => new
                {
                    Testimonialspageid = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PageTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IntroText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Bannerimagepath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Updatedbyadminid = table.Column<long>(type: "bigint", nullable: true),
                    Lastupdated = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Testimonialspages", x => x.Testimonialspageid);
                    table.ForeignKey(
                        name: "FK_Testimonialspages_Admins_Updatedbyadminid",
                        column: x => x.Updatedbyadminid,
                        principalTable: "Admins",
                        principalColumn: "Adminid",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Trainerspages",
                columns: table => new
                {
                    Trainerspageid = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PageTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IntroText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Bannerimagepath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Updatedbyadminid = table.Column<long>(type: "bigint", nullable: true),
                    Lastupdated = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trainerspages", x => x.Trainerspageid);
                    table.ForeignKey(
                        name: "FK_Trainerspages_Admins_Updatedbyadminid",
                        column: x => x.Updatedbyadminid,
                        principalTable: "Admins",
                        principalColumn: "Adminid",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Membershipplanspages_Updatedbyadminid",
                table: "Membershipplanspages",
                column: "Updatedbyadminid");

            migrationBuilder.CreateIndex(
                name: "IX_Testimonialspages_Updatedbyadminid",
                table: "Testimonialspages",
                column: "Updatedbyadminid");

            migrationBuilder.CreateIndex(
                name: "IX_Trainerspages_Updatedbyadminid",
                table: "Trainerspages",
                column: "Updatedbyadminid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Membershipplanspages");

            migrationBuilder.DropTable(
                name: "Testimonialspages");

            migrationBuilder.DropTable(
                name: "Trainerspages");

            migrationBuilder.DropColumn(
                name: "HeroButtonText",
                table: "Homepages");

            migrationBuilder.DropColumn(
                name: "HeroButtonUrl",
                table: "Homepages");

            migrationBuilder.DropColumn(
                name: "HeroSubtitle",
                table: "Homepages");

            migrationBuilder.DropColumn(
                name: "HeroTitle",
                table: "Homepages");

            migrationBuilder.DropColumn(
                name: "Sliderimagepath1",
                table: "Homepages");

            migrationBuilder.DropColumn(
                name: "Sliderimagepath2",
                table: "Homepages");

            migrationBuilder.DropColumn(
                name: "Sliderimagepath3",
                table: "Homepages");

            migrationBuilder.DropColumn(
                name: "IntroText",
                table: "Contactuspages");

            migrationBuilder.DropColumn(
                name: "PageTitle",
                table: "Contactuspages");

            migrationBuilder.DropColumn(
                name: "WorkingHours",
                table: "Contactuspages");

            migrationBuilder.DropColumn(
                name: "IntroText",
                table: "Aboutuspages");

            migrationBuilder.DropColumn(
                name: "PageTitle",
                table: "Aboutuspages");

            migrationBuilder.DropColumn(
                name: "Value1Text",
                table: "Aboutuspages");

            migrationBuilder.DropColumn(
                name: "Value1Title",
                table: "Aboutuspages");

            migrationBuilder.DropColumn(
                name: "Value2Text",
                table: "Aboutuspages");

            migrationBuilder.DropColumn(
                name: "Value2Title",
                table: "Aboutuspages");

            migrationBuilder.DropColumn(
                name: "Value3Text",
                table: "Aboutuspages");

            migrationBuilder.DropColumn(
                name: "Value3Title",
                table: "Aboutuspages");
        }
    }
}
