using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthRec.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Records_PatientId",
                table: "Records",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Records_Patients_PatientId",
                table: "Records",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Records_Patients_PatientId",
                table: "Records");

            migrationBuilder.DropIndex(
                name: "IX_Records_PatientId",
                table: "Records");
        }
    }
}
