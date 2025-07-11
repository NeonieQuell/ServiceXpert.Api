﻿using FluentBuilder.Core;
using Mapster;
using MapsterMapper;
using ServiceXpert.Application.DataObjects.Issue;
using ServiceXpert.Application.Services.Contracts;
using ServiceXpert.Domain.Entities;
using ServiceXpert.Domain.Repositories.Contracts;
using ServiceXpert.Domain.ValueObjects;
using DomainEnums = ServiceXpert.Domain.Shared.Enums;

namespace ServiceXpert.Application.Services;
public class IssueService : ServiceBase<int, Issue, IssueDataObject>, IIssueService
{
    private readonly IIssueRepository issueRepository;

    public IssueService(IIssueRepository issueRepository, IMapper mapper) : base(issueRepository, mapper)
    {
        this.issueRepository = issueRepository;
    }

    public async Task<PagedResult<IssueDataObject>> GetPagedIssuesByStatusAsync(
        string statusCategory, int pageNumber, int pageSize,
        IncludeOptions<Issue>? includeOptions = null)
    {
        var pagedResult = new PagedResult<Issue>();

        if (Enum.TryParse(statusCategory, ignoreCase: true, out IssueStatusCategory statusCategoryEnum))
        {
            switch (statusCategoryEnum)
            {
                case IssueStatusCategory.All:
                    pagedResult = await this.issueRepository.GetPagedAllAsync(
                        pageNumber, pageSize, includeOptions: includeOptions);
                    break;
                case IssueStatusCategory.Open:
                    pagedResult = await this.issueRepository.GetPagedAllAsync(
                        pageNumber, pageSize, i => i.IssueStatusId != (int)DomainEnums.IssueStatus.Resolved
                            && i.IssueStatusId != (int)DomainEnums.IssueStatus.Closed, includeOptions);
                    break;
                case IssueStatusCategory.Resolved:
                    pagedResult = await this.issueRepository.GetPagedAllAsync(pageNumber, pageSize,
                        i => i.IssueStatusId == (int)DomainEnums.IssueStatus.Resolved, includeOptions);
                    break;
                case IssueStatusCategory.Closed:
                    pagedResult = await this.issueRepository.GetPagedAllAsync(pageNumber, pageSize,
                        i => i.IssueStatusId == (int)DomainEnums.IssueStatus.Closed, includeOptions);
                    break;
            }

            // Use ICollection instead of IEnumerable to materialize object (required for Mapster)
            return new PagedResult<IssueDataObject>(
                pagedResult.Items.Adapt<List<IssueDataObject>>(),
                pagedResult.Pagination
            );
        }

        throw new InvalidCastException($"Cannot cast string into enum. Value: {statusCategory}");
    }
}
