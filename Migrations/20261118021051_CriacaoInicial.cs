using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GuardaRio.Abrigos.Migrations
{
    /// <inheritdoc />
    public partial class CriacaoInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TB_GR_AREA",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NOME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    CIDADE = table.Column<string>(type: "NVARCHAR2(80)", maxLength: 80, nullable: false),
                    UF = table.Column<string>(type: "NVARCHAR2(2)", maxLength: 2, nullable: false),
                    POPULACAO_ESTIMADA = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GRN_AREA", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TB_GR_ABRIGO",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NOME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    ENDERECO = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    CAPACIDADE = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    OCUPACAO = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ACEITA_PETS = table.Column<int>(type: "NUMBER(1)", nullable: false),
                    ACESSIVEL_PCD = table.Column<int>(type: "NUMBER(1)", nullable: false),
                    STATUS = table.Column<string>(type: "NVARCHAR2(10)", maxLength: 10, nullable: false),
                    AREA_ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GRN_ABRIGO", x => x.ID);
                    table.CheckConstraint("CK_GRN_ABRIGO_OCUPACAO", "OCUPACAO BETWEEN 0 AND CAPACIDADE");
                    table.ForeignKey(
                        name: "FK_GRN_ABRIGO_AREA",
                        column: x => x.AREA_ID,
                        principalTable: "TB_GR_AREA",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TB_GR_NECESSIDADE",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ITEM = table.Column<string>(type: "NVARCHAR2(80)", maxLength: 80, nullable: false),
                    QTD_NECESSARIA = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    QTD_RECEBIDA = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    URGENTE = table.Column<int>(type: "NUMBER(1)", nullable: false),
                    ABRIGO_ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GRN_NECESSIDADE", x => x.ID);
                    table.ForeignKey(
                        name: "FK_GRN_NECESSIDADE_ABRIGO",
                        column: x => x.ABRIGO_ID,
                        principalTable: "TB_GR_ABRIGO",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TB_GR_ABRIGO_AREA_ID",
                table: "TB_GR_ABRIGO",
                column: "AREA_ID");

            migrationBuilder.CreateIndex(
                name: "IX_TB_GR_NECESSIDADE_ABRIGO_ID",
                table: "TB_GR_NECESSIDADE",
                column: "ABRIGO_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TB_GR_NECESSIDADE");

            migrationBuilder.DropTable(
                name: "TB_GR_ABRIGO");

            migrationBuilder.DropTable(
                name: "TB_GR_AREA");
        }
    }
}
