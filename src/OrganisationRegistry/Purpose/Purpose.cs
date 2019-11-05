namespace OrganisationRegistry.Purpose
{
    using Events;
    using Infrastructure.Domain;

    public class Purpose : AggregateRoot
    {
        public PurposeName Name { get; private set; }

        private Purpose() { }

        public Purpose(PurposeId id, PurposeName name)
        {
            ApplyChange(new PurposeCreated(
                id,
                name));
        }

        public void Update(PurposeName name)
        {
            ApplyChange(new PurposeUpdated(
                Id,
                name,
                Name));
        }

        private void Apply(PurposeCreated @event)
        {
            Id = @event.PurposeId;
            Name = new PurposeName(@event.Name);
        }

        private void Apply(PurposeUpdated @event)
        {
            Name = new PurposeName(@event.Name);
        }
    }
}
