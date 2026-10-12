using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Simone.Common.StatusTransitions.Web.Test.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusTransitions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Transitions");

            migrationBuilder.CreateTable(
                name: "Notification_Type",
                schema: "Transitions",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Is_Active = table.Column<bool>(type: "bit", nullable: false),
                    RequiredPermissions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SendToAuthor = table.Column<bool>(type: "bit", nullable: false),
                    Channels = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification_Type", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Process",
                schema: "Transitions",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Process", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Status",
                schema: "Transitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Background_Color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Text_Color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Position_X = table.Column<int>(type: "int", nullable: false),
                    Position_Y = table.Column<int>(type: "int", nullable: false),
                    Is_Initial_Status = table.Column<bool>(type: "bit", nullable: false),
                    Process_Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Is_Active = table.Column<bool>(type: "bit", nullable: false),
                    Entity_Is_Editable = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Status", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Status_Process_Process_Name",
                        column: x => x.Process_Name,
                        principalSchema: "Transitions",
                        principalTable: "Process",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Status_Relation",
                schema: "Transitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Origin_Status_Id = table.Column<int>(type: "int", nullable: true),
                    Next_Status_Id = table.Column<int>(type: "int", nullable: true),
                    Is_Active = table.Column<bool>(type: "bit", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Comment_Is_Required = table.Column<bool>(type: "bit", nullable: false),
                    Require_All_Permissions = table.Column<bool>(type: "bit", nullable: false),
                    Notification_Type_Name = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Is_Visible = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Status_Relation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Status_Relation_Notification_Type_Notification_Type_Name",
                        column: x => x.Notification_Type_Name,
                        principalSchema: "Transitions",
                        principalTable: "Notification_Type",
                        principalColumn: "Name");
                    table.ForeignKey(
                        name: "FK_Status_Relation_Status_Next_Status_Id",
                        column: x => x.Next_Status_Id,
                        principalSchema: "Transitions",
                        principalTable: "Status",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Status_Relation_Status_Origin_Status_Id",
                        column: x => x.Origin_Status_Id,
                        principalSchema: "Transitions",
                        principalTable: "Status",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Transition",
                schema: "Transitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Origin_Status_Id = table.Column<int>(type: "int", nullable: true),
                    Next_Status_Id = table.Column<int>(type: "int", nullable: true),
                    Creation_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Author_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Process_Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Serialized_Entity_Id = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transition_Process_Process_Name",
                        column: x => x.Process_Name,
                        principalSchema: "Transitions",
                        principalTable: "Process",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transition_Status_Next_Status_Id",
                        column: x => x.Next_Status_Id,
                        principalSchema: "Transitions",
                        principalTable: "Status",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Transition_Status_Origin_Status_Id",
                        column: x => x.Origin_Status_Id,
                        principalSchema: "Transitions",
                        principalTable: "Status",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Status_Relation_Permission",
                schema: "Transitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status_Relation_Id = table.Column<int>(type: "int", nullable: false),
                    Permission_Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Status_Relation_Permission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Status_Relation_Permission_Status_Relation_Status_Relation_Id",
                        column: x => x.Status_Relation_Id,
                        principalSchema: "Transitions",
                        principalTable: "Status_Relation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Status_Process_Name",
                schema: "Transitions",
                table: "Status",
                column: "Process_Name");

            migrationBuilder.CreateIndex(
                name: "IX_Status_Relation_Next_Status_Id",
                schema: "Transitions",
                table: "Status_Relation",
                column: "Next_Status_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Status_Relation_Notification_Type_Name",
                schema: "Transitions",
                table: "Status_Relation",
                column: "Notification_Type_Name");

            migrationBuilder.CreateIndex(
                name: "IX_Status_Relation_Origin_Status_Id",
                schema: "Transitions",
                table: "Status_Relation",
                column: "Origin_Status_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Status_Relation_Permission_Status_Relation_Id",
                schema: "Transitions",
                table: "Status_Relation_Permission",
                column: "Status_Relation_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Transition_Next_Status_Id",
                schema: "Transitions",
                table: "Transition",
                column: "Next_Status_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Transition_Origin_Status_Id",
                schema: "Transitions",
                table: "Transition",
                column: "Origin_Status_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Transition_Process_Name",
                schema: "Transitions",
                table: "Transition",
                column: "Process_Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Status_Relation_Permission",
                schema: "Transitions");

            migrationBuilder.DropTable(
                name: "Transition",
                schema: "Transitions");

            migrationBuilder.DropTable(
                name: "Status_Relation",
                schema: "Transitions");

            migrationBuilder.DropTable(
                name: "Notification_Type",
                schema: "Transitions");

            migrationBuilder.DropTable(
                name: "Status",
                schema: "Transitions");

            migrationBuilder.DropTable(
                name: "Process",
                schema: "Transitions");
        }
    }
}
