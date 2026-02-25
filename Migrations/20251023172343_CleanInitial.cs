using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaxiDispatcherSystem.Migrations
{
    /// <inheritdoc />
    public partial class CleanInitial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Fares",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    БазовийТариф = table.Column<double>(type: "float", nullable: false),
                    ЦінаЗаКм = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fares", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Адреса = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Широта = table.Column<double>(type: "float", nullable: false),
                    Довгота = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Сума = table.Column<double>(type: "float", nullable: false),
                    Статус = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    НомернийЗнак = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Марка = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Модель = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Колір = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ім_я = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Прізвище = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Телефон = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Роль = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Discriminator = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    Доступний = table.Column<bool>(type: "bit", nullable: true),
                    ПоточнийСтатус = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    АвтомобільId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Vehicles_АвтомобільId",
                        column: x => x.АвтомобільId,
                        principalTable: "Vehicles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Статус = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ЧасСтворення = table.Column<DateTime>(type: "datetime2", nullable: false),
                    КлієнтId = table.Column<int>(type: "int", nullable: false),
                    ПризначенийВодійId = table.Column<int>(type: "int", nullable: true),
                    МісцеВідправленняId = table.Column<int>(type: "int", nullable: false),
                    МісцеПризначенняId = table.Column<int>(type: "int", nullable: false),
                    ТарифId = table.Column<int>(type: "int", nullable: false),
                    ОплатаId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Fares_ТарифId",
                        column: x => x.ТарифId,
                        principalTable: "Fares",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_Locations_МісцеВідправленняId",
                        column: x => x.МісцеВідправленняId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_Locations_МісцеПризначенняId",
                        column: x => x.МісцеПризначенняId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_Payments_ОплатаId",
                        column: x => x.ОплатаId,
                        principalTable: "Payments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_Users_КлієнтId",
                        column: x => x.КлієнтId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_Users_ПризначенийВодійId",
                        column: x => x.ПризначенийВодійId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_КлієнтId",
                table: "Orders",
                column: "КлієнтId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_МісцеВідправленняId",
                table: "Orders",
                column: "МісцеВідправленняId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_МісцеПризначенняId",
                table: "Orders",
                column: "МісцеПризначенняId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ОплатаId",
                table: "Orders",
                column: "ОплатаId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ПризначенийВодійId",
                table: "Orders",
                column: "ПризначенийВодійId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ТарифId",
                table: "Orders",
                column: "ТарифId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_АвтомобільId",
                table: "Users",
                column: "АвтомобільId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Fares");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Vehicles");
        }
    }
}
