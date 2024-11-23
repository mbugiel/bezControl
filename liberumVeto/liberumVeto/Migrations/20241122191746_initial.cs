using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace liberumVeto.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "question_set",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_question_set", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "question",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    setid = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_question", x => x.id);
                    table.ForeignKey(
                        name: "FK_question_question_set_setid",
                        column: x => x.setid,
                        principalTable: "question_set",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "voting_session",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    setid = table.Column<long>(type: "bigint", nullable: false),
                    votingDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_voting_session", x => x.id);
                    table.ForeignKey(
                        name: "FK_voting_session_question_set_setid",
                        column: x => x.setid,
                        principalTable: "question_set",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "question_statistic",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    votingSessionid = table.Column<long>(type: "bigint", nullable: false),
                    questionid = table.Column<long>(type: "bigint", nullable: false),
                    forQuantity = table.Column<int>(type: "integer", nullable: false),
                    againstQuantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_question_statistic", x => x.id);
                    table.ForeignKey(
                        name: "FK_question_statistic_question_questionid",
                        column: x => x.questionid,
                        principalTable: "question",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_question_statistic_voting_session_votingSessionid",
                        column: x => x.votingSessionid,
                        principalTable: "voting_session",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_question_setid",
                table: "question",
                column: "setid");

            migrationBuilder.CreateIndex(
                name: "IX_question_statistic_questionid",
                table: "question_statistic",
                column: "questionid");

            migrationBuilder.CreateIndex(
                name: "IX_question_statistic_votingSessionid",
                table: "question_statistic",
                column: "votingSessionid");

            migrationBuilder.CreateIndex(
                name: "IX_voting_session_setid",
                table: "voting_session",
                column: "setid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "question_statistic");

            migrationBuilder.DropTable(
                name: "question");

            migrationBuilder.DropTable(
                name: "voting_session");

            migrationBuilder.DropTable(
                name: "question_set");
        }
    }
}
