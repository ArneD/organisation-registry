namespace OrganisationRegistry.UnitTests.Organisation.CreateOrganisation
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Infrastructure.Tests.Extensions.TestHelpers;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Purpose;
    using Tests.Shared;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Organisation.Events;
    using Xunit;
    using Xunit.Abstractions;

    public class WithAnActiveValidity: Specification<Organisation, OrganisationCommandHandlers, CreateOrganisation>
    {
        protected override IEnumerable<IEvent> Given()
        {
            return new List<IEvent>();
        }

        protected override CreateOrganisation When()
        {
            return new CreateOrganisation(
                new OrganisationId(Guid.NewGuid()),
                "Test",
                "OVO0001234",
                "",
                null,
                "",
                new List<PurposeId>(),
                false,
                new ValidFrom(),
                new ValidTo());
        }

        protected override OrganisationCommandHandlers BuildHandler()
        {
            return new OrganisationCommandHandlers(
                new Mock<ILogger<OrganisationCommandHandlers>>().Object,
                Session,
                new SequentialOvoNumberGenerator(),
                new UniqueOvoNumberValidatorStub(false),
                new DateTimeProviderStub(DateTime.Today),
                Mock.Of<IOrganisationRegistryConfiguration>());
        }

        protected override int ExpectedNumberOfEvents => 2;

        [Fact]
        public void CreatesAnOrganisation()
        {
            var organisationCreated = PublishedEvents[0].UnwrapBody<OrganisationCreated>();
            organisationCreated.Should().NotBeNull();
        }

        [Fact]
        public void TheOrganisationBecomesActive()
        {
            var organisationBecameActive = PublishedEvents[1].UnwrapBody<OrganisationBecameActive>();;
            organisationBecameActive.Should().NotBeNull();
        }

        public WithAnActiveValidity(ITestOutputHelper helper) : base(helper) { }
    }
}
