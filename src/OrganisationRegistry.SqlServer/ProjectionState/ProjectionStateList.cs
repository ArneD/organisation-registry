﻿namespace OrganisationRegistry.SqlServer.ProjectionState
{
    using System;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class ProjectionStateItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int EventNumber { get; set; }
    }

    public class ProjectionStateListConfiguration : EntityMappingConfiguration<ProjectionStateItem>
    {
        public const int NameLength = 500;

        public override void Map(EntityTypeBuilder<ProjectionStateItem> b)
        {
            b.ToTable("ProjectionStateList", "OrganisationRegistry")
                .HasKey(p => p.Id)
                .IsClustered(false);

            b.Property(p => p.Name).IsRequired();
            b.Property(p => p.EventNumber).IsRequired();

            b.HasIndex(x => x.Name).IsUnique().IsClustered();
        }
    }
}
