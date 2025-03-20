using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingApplication.Dal.Migrations
{
    public partial class MigracionInicial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "_TableHotels",
                columns: table => new
                {
                    HotelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HotelName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StarsAssigned = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TableHotels", x => x.HotelId);
                });

            migrationBuilder.CreateTable(
                name: "_TableRooms",
                columns: table => new
                {
                    RoomId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoomNumber = table.Column<int>(type: "int", nullable: false),
                    RoomSize = table.Column<double>(type: "float", nullable: false),
                    NeedRepair = table.Column<bool>(type: "bit", nullable: false),
                    HotelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TableRooms", x => x.RoomId);
                    table.ForeignKey(
                        name: "FK__TableRooms__TableHotels_HotelId",
                        column: x => x.HotelId,
                        principalTable: "_TableHotels",
                        principalColumn: "HotelId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "_TableReservations",
                columns: table => new
                {
                    ReservationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoomId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    HotelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Customer = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TableReservations", x => x.ReservationId);
                    table.ForeignKey(
                        name: "FK__TableReservations__TableHotels_HotelId",
                        column: x => x.HotelId,
                        principalTable: "_TableHotels",
                        principalColumn: "HotelId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__TableReservations__TableRooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "_TableRooms",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "_TableRoomReservationDates",
                columns: table => new
                {
                    RoomReservationDateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReservationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RoomId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TableRoomReservationDates", x => x.RoomReservationDateId);
                    table.ForeignKey(
                        name: "FK__TableRoomReservationDates__TableRooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "_TableRooms",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "_TableHotelReservationDates",
                columns: table => new
                {
                    HotelReservationDateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReservationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReservationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TableHotelReservationDates", x => x.HotelReservationDateId);
                    table.ForeignKey(
                        name: "FK__TableHotelReservationDates__TableReservations_ReservationId",
                        column: x => x.ReservationId,
                        principalTable: "_TableReservations",
                        principalColumn: "ReservationId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX__TableHotelReservationDates_ReservationId",
                table: "_TableHotelReservationDates",
                column: "ReservationId");

            migrationBuilder.CreateIndex(
                name: "IX__TableReservations_HotelId",
                table: "_TableReservations",
                column: "HotelId");

            migrationBuilder.CreateIndex(
                name: "IX__TableReservations_RoomId",
                table: "_TableReservations",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX__TableRoomReservationDates_RoomId",
                table: "_TableRoomReservationDates",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX__TableRooms_HotelId",
                table: "_TableRooms",
                column: "HotelId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "_TableHotelReservationDates");

            migrationBuilder.DropTable(
                name: "_TableRoomReservationDates");

            migrationBuilder.DropTable(
                name: "_TableReservations");

            migrationBuilder.DropTable(
                name: "_TableRooms");

            migrationBuilder.DropTable(
                name: "_TableHotels");
        }
    }
}
