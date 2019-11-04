﻿namespace OrganisationRegistry
{
    using System;
    using Infrastructure.Commands;
    using Infrastructure.Messages;

    public class BaseCommand : ICommand
    {
        protected Guid Id { get; set; }

        public int ExpectedVersion { get; set; }

        Guid IMessage.Id
        {
            get => Id;
            set => Id = value;
        }
    }

    public class BaseCommand<T> : BaseCommand where T : GenericId<T>
    {
        protected new T Id { get; set; }
    }
}
