﻿namespace OrganisationRegistry.SqlServer.Body
{
    using System;
    using System.Data.Common;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Infrastructure;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Body.Events;

    using System.Linq;
    using FormalFramework;
    using Microsoft.Extensions.Logging;
    using OrganisationRegistry.FormalFramework.Events;

    public class BodyFormalFrameworkListItem
    {
        public Guid BodyFormalFrameworkId { get; set; }
        public Guid BodyId { get; set; }

        public Guid FormalFrameworkId { get; set; }
        public string FormalFrameworkName { get; set; }

        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }

    public class BodyFormalFrameworkListConfiguration : EntityMappingConfiguration<BodyFormalFrameworkListItem>
    {
        public override void Map(EntityTypeBuilder<BodyFormalFrameworkListItem> b)
        {
            b.ToTable(nameof(BodyFormalFrameworkListView.ProjectionTables.BodyFormalFrameworkList), "OrganisationRegistry")
                .HasKey(p => p.BodyFormalFrameworkId)
                .ForSqlServerIsClustered(false);

            b.Property(p => p.BodyId).IsRequired();

            b.Property(p => p.FormalFrameworkId).IsRequired();
            b.Property(p => p.FormalFrameworkName).HasMaxLength(FormalFrameworkListConfiguration.NameLength).IsRequired();

            b.Property(p => p.ValidFrom);
            b.Property(p => p.ValidTo);

            b.HasIndex(x => x.FormalFrameworkName).ForSqlServerIsClustered();
            b.HasIndex(x => x.ValidFrom);
            b.HasIndex(x => x.ValidTo);
        }
    }

    public class BodyFormalFrameworkListView :
        Projection<BodyFormalFrameworkListView>,
        IEventHandler<BodyFormalFrameworkAdded>,
        IEventHandler<BodyFormalFrameworkUpdated>,
        IEventHandler<FormalFrameworkUpdated>
    {
        public override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));

        public enum ProjectionTables
        {
            BodyFormalFrameworkList
        }

        private readonly IEventStore _eventStore;

        public BodyFormalFrameworkListView(
            ILogger<BodyFormalFrameworkListView> logger,
            IEventStore eventStore) : base(logger)
        {
            _eventStore = eventStore;
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<FormalFrameworkUpdated> message)
        {
            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                var bodyFormalFrameworks = context.BodyFormalFrameworkList.Where(x => x.FormalFrameworkId == message.Body.FormalFrameworkId);
                if (!bodyFormalFrameworks.Any())
                    return;

                foreach (var bodyFormalFramework in bodyFormalFrameworks)
                    bodyFormalFramework.FormalFrameworkName = message.Body.Name;

                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyFormalFrameworkAdded> message)
        {
            var bodyFormalFrameworkListItem = new BodyFormalFrameworkListItem
            {
                BodyFormalFrameworkId = message.Body.BodyFormalFrameworkId,
                BodyId = message.Body.BodyId,
                FormalFrameworkId = message.Body.FormalFrameworkId,
                FormalFrameworkName = message.Body.FormalFrameworkName,
                ValidFrom = message.Body.ValidFrom,
                ValidTo = message.Body.ValidTo
            };

            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                context.BodyFormalFrameworkList.Add(bodyFormalFrameworkListItem);
                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyFormalFrameworkUpdated> message)
        {
            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                var bodyFormalFramework = context.BodyFormalFrameworkList.SingleOrDefault(item => item.BodyFormalFrameworkId == message.Body.BodyFormalFrameworkId);

                bodyFormalFramework.BodyFormalFrameworkId = message.Body.BodyFormalFrameworkId;
                bodyFormalFramework.BodyId = message.Body.BodyId;
                bodyFormalFramework.FormalFrameworkId = message.Body.FormalFrameworkId;
                bodyFormalFramework.FormalFrameworkName = message.Body.FormalFrameworkName;
                bodyFormalFramework.ValidFrom = message.Body.ValidFrom;
                bodyFormalFramework.ValidTo = message.Body.ValidTo;

                context.SaveChanges();
            }
        }

        public override void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }
    }
}