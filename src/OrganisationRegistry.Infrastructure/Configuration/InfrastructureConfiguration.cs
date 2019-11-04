﻿namespace OrganisationRegistry.Infrastructure.Configuration
{
    using System;
    using Infrastructure.Json;
    using Newtonsoft.Json;

    public class InfrastructureConfiguration
    {
        public static string Section = "Infrastructure";

        [JsonConverter(typeof(TimestampConverter))]
        public DateTime Created => DateTime.Now;

        public string EventStoreConnectionString { get; set; }
        public string EventStoreAdministrationConnectionString { get; set; }

        public InfrastructureConfiguration Obfuscate()
        {
            return new InfrastructureConfiguration
            {
                EventStoreConnectionString = Obfuscator.ObfuscateConnectionString(EventStoreConnectionString),
                EventStoreAdministrationConnectionString = Obfuscator.ObfuscateConnectionString(EventStoreAdministrationConnectionString),
            };
        }
    }
}
