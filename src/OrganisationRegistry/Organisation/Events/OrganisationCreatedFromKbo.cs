namespace OrganisationRegistry.Organisation.Events
{
    using System;
    using System.Collections.Generic;

    public class OrganisationCreatedFromKbo : BaseEvent<OrganisationCreatedFromKbo>
    {
        public Guid OrganisationId => Id;

        public string Name { get; }
        public string OvoNumber { get; }
        public string ShortName { get; }
        public string Description { get; }
        public List<Purpose> Purposes { get; }
        public bool ShowOnVlaamseOverheidSites { get; }
        public DateTime? ValidFrom { get; }
        public DateTime? ValidTo { get; }
        public string KboNumber { get; }

        public OrganisationCreatedFromKbo(
            Guid organisationId,
            string kboNumber,
            string name,
            string ovoNumber,
            string shortName,
            string description,
            List<Purpose> purposes,
            bool showOnVlaamseOverheidSites,
            DateTime? validFrom,
            DateTime? validTo)
        {
            Id = organisationId;

            KboNumber = kboNumber;
            Name = name;
            OvoNumber = ovoNumber;
            ShortName = shortName;
            Description = description;
            Purposes = purposes;
            ShowOnVlaamseOverheidSites = showOnVlaamseOverheidSites;
            ValidFrom = validFrom;
            ValidTo = validTo;
        }
    }
}
