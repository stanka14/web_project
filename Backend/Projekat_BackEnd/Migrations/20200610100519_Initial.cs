using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Projekat_BackEnd.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AirlineCompanies",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    Lon = table.Column<double>(nullable: false),
                    Lat = table.Column<double>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Img = table.Column<string>(nullable: true),
                    Rating = table.Column<int>(nullable: false),
                    Admin = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AirlineCompanies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Discount",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BronzeTier = table.Column<double>(nullable: false),
                    SilverTier = table.Column<double>(nullable: false),
                    GoldTier = table.Column<double>(nullable: false),
                    DiscountPercent = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discount", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Luggage",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Weight = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    Dimensions = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Luggage", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Traveller",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    IdUser = table.Column<string>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Passport = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Traveller", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(150)", nullable: true),
                    Address = table.Column<string>(nullable: true),
                    Birthday = table.Column<DateTime>(nullable: false),
                    Passport = table.Column<string>(nullable: true),
                    Points = table.Column<int>(nullable: false),
                    CompanyId = table.Column<int>(nullable: false),
                    ChangedPassword = table.Column<bool>(nullable: false),
                    MainWebsiteAdministrator = table.Column<bool>(nullable: false),
                    DiscountId = table.Column<int>(nullable: true),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Discount_DiscountId",
                        column: x => x.DiscountId,
                        principalTable: "Discount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(maxLength: 128, nullable: false),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FriendRequestReceived",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    User = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendRequestReceived", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FriendRequestReceived_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FriendRequestSent",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    User = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendRequestSent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FriendRequestSent_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notification_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RentACarCompanies",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    MainLocationId = table.Column<int>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Image = table.Column<string>(nullable: true),
                    AvrageRating = table.Column<int>(nullable: false),
                    Activated = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RentACarCompanies", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullAddress = table.Column<string>(nullable: true),
                    Longitude = table.Column<double>(nullable: false),
                    Latitude = table.Column<double>(nullable: false),
                    RentACarID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Address_RentACarCompanies_RentACarID",
                        column: x => x.RentACarID,
                        principalTable: "RentACarCompanies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Car",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Brand = table.Column<string>(nullable: true),
                    Model = table.Column<string>(nullable: true),
                    Year = table.Column<int>(nullable: false),
                    PricePerDay = table.Column<double>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    Passengers = table.Column<int>(nullable: false),
                    Location = table.Column<string>(nullable: true),
                    Image = table.Column<string>(nullable: true),
                    CompanyId = table.Column<int>(nullable: false),
                    AvrageRating = table.Column<int>(nullable: false),
                    Removed = table.Column<bool>(nullable: false),
                    RentACarID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Car", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Car_RentACarCompanies_RentACarID",
                        column: x => x.RentACarID,
                        principalTable: "RentACarCompanies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarReservation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CarID = table.Column<int>(nullable: true),
                    RatedCar = table.Column<int>(nullable: false),
                    RatedCompany = table.Column<int>(nullable: false),
                    PickUpLocation = table.Column<string>(nullable: true),
                    ReturnLocation = table.Column<string>(nullable: true),
                    PickUpTime = table.Column<string>(nullable: true),
                    ReturnTime = table.Column<string>(nullable: true),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    TotalPrice = table.Column<double>(nullable: false),
                    RentACarID = table.Column<int>(nullable: true),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarReservation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarReservation_Car_CarID",
                        column: x => x.CarID,
                        principalTable: "Car",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarReservation_RentACarCompanies_RentACarID",
                        column: x => x.RentACarID,
                        principalTable: "RentACarCompanies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarReservation_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuickReservation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DiscountedCarID = table.Column<int>(nullable: true),
                    RentACarID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuickReservation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuickReservation_Car_DiscountedCarID",
                        column: x => x.DiscountedCarID,
                        principalTable: "Car",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuickReservation_RentACarCompanies_RentACarID",
                        column: x => x.RentACarID,
                        principalTable: "RentACarCompanies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Rating",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Rate = table.Column<int>(nullable: false),
                    CarID = table.Column<int>(nullable: true),
                    RentACarID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rating", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rating_Car_CarID",
                        column: x => x.CarID,
                        principalTable: "Car",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rating_RentACarCompanies_RentACarID",
                        column: x => x.RentACarID,
                        principalTable: "RentACarCompanies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExtraAmenity",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Price = table.Column<double>(nullable: false),
                    Image = table.Column<string>(nullable: true),
                    OneTimePayment = table.Column<bool>(nullable: false),
                    CarReservationId = table.Column<int>(nullable: true),
                    RentACarID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExtraAmenity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExtraAmenity_CarReservation_CarReservationId",
                        column: x => x.CarReservationId,
                        principalTable: "CarReservation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExtraAmenity_RentACarCompanies_RentACarID",
                        column: x => x.RentACarID,
                        principalTable: "RentACarCompanies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Date",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateStr = table.Column<string>(nullable: true),
                    CarID = table.Column<int>(nullable: true),
                    CarReservationId = table.Column<int>(nullable: true),
                    QuickReservationId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Date", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Date_Car_CarID",
                        column: x => x.CarID,
                        principalTable: "Car",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Date_CarReservation_CarReservationId",
                        column: x => x.CarReservationId,
                        principalTable: "CarReservation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Date_QuickReservation_QuickReservationId",
                        column: x => x.QuickReservationId,
                        principalTable: "QuickReservation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Destination",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Img = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    AirlineId = table.Column<int>(nullable: true),
                    FlightId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Destination", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Destination_AirlineCompanies_AirlineId",
                        column: x => x.AirlineId,
                        principalTable: "AirlineCompanies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DestinationPopular",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    destinationId = table.Column<int>(nullable: true),
                    AirlineId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DestinationPopular", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DestinationPopular_AirlineCompanies_AirlineId",
                        column: x => x.AirlineId,
                        principalTable: "AirlineCompanies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DestinationPopular_Destination_destinationId",
                        column: x => x.destinationId,
                        principalTable: "Destination",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Flight",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCompany = table.Column<string>(nullable: true),
                    Duration = table.Column<string>(nullable: true),
                    Extra = table.Column<string>(nullable: true),
                    PovratniletId = table.Column<int>(nullable: true),
                    Price = table.Column<int>(nullable: false),
                    NumOfPassengers = table.Column<int>(nullable: false),
                    DepartureDate = table.Column<string>(nullable: true),
                    LuggageId = table.Column<int>(nullable: true),
                    FromId = table.Column<int>(nullable: true),
                    ToId = table.Column<int>(nullable: true),
                    Rate = table.Column<int>(nullable: false),
                    Trip = table.Column<int>(nullable: false),
                    AirlineId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flight", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Flight_AirlineCompanies_AirlineId",
                        column: x => x.AirlineId,
                        principalTable: "AirlineCompanies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Flight_Destination_FromId",
                        column: x => x.FromId,
                        principalTable: "Destination",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Flight_Luggage_LuggageId",
                        column: x => x.LuggageId,
                        principalTable: "Luggage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Flight_Flight_PovratniletId",
                        column: x => x.PovratniletId,
                        principalTable: "Flight",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Flight_Destination_ToId",
                        column: x => x.ToId,
                        principalTable: "Destination",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Rater",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Rate = table.Column<int>(nullable: false),
                    AirlineId = table.Column<int>(nullable: true),
                    FlightId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rater", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rater_AirlineCompanies_AirlineId",
                        column: x => x.AirlineId,
                        principalTable: "AirlineCompanies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rater_Flight_FlightId",
                        column: x => x.FlightId,
                        principalTable: "Flight",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Seat",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Taken = table.Column<bool>(nullable: false),
                    IsSelected = table.Column<bool>(nullable: false),
                    TravellerId = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    FlightId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seat", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Seat_Flight_FlightId",
                        column: x => x.FlightId,
                        principalTable: "Flight",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Seat_Traveller_TravellerId",
                        column: x => x.TravellerId,
                        principalTable: "Traveller",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ticket",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FlightId = table.Column<int>(nullable: true),
                    SeatId = table.Column<int>(nullable: true),
                    Discount = table.Column<int>(nullable: false),
                    gaveRate = table.Column<bool>(nullable: false),
                    gaveRateCompany = table.Column<bool>(nullable: false),
                    AirlineId = table.Column<int>(nullable: true),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ticket", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ticket_AirlineCompanies_AirlineId",
                        column: x => x.AirlineId,
                        principalTable: "AirlineCompanies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ticket_Flight_FlightId",
                        column: x => x.FlightId,
                        principalTable: "Flight",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ticket_Seat_SeatId",
                        column: x => x.SeatId,
                        principalTable: "Seat",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ticket_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TicketInvitation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketId = table.Column<int>(nullable: true),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketInvitation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketInvitation_Ticket_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Ticket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketInvitation_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Address_RentACarID",
                table: "Address",
                column: "RentACarID");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_DiscountId",
                table: "AspNetUsers",
                column: "DiscountId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_UserId",
                table: "AspNetUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Car_RentACarID",
                table: "Car",
                column: "RentACarID");

            migrationBuilder.CreateIndex(
                name: "IX_CarReservation_CarID",
                table: "CarReservation",
                column: "CarID");

            migrationBuilder.CreateIndex(
                name: "IX_CarReservation_RentACarID",
                table: "CarReservation",
                column: "RentACarID");

            migrationBuilder.CreateIndex(
                name: "IX_CarReservation_UserId",
                table: "CarReservation",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Date_CarID",
                table: "Date",
                column: "CarID");

            migrationBuilder.CreateIndex(
                name: "IX_Date_CarReservationId",
                table: "Date",
                column: "CarReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_Date_QuickReservationId",
                table: "Date",
                column: "QuickReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_Destination_AirlineId",
                table: "Destination",
                column: "AirlineId");

            migrationBuilder.CreateIndex(
                name: "IX_Destination_FlightId",
                table: "Destination",
                column: "FlightId");

            migrationBuilder.CreateIndex(
                name: "IX_DestinationPopular_AirlineId",
                table: "DestinationPopular",
                column: "AirlineId");

            migrationBuilder.CreateIndex(
                name: "IX_DestinationPopular_destinationId",
                table: "DestinationPopular",
                column: "destinationId");

            migrationBuilder.CreateIndex(
                name: "IX_ExtraAmenity_CarReservationId",
                table: "ExtraAmenity",
                column: "CarReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_ExtraAmenity_RentACarID",
                table: "ExtraAmenity",
                column: "RentACarID");

            migrationBuilder.CreateIndex(
                name: "IX_Flight_AirlineId",
                table: "Flight",
                column: "AirlineId");

            migrationBuilder.CreateIndex(
                name: "IX_Flight_FromId",
                table: "Flight",
                column: "FromId");

            migrationBuilder.CreateIndex(
                name: "IX_Flight_LuggageId",
                table: "Flight",
                column: "LuggageId");

            migrationBuilder.CreateIndex(
                name: "IX_Flight_PovratniletId",
                table: "Flight",
                column: "PovratniletId");

            migrationBuilder.CreateIndex(
                name: "IX_Flight_ToId",
                table: "Flight",
                column: "ToId");

            migrationBuilder.CreateIndex(
                name: "IX_FriendRequestReceived_UserId",
                table: "FriendRequestReceived",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FriendRequestSent_UserId",
                table: "FriendRequestSent",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_UserId",
                table: "Notification",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_QuickReservation_DiscountedCarID",
                table: "QuickReservation",
                column: "DiscountedCarID");

            migrationBuilder.CreateIndex(
                name: "IX_QuickReservation_RentACarID",
                table: "QuickReservation",
                column: "RentACarID");

            migrationBuilder.CreateIndex(
                name: "IX_Rater_AirlineId",
                table: "Rater",
                column: "AirlineId");

            migrationBuilder.CreateIndex(
                name: "IX_Rater_FlightId",
                table: "Rater",
                column: "FlightId");

            migrationBuilder.CreateIndex(
                name: "IX_Rating_CarID",
                table: "Rating",
                column: "CarID");

            migrationBuilder.CreateIndex(
                name: "IX_Rating_RentACarID",
                table: "Rating",
                column: "RentACarID");

            migrationBuilder.CreateIndex(
                name: "IX_RentACarCompanies_MainLocationId",
                table: "RentACarCompanies",
                column: "MainLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Seat_FlightId",
                table: "Seat",
                column: "FlightId");

            migrationBuilder.CreateIndex(
                name: "IX_Seat_TravellerId",
                table: "Seat",
                column: "TravellerId");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_AirlineId",
                table: "Ticket",
                column: "AirlineId");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_FlightId",
                table: "Ticket",
                column: "FlightId");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_SeatId",
                table: "Ticket",
                column: "SeatId");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_UserId",
                table: "Ticket",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketInvitation_TicketId",
                table: "TicketInvitation",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketInvitation_UserId",
                table: "TicketInvitation",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_RentACarCompanies_Address_MainLocationId",
                table: "RentACarCompanies",
                column: "MainLocationId",
                principalTable: "Address",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Destination_Flight_FlightId",
                table: "Destination",
                column: "FlightId",
                principalTable: "Flight",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Address_RentACarCompanies_RentACarID",
                table: "Address");

            migrationBuilder.DropForeignKey(
                name: "FK_Destination_AirlineCompanies_AirlineId",
                table: "Destination");

            migrationBuilder.DropForeignKey(
                name: "FK_Flight_AirlineCompanies_AirlineId",
                table: "Flight");

            migrationBuilder.DropForeignKey(
                name: "FK_Destination_Flight_FlightId",
                table: "Destination");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Date");

            migrationBuilder.DropTable(
                name: "DestinationPopular");

            migrationBuilder.DropTable(
                name: "ExtraAmenity");

            migrationBuilder.DropTable(
                name: "FriendRequestReceived");

            migrationBuilder.DropTable(
                name: "FriendRequestSent");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "Rater");

            migrationBuilder.DropTable(
                name: "Rating");

            migrationBuilder.DropTable(
                name: "TicketInvitation");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "QuickReservation");

            migrationBuilder.DropTable(
                name: "CarReservation");

            migrationBuilder.DropTable(
                name: "Ticket");

            migrationBuilder.DropTable(
                name: "Car");

            migrationBuilder.DropTable(
                name: "Seat");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Traveller");

            migrationBuilder.DropTable(
                name: "Discount");

            migrationBuilder.DropTable(
                name: "RentACarCompanies");

            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropTable(
                name: "AirlineCompanies");

            migrationBuilder.DropTable(
                name: "Flight");

            migrationBuilder.DropTable(
                name: "Destination");

            migrationBuilder.DropTable(
                name: "Luggage");
        }
    }
}
