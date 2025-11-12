using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyGymSystem.Migrations
{
    /// <inheritdoc />
    public partial class Models : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    Adminid = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Firstname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lastname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phonenumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Dateofbirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Imagepath = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.Adminid);
                });

            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    Memberid = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Firstname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lastname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phonenumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Dateofbirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Fitnessgoal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Imagepath = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.Memberid);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Paymentid = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Paymentdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amountpaid = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Paymentmethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Paymentstatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Externaltransactionid = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Paymentid);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Roleid = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Rolename = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Roleid);
                });

            migrationBuilder.CreateTable(
                name: "Trainers",
                columns: table => new
                {
                    Trainerid = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Firstname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lastname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phonenumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Specialization = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Imagepath = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trainers", x => x.Trainerid);
                });

            migrationBuilder.CreateTable(
                name: "Aboutuspages",
                columns: table => new
                {
                    Aboutuspageid = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Missionstatement = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Visionstatement = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Teamdescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Imagepath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Updatedbyadminid = table.Column<long>(type: "bigint", nullable: true),
                    Lastupdated = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aboutuspages", x => x.Aboutuspageid);
                    table.ForeignKey(
                        name: "FK_Aboutuspages_Admins_Updatedbyadminid",
                        column: x => x.Updatedbyadminid,
                        principalTable: "Admins",
                        principalColumn: "Adminid",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Contactuspages",
                columns: table => new
                {
                    Contactuspageid = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phonenumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Mapembedcode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Contenet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Picpath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Updatedbyadminid = table.Column<long>(type: "bigint", nullable: true),
                    Lastupdated = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contactuspages", x => x.Contactuspageid);
                    table.ForeignKey(
                        name: "FK_Contactuspages_Admins_Updatedbyadminid",
                        column: x => x.Updatedbyadminid,
                        principalTable: "Admins",
                        principalColumn: "Adminid",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Homepages",
                columns: table => new
                {
                    Homepageid = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Welcomemessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Featuredcontent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Updatedbyadminid = table.Column<long>(type: "bigint", nullable: true),
                    Lastupdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Sliderimagepath = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Homepages", x => x.Homepageid);
                    table.ForeignKey(
                        name: "FK_Homepages_Admins_Updatedbyadminid",
                        column: x => x.Updatedbyadminid,
                        principalTable: "Admins",
                        principalColumn: "Adminid",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Membershipplans",
                columns: table => new
                {
                    Planid = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Planname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Durationmonths = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Createdbyadminid = table.Column<long>(type: "bigint", nullable: true),
                    Plantype = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Membershipplans", x => x.Planid);
                    table.ForeignKey(
                        name: "FK_Membershipplans_Admins_Createdbyadminid",
                        column: x => x.Createdbyadminid,
                        principalTable: "Admins",
                        principalColumn: "Adminid",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Testimonials",
                columns: table => new
                {
                    Testimonialid = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Memberid = table.Column<long>(type: "bigint", nullable: true),
                    Feedbacktext = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Approvalstatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Submitteddate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Approvedbyadminid = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Testimonials", x => x.Testimonialid);
                    table.CheckConstraint("CK_Testimonial_Approvalstatus", "Approvalstatus in ('Pending','Approved','Rejected')");
                    table.ForeignKey(
                        name: "FK_Testimonials_Admins_Approvedbyadminid",
                        column: x => x.Approvedbyadminid,
                        principalTable: "Admins",
                        principalColumn: "Adminid");
                    table.ForeignKey(
                        name: "FK_Testimonials_Members_Memberid",
                        column: x => x.Memberid,
                        principalTable: "Members",
                        principalColumn: "Memberid");
                });

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    Invoiceid = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Paymentid = table.Column<long>(type: "bigint", nullable: true),
                    Invoicedate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Memberid = table.Column<long>(type: "bigint", nullable: true),
                    Filepath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: true),
                    Tax = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    Discount = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    Totalamount = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.Invoiceid);
                    table.ForeignKey(
                        name: "FK_Invoices_Members_Memberid",
                        column: x => x.Memberid,
                        principalTable: "Members",
                        principalColumn: "Memberid");
                    table.ForeignKey(
                        name: "FK_Invoices_Payments_Paymentid",
                        column: x => x.Paymentid,
                        principalTable: "Payments",
                        principalColumn: "Paymentid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Userlogins",
                columns: table => new
                {
                    Loginid = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Passwordhash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Roleid = table.Column<long>(type: "bigint", nullable: true),
                    Trainerid = table.Column<long>(type: "bigint", nullable: true),
                    Memberid = table.Column<long>(type: "bigint", nullable: true),
                    Adminid = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Userlogins", x => x.Loginid);
                    table.ForeignKey(
                        name: "FK_Userlogins_Admins_Adminid",
                        column: x => x.Adminid,
                        principalTable: "Admins",
                        principalColumn: "Adminid");
                    table.ForeignKey(
                        name: "FK_Userlogins_Members_Memberid",
                        column: x => x.Memberid,
                        principalTable: "Members",
                        principalColumn: "Memberid");
                    table.ForeignKey(
                        name: "FK_Userlogins_Roles_Roleid",
                        column: x => x.Roleid,
                        principalTable: "Roles",
                        principalColumn: "Roleid");
                    table.ForeignKey(
                        name: "FK_Userlogins_Trainers_Trainerid",
                        column: x => x.Trainerid,
                        principalTable: "Trainers",
                        principalColumn: "Trainerid");
                });

            migrationBuilder.CreateTable(
                name: "Workoutplans",
                columns: table => new
                {
                    Workoutplanid = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Trainerid = table.Column<long>(type: "bigint", nullable: true),
                    Memberid = table.Column<long>(type: "bigint", nullable: false),
                    Planname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Plandescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Startdate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Enddate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Routinedetails = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Schedule = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Goal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workoutplans", x => x.Workoutplanid);
                    table.ForeignKey(
                        name: "FK_Workoutplans_Members_Memberid",
                        column: x => x.Memberid,
                        principalTable: "Members",
                        principalColumn: "Memberid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Workoutplans_Trainers_Trainerid",
                        column: x => x.Trainerid,
                        principalTable: "Trainers",
                        principalColumn: "Trainerid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    Subscriptionid = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Memberid = table.Column<long>(type: "bigint", nullable: true),
                    Planid = table.Column<long>(type: "bigint", nullable: true),
                    Startdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Enddate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Paymentid = table.Column<long>(type: "bigint", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: true),
                    Createdat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.Subscriptionid);
                    table.ForeignKey(
                        name: "FK_Subscriptions_Members_Memberid",
                        column: x => x.Memberid,
                        principalTable: "Members",
                        principalColumn: "Memberid");
                    table.ForeignKey(
                        name: "FK_Subscriptions_Membershipplans_Planid",
                        column: x => x.Planid,
                        principalTable: "Membershipplans",
                        principalColumn: "Planid");
                    table.ForeignKey(
                        name: "FK_Subscriptions_Payments_Paymentid",
                        column: x => x.Paymentid,
                        principalTable: "Payments",
                        principalColumn: "Paymentid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Aboutuspages_Updatedbyadminid",
                table: "Aboutuspages",
                column: "Updatedbyadminid");

            migrationBuilder.CreateIndex(
                name: "IX_Contactuspages_Updatedbyadminid",
                table: "Contactuspages",
                column: "Updatedbyadminid");

            migrationBuilder.CreateIndex(
                name: "IX_Homepages_Updatedbyadminid",
                table: "Homepages",
                column: "Updatedbyadminid");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_Memberid",
                table: "Invoices",
                column: "Memberid");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_Paymentid",
                table: "Invoices",
                column: "Paymentid",
                unique: true,
                filter: "[Paymentid] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Membershipplans_Createdbyadminid",
                table: "Membershipplans",
                column: "Createdbyadminid");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_Createdat",
                table: "Subscriptions",
                column: "Createdat");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_Enddate",
                table: "Subscriptions",
                column: "Enddate");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_Memberid",
                table: "Subscriptions",
                column: "Memberid");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_Paymentid",
                table: "Subscriptions",
                column: "Paymentid",
                unique: true,
                filter: "[Paymentid] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_Planid",
                table: "Subscriptions",
                column: "Planid");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_Startdate",
                table: "Subscriptions",
                column: "Startdate");

            migrationBuilder.CreateIndex(
                name: "IX_Testimonials_Approvedbyadminid",
                table: "Testimonials",
                column: "Approvedbyadminid");

            migrationBuilder.CreateIndex(
                name: "IX_Testimonials_Memberid",
                table: "Testimonials",
                column: "Memberid");

            migrationBuilder.CreateIndex(
                name: "IX_Userlogins_Adminid",
                table: "Userlogins",
                column: "Adminid");

            migrationBuilder.CreateIndex(
                name: "IX_Userlogins_Memberid",
                table: "Userlogins",
                column: "Memberid");

            migrationBuilder.CreateIndex(
                name: "IX_Userlogins_Roleid",
                table: "Userlogins",
                column: "Roleid");

            migrationBuilder.CreateIndex(
                name: "IX_Userlogins_Trainerid",
                table: "Userlogins",
                column: "Trainerid");

            migrationBuilder.CreateIndex(
                name: "IX_Userlogins_Username",
                table: "Userlogins",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Workoutplans_Memberid",
                table: "Workoutplans",
                column: "Memberid");

            migrationBuilder.CreateIndex(
                name: "IX_Workoutplans_Trainerid",
                table: "Workoutplans",
                column: "Trainerid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Aboutuspages");

            migrationBuilder.DropTable(
                name: "Contactuspages");

            migrationBuilder.DropTable(
                name: "Homepages");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropTable(
                name: "Testimonials");

            migrationBuilder.DropTable(
                name: "Userlogins");

            migrationBuilder.DropTable(
                name: "Workoutplans");

            migrationBuilder.DropTable(
                name: "Membershipplans");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Members");

            migrationBuilder.DropTable(
                name: "Trainers");

            migrationBuilder.DropTable(
                name: "Admins");
        }
    }
}
