﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddPersonFunctionList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PersonFunctionList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    OrganisationFunctionId = table.Column<Guid>(nullable: false),
                    FunctionId = table.Column<Guid>(nullable: false),
                    FunctionName = table.Column<string>(maxLength: 500, nullable: true),
                    OrganisationId = table.Column<Guid>(nullable: false),
                    OrganisationName = table.Column<string>(maxLength: 2000, nullable: true),
                    PersonId = table.Column<Guid>(nullable: false),
                    PersonName = table.Column<string>(maxLength: 500, nullable: true),
                    ValidFrom = table.Column<DateTime>(nullable: true),
                    ValidTo = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonFunctionList", x => x.OrganisationFunctionId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PersonFunctionList",
                schema: "OrganisationRegistry");
        }
    }
}
