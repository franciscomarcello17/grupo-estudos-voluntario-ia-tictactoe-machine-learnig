﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JogoDaVelhaIA.API.Migrations
{
    public partial class AddDataProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Data",
                table: "JogadasAprendidas",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Data",
                table: "JogadasAprendidas");
        }
    }
}
