using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyGymSystem.Migrations
{
    /// <inheritdoc />
    public partial class trainerWorkoutPlansPrivate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Members_Memberid",
                table: "Subscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Membershipplans_Planid",
                table: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_MemberTrainer_MemberId_IsActive",
                table: "MemberTrainer");

            migrationBuilder.AlterColumn<long>(
                name: "Trainerid",
                table: "Workoutplans",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "Planid",
                table: "Subscriptions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "Memberid",
                table: "Subscriptions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Trainerid",
                table: "Subscriptions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SubscriptionId",
                table: "MemberTrainer",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_Trainerid",
                table: "Subscriptions",
                column: "Trainerid");

            migrationBuilder.CreateIndex(
                name: "IX_MemberTrainer_MemberId",
                table: "MemberTrainer",
                column: "MemberId",
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_MemberTrainer_SubscriptionId",
                table: "MemberTrainer",
                column: "SubscriptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_MemberTrainer_Subscriptions_SubscriptionId",
                table: "MemberTrainer",
                column: "SubscriptionId",
                principalTable: "Subscriptions",
                principalColumn: "Subscriptionid",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Members_Memberid",
                table: "Subscriptions",
                column: "Memberid",
                principalTable: "Members",
                principalColumn: "Memberid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Membershipplans_Planid",
                table: "Subscriptions",
                column: "Planid",
                principalTable: "Membershipplans",
                principalColumn: "Planid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Trainers_Trainerid",
                table: "Subscriptions",
                column: "Trainerid",
                principalTable: "Trainers",
                principalColumn: "Trainerid",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MemberTrainer_Subscriptions_SubscriptionId",
                table: "MemberTrainer");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Members_Memberid",
                table: "Subscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Membershipplans_Planid",
                table: "Subscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Trainers_Trainerid",
                table: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_Trainerid",
                table: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_MemberTrainer_MemberId",
                table: "MemberTrainer");

            migrationBuilder.DropIndex(
                name: "IX_MemberTrainer_SubscriptionId",
                table: "MemberTrainer");

            migrationBuilder.DropColumn(
                name: "Trainerid",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "SubscriptionId",
                table: "MemberTrainer");

            migrationBuilder.AlterColumn<long>(
                name: "Trainerid",
                table: "Workoutplans",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "Planid",
                table: "Subscriptions",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "Memberid",
                table: "Subscriptions",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateIndex(
                name: "IX_MemberTrainer_MemberId_IsActive",
                table: "MemberTrainer",
                columns: new[] { "MemberId", "IsActive" },
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Members_Memberid",
                table: "Subscriptions",
                column: "Memberid",
                principalTable: "Members",
                principalColumn: "Memberid");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Membershipplans_Planid",
                table: "Subscriptions",
                column: "Planid",
                principalTable: "Membershipplans",
                principalColumn: "Planid");
        }
    }
}
