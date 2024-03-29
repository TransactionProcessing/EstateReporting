﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EstateReporting.Database.Migrations.MySql
{
    public partial class addtxnidtofileline : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TransactionId",
                table: "fileline",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "fileline");
        }
    }
}
