﻿using FluentBuilder.Core;
using FluentBuilder.Persistence;
using Microsoft.EntityFrameworkCore;
using ServiceXpert.Domain.Entities;
using ServiceXpert.Domain.Repositories.Contracts;
using ServiceXpert.Domain.ValueObjects;
using ServiceXpert.ServiceXpert.Infrastructure.DbContexts;
using System.Linq.Expressions;

namespace ServiceXpert.Infrastructure.Repositories;
public abstract class RepositoryBase<TEntityId, TEntity> : IRepositoryBase<TEntityId, TEntity> where TEntity : EntityBase
{
    private readonly SxpDbContext dbContext;

    protected string EntityId { get => string.Concat(typeof(TEntity).Name, "Id"); }

    public RepositoryBase(SxpDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public void Attach(TEntity entity)
    {
        this.dbContext.Set<TEntity>().Attach(entity);
    }

    public async Task CreateAsync(TEntity entity)
    {
        await this.dbContext.Set<TEntity>().AddAsync(entity);
    }

    public async Task DeleteByIdAsync(TEntityId entityId)
    {
        await this.dbContext.Set<TEntity>()
            .Where(e => EF.Property<TEntityId>(e, this.EntityId)!.Equals(entityId)).ExecuteDeleteAsync();
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? condition = null,
        IncludeOptions<TEntity>? includeOptions = null)
    {
        IQueryable<TEntity> query = QueryBuilder.Build(this.dbContext.Set<TEntity>(), includeOptions);

        if (condition != null)
        {
            query = query.Where(condition);
        }

        return await query.ToListAsync();
    }

    public Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> condition, IncludeOptions<TEntity>? includeOptions = null)
    {
        IQueryable<TEntity> query = QueryBuilder.Build(this.dbContext.Set<TEntity>(), includeOptions);
        return query.Where(condition).SingleOrDefaultAsync();
    }

    public async Task<TEntity?> GetByIdAsync(TEntityId entityId, IncludeOptions<TEntity>? includeOptions = null)
    {
        IQueryable<TEntity> query = QueryBuilder.Build(this.dbContext.Set<TEntity>(), includeOptions);
        return await query.SingleOrDefaultAsync(e => EF.Property<TEntityId>(e, this.EntityId)!.Equals(entityId));
    }

    public async Task<(IEnumerable<TEntity>, Pagination)> GetPagedAllAsync(int pageNumber, int pageSize,
        Expression<Func<TEntity, bool>>? condition = null, IncludeOptions<TEntity>? includeOptions = null)
    {
        IQueryable<TEntity> selectQuery = QueryBuilder.Build(this.dbContext.Set<TEntity>(), includeOptions);
        IQueryable<TEntity> totalCountQuery = this.dbContext.Set<TEntity>();

        if (condition != null)
        {
            selectQuery = selectQuery.Where(condition);
            totalCountQuery = totalCountQuery.Where(condition);
        }

        var entities = await selectQuery
            .OrderBy(e => EF.Property<TEntityId>(e, this.EntityId))
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToListAsync();

        var metadata = new Pagination(await totalCountQuery.CountAsync(), pageSize, pageNumber);

        return (entities, metadata);
    }

    public async Task<bool> IsExistsByIdAsync(TEntityId entityId)
    {
        return await this.dbContext.Set<TEntity>()
            .AnyAsync(e => EF.Property<TEntityId>(e, this.EntityId)!.Equals(entityId));
    }

    public async Task<int> SaveChangesAsync()
    {
        return await this.dbContext.SaveChangesAsync();
    }
}
