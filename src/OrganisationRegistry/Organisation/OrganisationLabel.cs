﻿namespace OrganisationRegistry.Organisation
{
    using System;

    public class OrganisationLabel
    {
        public Guid OrganisationId { get; } // todo: remove organisationId from this (but not from event, possibly not from command)
        public Guid OrganisationLabelId { get; }
        public Guid LabelTypeId { get; }
        public string LabelTypeName { get; }
        public string Value { get; }
        public Period Validity { get; }

        public OrganisationLabel(
            Guid organisationLabelId,
            Guid organisationId,
            Guid labelTypeId,
            string labelTypeName,
            string value,
            Period validity)
        {
            OrganisationId = organisationId;
            OrganisationLabelId = organisationLabelId;
            LabelTypeId = labelTypeId;
            LabelTypeName = labelTypeName;
            Value = value;
            Validity = validity;
        }
    }
}
