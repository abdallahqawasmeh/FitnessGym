using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyGymSystem.Migrations
{
    /// <inheritdoc />
    public partial class Dynamic_HomePage_Edited : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SliderCaption1",
                table: "Homepages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SliderCaption2",
                table: "Homepages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SliderCaption3",
                table: "Homepages",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SliderCaption1",
                table: "Homepages");

            migrationBuilder.DropColumn(
                name: "SliderCaption2",
                table: "Homepages");

            migrationBuilder.DropColumn(
                name: "SliderCaption3",
                table: "Homepages");
        }
    }
}
