using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zero.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddFormResponseTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Responses",
                schema: "Form",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FormId = table.Column<int>(type: "int", nullable: false),
                    Obra = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Responses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Responses_Forms_FormId",
                        column: x => x.FormId,
                        principalSchema: "Form",
                        principalTable: "Forms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResponseItems",
                schema: "Form",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FormResponseId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FormFieldId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResponseItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResponseItems_Fields_FormFieldId",
                        column: x => x.FormFieldId,
                        principalSchema: "Form",
                        principalTable: "Fields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResponseItems_Responses_FormResponseId",
                        column: x => x.FormResponseId,
                        principalSchema: "Form",
                        principalTable: "Responses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResponseItemOptions",
                schema: "Form",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormResponseItemId = table.Column<int>(type: "int", nullable: false),
                    FormFieldOptionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResponseItemOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResponseItemOptions_FieldOptions_FormFieldOptionId",
                        column: x => x.FormFieldOptionId,
                        principalSchema: "Form",
                        principalTable: "FieldOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResponseItemOptions_ResponseItems_FormResponseItemId",
                        column: x => x.FormResponseItemId,
                        principalSchema: "Form",
                        principalTable: "ResponseItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResponseItemOptions_FormFieldOptionId",
                schema: "Form",
                table: "ResponseItemOptions",
                column: "FormFieldOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_ResponseItemOptions_FormResponseItemId",
                schema: "Form",
                table: "ResponseItemOptions",
                column: "FormResponseItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ResponseItems_FormFieldId",
                schema: "Form",
                table: "ResponseItems",
                column: "FormFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_ResponseItems_FormResponseId",
                schema: "Form",
                table: "ResponseItems",
                column: "FormResponseId");

            migrationBuilder.CreateIndex(
                name: "IX_Responses_FormId",
                schema: "Form",
                table: "Responses",
                column: "FormId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResponseItemOptions",
                schema: "Form");

            migrationBuilder.DropTable(
                name: "ResponseItems",
                schema: "Form");

            migrationBuilder.DropTable(
                name: "Responses",
                schema: "Form");
        }
    }
}
