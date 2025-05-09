using AutoMapper;
using BookStore.Application.AutoMapping;
using BookStore.Application.Interfaces;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using BookStore.Infrastructure.EFCore.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using System.Reflection;

namespace BookStore.Application.Services
{
    public class CrudManager<TEntity, TDto, TCreateDto, TUpdateDto> : ICrudService<TEntity, TDto, TCreateDto, TUpdateDto>
    where TEntity : Entity
    {
        protected readonly IRepository<TEntity> Repository;
        protected readonly IMapper Mapper;
        protected readonly IValidator<TCreateDto> CreateValidator;
        protected readonly IValidator<TUpdateDto> UpdateValidator;

        public CrudManager()
        {
            Repository = new EfCoreRepository<TEntity>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();

            });

            Mapper = config.CreateMapper();

            CreateValidator = InstantiateValidator<TCreateDto>();
            UpdateValidator = InstantiateValidator<TUpdateDto>();
        }

        private IValidator<T>? InstantiateValidator<T>()
        {
            var validatorType = Assembly.GetExecutingAssembly().GetTypes()
              .FirstOrDefault(t => typeof(IValidator<T>).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            if (validatorType == null)
                throw new InvalidOperationException($"Validator for {typeof(T).Name} not found");

            return (IValidator<T>)Activator.CreateInstance(validatorType)!;
        }

        public virtual TDto Add(TCreateDto createDto)
        {
            var validationResult = CreateValidator.Validate(createDto);

            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new ValidationException($"Validation failed: {errors}");
            }

            var entity = Mapper.Map<TEntity>(createDto);
            var addedEntity = Repository.Add(entity);

            return Mapper.Map<TDto>(addedEntity);
        }

        public TDto Delete(int id)
        {
            var exist = Repository.GetById(id);

            if (exist == null)
                throw new InvalidOperationException("Entity not found");

            var deletedEntity = Repository.Delete(exist);

            return Mapper.Map<TDto>(deletedEntity);
        }

        public TDto Get(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null)
        {
            var entity = Repository.Get(predicate, asNoTracking, include);

            if (entity == null)
                throw new InvalidOperationException("Entity not found");

            return Mapper.Map<TDto>(entity);
        }

        public List<TDto> GetAll(Expression<Func<TEntity, bool>>? predicate = null, bool asNoTracking = false, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null)
        {
            var entities = Repository.GetAll(predicate, asNoTracking, include, orderBy);

            if (entities == null || !entities.Any())
                throw new InvalidOperationException("No entities found");

            return Mapper.Map<List<TDto>>(entities);
        }

        public TDto GetById(int id)
        {
            var entity = Repository.GetById(id);

            if (entity == null)
                throw new InvalidOperationException("Entity not found");

            return Mapper.Map<TDto>(entity);
        }

        public virtual TDto Update(TUpdateDto updateDto)
        {
            var validationResult = UpdateValidator.Validate(updateDto);

            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new ValidationException($"Validation failed: {errors}");
            }

            var updatedEntity = Mapper.Map<TEntity>(updateDto);

            var existingEntity = Repository.GetById(updatedEntity.Id);

            if (existingEntity == null)
                throw new InvalidOperationException("Entity not found");

            return Mapper.Map<TDto>(Repository.Update(updatedEntity));
        }
    }
}