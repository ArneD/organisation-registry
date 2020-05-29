namespace OrganisationRegistry.Api.Report.Responses
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;
    using Infrastructure;
    using Infrastructure.Search.Sorting;
    using Microsoft.EntityFrameworkCore;
    using OrganisationRegistry.Person;
    using SqlServer.Infrastructure;
    using SqlServer.Reporting;

    public class ParticipationSummary
    {
        [ExcludeFromCsv] public Guid OrganisationId { get; set; }
        [DisplayName("Entiteit")] public string OrganisationName { get; set; }

        [ExcludeFromCsv] public Guid BodyId { get; set; }
        [DisplayName("Orgaan")] public string BodyName { get; set; }

        [DisplayName("Percentage Man effectief")] public decimal EffectiveMalePercentage { get; set; }
        [DisplayName("Percentage Vrouw effectief")] public decimal EffectiveFemalePercentage { get; set; }
        [DisplayName("Percentage Onbekend effectief")] public decimal EffectiveUnknownPercentage { get; set; }
        [DisplayName("Percentage Man plaatsvervangend")] public decimal SubsidiaryMalePercentage { get; set; }
        [DisplayName("Percentage Vrouw plaatsvervangend")] public decimal SubsidiaryFemalePercentage { get; set; }
        [DisplayName("Percentage Onbekend plaatsvervangend")] public decimal SubsidiaryUnknownPercentage { get; set; }
        [DisplayName("Aantal Man effectief")] public int EffectiveMaleCount { get; set; }
        [DisplayName("Aantal Vrouw effectief")] public int EffectiveFemaleCount { get; set; }
        [DisplayName("Aantal Onbekend effectief")] public int EffectiveUnknownCount { get; set; }
        [DisplayName("Totaal Aantal effectief")] public int EffectiveTotalCount { get; set; }
        [DisplayName("Aantal Man plaatsvervangend")] public int SubsidiaryMaleCount { get; set; }
        [DisplayName("Aantal Vrouw plaatsvervangend")] public int SubsidiaryFemaleCount { get; set; }
        [DisplayName("Aantal Onbekend plaatsvervangend")] public int SubsidiaryUnknownCount { get; set; }
        [DisplayName("Totaal Aantal plaatsvervangend")] public int SubsidiaryTotalCount { get; set; }
        [DisplayName("Totaal Aantal")] public int TotalCount { get; set; }

        [ExcludeFromCsv] public int AssignedCount { get; set; }
        [ExcludeFromCsv] public int UnassignedCount { get; set; }

        [DisplayName("Totaal ok")] public bool IsTotalCompliant { get; set; }

        [DisplayName("Vrouw ok")] public bool IsSubsidiaryCompliant { get; set; }

        [DisplayName("Man ok")] public bool IsEffectiveCompliant { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="today"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<ParticipationSummary>> Search(
            OrganisationRegistryContext context,
            DateTime today)
        {
            var bodies = context.BodySeatGenderRatioBodyList
                .Include(item => item.PostsPerType)
                .Include(item => item.LifecyclePhaseValidities)
                .Where(body => body.LifecyclePhaseValidities.Any(y =>
                    y.RepresentsActivePhase &&
                    (!y.ValidFrom.HasValue || y.ValidFrom <= today) &&
                    (!y.ValidTo.HasValue || y.ValidFrom >= today)))
                .ToList();

            var activeSeatsPerType = bodies
                .SelectMany(x => x.PostsPerType)
                .Where(post =>
                    (!post.BodySeatValidFrom.HasValue || post.BodySeatValidFrom <= today) &&
                    (!post.BodySeatValidTo.HasValue || post.BodySeatValidTo >= today));

            var bodySeatGenderRatioPostsPerTypeItems = activeSeatsPerType.ToList();

            var activeSeatIds = bodySeatGenderRatioPostsPerTypeItems
                .Select(item => item.BodySeatId)
                .ToList();

            var activeSeatsPerBodyAndIsEffective = bodySeatGenderRatioPostsPerTypeItems
                .ToList()
                .GroupBy(mandate => new
                {
                    mandate.BodyId,
                    mandate.BodySeatTypeIsEffective,
                })
                .ToDictionary(
                    x => x.Key,
                    x => x);

            var activeMandates = context.BodySeatGenderRatioBodyMandateList
                .AsAsyncQueryable()
                .Where(mandate =>
                    (!mandate.BodyMandateValidFrom.HasValue || mandate.BodyMandateValidFrom <= today) &&
                    (!mandate.BodyMandateValidTo.HasValue || mandate.BodyMandateValidTo >= today))
                .Where(mandate => activeSeatIds.Contains(mandate.BodySeatId))
                .ToList();

            var activeMandateIds = activeMandates
                .Select(item => item.BodyMandateId)
                .ToList();

            var activeAssignmentsPerIsEffective =
                context.BodySeatGenderRatioBodyMandateList
                    .Include(mandate => mandate.Assignments)
                    .AsAsyncQueryable()
                    .Where(mandate =>
                        (!mandate.BodyMandateValidFrom.HasValue || mandate.BodyMandateValidFrom <= DateTime.Today) &&
                        (!mandate.BodyMandateValidTo.HasValue || mandate.BodyMandateValidTo >= DateTime.Today))
                    .Where(mandate => activeMandateIds.Contains(mandate.BodyMandateId))
                    .ToList()
                    .GroupBy(mandate => new
                    {
                        mandate.BodyId,
                        mandate.BodySeatTypeIsEffective,
                    })
                    .ToDictionary(
                        x => x.Key,
                        x => x.SelectMany(y => y.Assignments));

            var bodyIdsWithAssignments = activeAssignmentsPerIsEffective.Select(x => x.Key.BodyId);
            var bodiesWithAssignments = bodies.Where(x => bodyIdsWithAssignments.Contains(x.BodyId));

            return bodiesWithAssignments
                .Select(body =>
                {
                    var effectiveKey = new { BodyId = body.BodyId, BodySeatTypeIsEffective = true};
                    var subsidiaryKey = new { BodyId = body.BodyId, BodySeatTypeIsEffective = false};

                    var effectiveSeats =
                        activeSeatsPerBodyAndIsEffective.ContainsKey(effectiveKey)
                            ? activeSeatsPerBodyAndIsEffective[effectiveKey].ToList()
                            : new List<BodySeatGenderRatioPostsPerTypeItem>();

                    var subsidiarySeats =
                        activeSeatsPerBodyAndIsEffective.ContainsKey(subsidiaryKey)
                            ? activeSeatsPerBodyAndIsEffective[subsidiaryKey].ToList()
                            : new List<BodySeatGenderRatioPostsPerTypeItem>();

                    var effectiveAssignments =
                        activeAssignmentsPerIsEffective.ContainsKey(effectiveKey)
                            ? activeAssignmentsPerIsEffective[effectiveKey].ToList()
                            : new List<BodySeatGenderRatioAssignmentItem>();

                    var subsidiaryAssignments =
                        activeAssignmentsPerIsEffective.ContainsKey(subsidiaryKey)
                            ? activeAssignmentsPerIsEffective[subsidiaryKey].ToList()
                            : new List<BodySeatGenderRatioAssignmentItem>();

                    var effectiveTotalCount = effectiveAssignments.Count;
                    var subsidiaryTotalCount = subsidiaryAssignments.Count;
                    var assignedCount = effectiveTotalCount + subsidiaryTotalCount;

                    var totalCount = effectiveSeats.Count + subsidiarySeats.Count;

                    return new ParticipationSummary
                    {
                        BodyId = body.BodyId,
                        BodyName = body.BodyName ?? string.Empty,

                        OrganisationId = body.OrganisationId ?? Guid.Empty,
                        OrganisationName = body.OrganisationName ?? string.Empty,

                        EffectiveMaleCount = effectiveAssignments.Count(x => x.Sex == Sex.Male),
                        EffectiveFemaleCount = effectiveAssignments.Count(x => x.Sex == Sex.Female),
                        EffectiveUnknownCount = effectiveAssignments.Count(x => !x.Sex.HasValue),

                        SubsidiaryMaleCount = subsidiaryAssignments.Count(x => x.Sex == Sex.Male),
                        SubsidiaryFemaleCount = subsidiaryAssignments.Count(x => x.Sex == Sex.Female),
                        SubsidiaryUnknownCount = subsidiaryAssignments.Count(x => !x.Sex.HasValue),

                        EffectiveTotalCount = effectiveTotalCount,
                        SubsidiaryTotalCount = subsidiaryTotalCount,

                        AssignedCount = assignedCount,
                        UnassignedCount = totalCount - assignedCount,

                        TotalCount = totalCount
                    };
                })
                .Where(summary => summary.UnassignedCount == 0);
        }

        /// <summary>
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        public static IEnumerable<ParticipationSummary> Map(
            IEnumerable<ParticipationSummary> results)
        {
            var participations = new List<ParticipationSummary>();
            var lower = Math.Floor(1m / 3 * 100) / 100;
            var upper = Math.Ceiling(2m / 3 * 100) / 100;

            foreach (var result in results)
            {
                if (result.AssignedCount > 0)
                {
                    result.SubsidiaryMalePercentage = Math.Round((decimal) result.SubsidiaryMaleCount / result.AssignedCount, 2);
                    result.SubsidiaryFemalePercentage = Math.Round((decimal) result.SubsidiaryFemaleCount / result.AssignedCount, 2);
                    result.SubsidiaryUnknownPercentage = Math.Round((decimal) result.SubsidiaryUnknownCount / result.AssignedCount, 2);
                    result.EffectiveMalePercentage = Math.Round((decimal) result.EffectiveMaleCount / result.AssignedCount, 2);
                    result.EffectiveFemalePercentage = Math.Round((decimal) result.EffectiveFemaleCount / result.AssignedCount, 2);
                    result.EffectiveUnknownPercentage = Math.Round((decimal) result.EffectiveUnknownCount / result.AssignedCount, 2);

                    result.IsEffectiveCompliant =
                        result.EffectiveTotalCount <= 1 ||
                        result.EffectiveMalePercentage >= lower && result.EffectiveMalePercentage <= upper &&
                        result.EffectiveFemalePercentage >= lower && result.EffectiveFemalePercentage <= upper;

                    result.IsSubsidiaryCompliant =
                        result.SubsidiaryTotalCount <= 1 ||
                        result.SubsidiaryMalePercentage >= lower && result.SubsidiaryMalePercentage <= upper &&
                        result.SubsidiaryFemalePercentage >= lower && result.SubsidiaryFemalePercentage <= upper;

                    result.IsTotalCompliant = result.IsEffectiveCompliant && result.IsSubsidiaryCompliant;
                }

                participations.Add(result);
            }

            return participations;
        }

        /// <summary>
        /// </summary>
        /// <param name="results"></param>
        /// <param name="sortingHeader"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<ParticipationSummary> Sort(
            IEnumerable<ParticipationSummary> results,
            SortingHeader sortingHeader)
        {
            if (!sortingHeader.ShouldSort)
                return results.OrderBy(x => x.BodyName).ThenBy(x => x.BodyName);

            switch (sortingHeader.SortBy.ToLowerInvariant())
            {
                case "bodyname":
                    return sortingHeader.SortOrder == SortOrder.Ascending
                        ? results.OrderBy(x => x.BodyName)
                        : results.OrderByDescending(x => x.BodyName);
                case "istotalcompliant":
                    return sortingHeader.SortOrder == SortOrder.Ascending
                        ? results.OrderBy(x => x.IsTotalCompliant)
                        : results.OrderByDescending(x => x.IsTotalCompliant);
                default:
                    return results.OrderBy(x => x.BodyName).ThenBy(x => x.BodyName);
            }
        }
    }
}