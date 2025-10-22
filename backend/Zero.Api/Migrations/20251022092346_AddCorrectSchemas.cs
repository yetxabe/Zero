using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zero.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCorrectSchemas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fields_FormSection_FormSectionId",
                schema: "Form",
                table: "Fields");

            migrationBuilder.DropForeignKey(
                name: "FK_FormSection_Forms_FormId",
                table: "FormSection");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FormSection",
                table: "FormSection");

            migrationBuilder.RenameTable(
                name: "FormSection",
                newName: "Sections",
                newSchema: "Form");

            migrationBuilder.RenameIndex(
                name: "IX_FormSection_FormId",
                schema: "Form",
                table: "Sections",
                newName: "IX_Sections_FormId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sections",
                schema: "Form",
                table: "Sections",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Fields_Sections_FormSectionId",
                schema: "Form",
                table: "Fields",
                column: "FormSectionId",
                principalSchema: "Form",
                principalTable: "Sections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sections_Forms_FormId",
                schema: "Form",
                table: "Sections",
                column: "FormId",
                principalSchema: "Form",
                principalTable: "Forms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fields_Sections_FormSectionId",
                schema: "Form",
                table: "Fields");

            migrationBuilder.DropForeignKey(
                name: "FK_Sections_Forms_FormId",
                schema: "Form",
                table: "Sections");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sections",
                schema: "Form",
                table: "Sections");

            migrationBuilder.RenameTable(
                name: "Sections",
                schema: "Form",
                newName: "FormSection");

            migrationBuilder.RenameIndex(
                name: "IX_Sections_FormId",
                table: "FormSection",
                newName: "IX_FormSection_FormId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FormSection",
                table: "FormSection",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Fields_FormSection_FormSectionId",
                schema: "Form",
                table: "Fields",
                column: "FormSectionId",
                principalTable: "FormSection",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FormSection_Forms_FormId",
                table: "FormSection",
                column: "FormId",
                principalSchema: "Form",
                principalTable: "Forms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
