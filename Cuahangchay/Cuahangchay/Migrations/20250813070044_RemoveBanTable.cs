using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cuahangchay.Migrations
{
    /// <inheritdoc />
    public partial class RemoveBanTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HoaDon_Ban_BanID",
                table: "HoaDon");

            migrationBuilder.DropIndex(
                name: "IX_HoaDon_BanID",
                table: "HoaDon");

            migrationBuilder.DropColumn(
                name: "BanID",
                table: "HoaDon");
            migrationBuilder.DropTable(
    name: "Ban");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<int>(
            //    name: "BanID",
            //    table: "HoaDon",
            //    type: "int",
            //    nullable: false,
            //    defaultValue: 0);

            //migrationBuilder.CreateIndex(
            //    name: "IX_HoaDon_BanID",
            //    table: "HoaDon",
            //    column: "BanID");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_HoaDon_Ban_BanID",
            //    table: "HoaDon",
            //    column: "BanID",
            //    principalTable: "Ban",
            //    principalColumn: "BanID",
            //    onDelete: ReferentialAction.Cascade);

        }
    }
}
