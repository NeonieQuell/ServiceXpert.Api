﻿using FluentBuilder.Core;
using ServiceXpert.Application.DataObjects;
using ServiceXpert.Domain.Entities;
using ServiceXpert.Domain.ValueObjects;

namespace ServiceXpert.Application.Services.Contracts;
public interface IServiceBase<TId, TEntity, TDataObject>
    where TEntity : EntityBase
    where TDataObject : DataObjectBase
{
    Task<TId> CreateAsync<TDataObjectForCreate>(TDataObjectForCreate dataObject)
        where TDataObjectForCreate : DataObjectBase;

    Task DeleteByIdAsync(TId id);

    Task<IEnumerable<TDataObject>> GetAllAsync(IncludeOptions<TEntity>? includeOptions = null);

    Task<TDataObject?> GetByIdAsync(TId id, IncludeOptions<TEntity>? includeOptions = null);

    Task<PagedResult<TDataObject>> GetPagedAllAsync(int pageNumber, int pageSize,
        IncludeOptions<TEntity>? includeOptions = null);

    Task<bool> IsExistsByIdAsync(TId id);

    Task UpdateByIdAsync<TDataObjectForUpdate>(TId id, TDataObjectForUpdate dataObject)
        where TDataObjectForUpdate : DataObjectBase;
}