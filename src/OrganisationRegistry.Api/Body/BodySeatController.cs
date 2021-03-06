﻿namespace OrganisationRegistry.Api.Body
{
    using Microsoft.AspNetCore.Mvc;
    using Requests;
    using System.Threading.Tasks;
    using Infrastructure;
    using OrganisationRegistry.Infrastructure.Commands;
    using System;
    using Microsoft.EntityFrameworkCore;
    using Infrastructure.Search.Sorting;
    using Infrastructure.Search.Pagination;
    using SqlServer.Body;
    using Infrastructure.Search.Filtering;
    using Queries;
    using System.Net;
    using Infrastructure.Security;
    using Security;
    using SqlServer.Infrastructure;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("bodies/{bodyId}/seats")]
    public class BodySeatController : OrganisationRegistryController
    {
        public BodySeatController(ICommandSender commandSender)
            : base(commandSender)
        {
        }

        /// <summary>Get a list of available seats for a body.</summary>
        [HttpGet]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid bodyId)
        {
            var filtering = Request.ExtractFilteringRequest<BodySeatListItemFilter>();
            var sorting = Request.ExtractSortingRequest();
            var pagination = Request.ExtractPaginationRequest();

            var pagedBodySeats = new BodySeatListQuery(context, bodyId).Fetch(filtering, sorting, pagination);

            Response.AddPaginationResponse(pagedBodySeats.PaginationInfo);
            Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

            return Ok(await pagedBodySeats.Items.ToListAsync());
        }

        /// <summary>Get a seat for a body.</summary>
        /// <response code="200">If the seat is found.</response>
        /// <response code="404">If the seat cannot be found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BodySeatListItem), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid bodyId, [FromRoute] Guid id)
        {
            var bodySeat = await context.BodySeatList.FirstOrDefaultAsync(x => x.BodySeatId == id);

            if (bodySeat == null)
                return NotFound();

            return Ok(bodySeat);
        }

        /// <summary>Create a seat for a body.</summary>
        /// <response code="201">If the seat is created, together with the location.</response>
        /// <response code="400">If the seat information does not pass validation.</response>
        [HttpPost]
        [OrganisationRegistryAuthorize]
        [ProducesResponseType(typeof(CreatedResult), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Post([FromServices] ISecurityService securityService, [FromRoute] Guid bodyId, [FromBody] AddBodySeatRequest message)
        {
            var internalMessage = new AddBodySeatInternalRequest(bodyId, message);

            if (!securityService.CanEditBody(User, internalMessage.BodyId))
                ModelState.AddModelError("NotAllowed", "U hebt niet voldoende rechten voor dit orgaan.");

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            await CommandSender.Send(AddBodySeatRequestMapping.Map(internalMessage));

            return Created(Url.Action(nameof(Get), new { id = message.BodySeatId }), null);
        }

        /// <summary>Update a seat for a body.</summary>
        /// <response code="201">If the seat is updated, together with the location.</response>
        /// <response code="400">If the seat information does not pass validation.</response>
        [HttpPut("{id}")]
        [OrganisationRegistryAuthorize]
        [ProducesResponseType(typeof(OkResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Put([FromServices] ISecurityService securityService, [FromRoute] Guid bodyId, [FromBody] UpdateBodySeatRequest message)
        {
            var internalMessage = new UpdateBodySeatInternalRequest(bodyId, message);

            if (!securityService.CanEditBody(User, internalMessage.BodyId))
                ModelState.AddModelError("NotAllowed", "U hebt niet voldoende rechten voor dit orgaan.");

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            await CommandSender.Send(UpdateBodySeatRequestMapping.Map(internalMessage));

            return OkWithLocation(Url.Action(nameof(Get), new { id = internalMessage.BodyId }));
        }
    }
}
