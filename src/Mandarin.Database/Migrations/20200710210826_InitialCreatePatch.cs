﻿// <auto-generated />

using System;
using System.CodeDom.Compiler;
using Microsoft.EntityFrameworkCore.Migrations;
#pragma warning disable 1413
#pragma warning disable 1591
#pragma warning disable 1600
#pragma warning disable 1601

namespace Mandarin.Database.Migrations
{
    [GeneratedCode("dotnet-ef", "3.1.5")]
    public partial class InitialCreatePatch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "rate",
                schema: "billing",
                table: "commission_rate_group",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "start_date",
                schema: "billing",
                table: "commission",
                nullable: false,
                defaultValueSql: "CURRENT_DATE",
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "inserted_at",
                schema: "billing",
                table: "commission",
                nullable: true,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "rate",
                schema: "billing",
                table: "commission_rate_group",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<DateTime>(
                name: "start_date",
                schema: "billing",
                table: "commission",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValueSql: "CURRENT_DATE");

            migrationBuilder.AlterColumn<DateTime>(
                name: "inserted_at",
                schema: "billing",
                table: "commission",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldNullable: true,
                oldDefaultValueSql: "CURRENT_TIMESTAMP");
        }
    }
}
