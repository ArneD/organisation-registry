// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace OrganisationRegistry.Import.Piavo.Models
{
    using System.Linq;

    public partial class UpdateOrganisationRelationRequest
    {
        /// <summary>
        /// Initializes a new instance of the
        /// UpdateOrganisationRelationRequest class.
        /// </summary>
        public UpdateOrganisationRelationRequest() { }

        /// <summary>
        /// Initializes a new instance of the
        /// UpdateOrganisationRelationRequest class.
        /// </summary>
        public UpdateOrganisationRelationRequest(System.Guid? organisationRelationId = default(System.Guid?), System.Guid? relationId = default(System.Guid?), System.Guid? relatedOrganisationId = default(System.Guid?), System.DateTime? validFrom = default(System.DateTime?), System.DateTime? validTo = default(System.DateTime?))
        {
            OrganisationRelationId = organisationRelationId;
            RelationId = relationId;
            RelatedOrganisationId = relatedOrganisationId;
            ValidFrom = validFrom;
            ValidTo = validTo;
        }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "organisationRelationId")]
        public System.Guid? OrganisationRelationId { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "relationId")]
        public System.Guid? RelationId { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "relatedOrganisationId")]
        public System.Guid? RelatedOrganisationId { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "validFrom")]
        public System.DateTime? ValidFrom { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "validTo")]
        public System.DateTime? ValidTo { get; set; }

    }
}