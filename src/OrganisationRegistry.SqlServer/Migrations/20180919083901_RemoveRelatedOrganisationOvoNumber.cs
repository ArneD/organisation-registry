﻿using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class RemoveRelatedOrganisationOvoNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RelatedOrganisationOvoNumber",
                schema: "OrganisationRegistry",
                table: "OrganisationRelationList");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RelatedOrganisationOvoNumber",
                schema: "OrganisationRegistry",
                table: "OrganisationRelationList",
                maxLength: 10,
                nullable: false,
                defaultValue: "");
        }
    }
}
