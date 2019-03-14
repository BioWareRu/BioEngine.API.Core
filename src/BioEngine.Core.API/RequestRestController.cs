﻿using System;
using System.Linq;
using System.Threading.Tasks;
using BioEngine.Core.API.Models;
using BioEngine.Core.API.Response;
using BioEngine.Core.Entities;
using BioEngine.Core.Interfaces;
using BioEngine.Core.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BioEngine.Core.API
{
    public abstract class
        RequestRestController<TEntity, TResponse, TRequest> : ResponseRestController<TEntity,
            TResponse>
        where TEntity : class, IEntity
        where TResponse : class, IResponseRestModel<TEntity>
        where TRequest : class, IRequestRestModel<TEntity>
    {
        protected RequestRestController(BaseControllerContext<TEntity> context) : base(context)
        {
        }

        protected virtual async Task<TEntity> MapDomainModelAsync(TRequest restModel,
            TEntity domainModel = null)
        {
            return await restModel.GetEntityAsync(domainModel);
        }


        [HttpPost]
        public virtual async Task<ActionResult<TResponse>> AddAsync(TRequest item)
        {
            var entity = await MapDomainModelAsync(item, Activator.CreateInstance<TEntity>());

            var result = await Repository.AddAsync(entity);
            if (result.IsSuccess)
            {
                await AfterSaveAsync(item, result.Entity);
                return Created(await MapRestModelAsync(result.Entity));
            }

            return Errors(StatusCodes.Status422UnprocessableEntity,
                result.Errors.Select(e => new ValidationErrorResponse(e.PropertyName, e.ErrorMessage)));
        }

        [HttpPut("{id}")]
        public virtual async Task<ActionResult<TResponse>> UpdateAsync(Guid id, TRequest item)
        {
            var entity = await Repository.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            entity = await MapDomainModelAsync(item, entity);

            var result = await Repository.UpdateAsync(entity);
            if (result.IsSuccess)
            {
                await AfterSaveAsync(item, result.Entity);
                return Updated(await MapRestModelAsync(result.Entity));
            }

            return Errors(StatusCodes.Status422UnprocessableEntity,
                result.Errors.Select(e => new ValidationErrorResponse(e.PropertyName, e.ErrorMessage)));
        }

        [HttpPost("upload")]
        public virtual Task<ActionResult<StorageItem>> UploadAsync([FromQuery] string name)
        {
            throw new NotImplementedException();
        }

        //protected abstract TRest MapEntity(TRest entity, TRest newData);

        [HttpDelete("{id}")]
        public virtual async Task<ActionResult<TResponse>> DeleteAsync(Guid id)
        {
            var result = await Repository.DeleteAsync(id);
            if (result) return Deleted();
            return BadRequest();
        }

        [HttpPost("publish/{id}")]
        public virtual async Task<ActionResult<TResponse>> PublishAsync(Guid id)
        {
            var entity = await Repository.GetByIdAsync(id);
            if (entity != null)
            {
                if (entity.IsPublished)
                {
                    return BadRequest();
                }

                await Repository.PublishAsync(entity);
                return Model(await MapRestModelAsync(entity));
            }

            return NotFound();
        }

        [HttpPost("hide/{id}")]
        public virtual async Task<ActionResult<TResponse>> HideAsync(Guid id)
        {
            var entity = await Repository.GetByIdAsync(id);
            if (entity != null)
            {
                if (!entity.IsPublished)
                {
                    return BadRequest();
                }

                await Repository.UnPublishAsync(entity);
                return Model(await MapRestModelAsync(entity));
            }

            return NotFound();
        }


        protected ActionResult<TResponse> Created(
            TResponse model)
        {
            return SaveResponse(StatusCodes.Status201Created, model);
        }

        protected ActionResult<TResponse> Updated(
            TResponse model)
        {
            return SaveResponse(StatusCodes.Status202Accepted, model);
        }

        protected ActionResult<TResponse> Deleted()
        {
            return SaveResponse(StatusCodes.Status204NoContent, null);
        }


        private ActionResult<TResponse> SaveResponse(int code,
            TResponse model)
        {
            return new ObjectResult(new SaveModelResponse<TResponse>(code, model))
                {StatusCode = code};
        }

        protected virtual Task AfterSaveAsync(TRequest restModel, TEntity domainModel)
        {
            return Task.CompletedTask;
        }
    }

    public abstract class
        ResponseRequestRestController<TEntity, TRequestResponse> :
            RequestRestController<TEntity, TRequestResponse, TRequestResponse>
        where TEntity : class, IEntity
        where TRequestResponse : class, IResponseRestModel<TEntity>, IRequestRestModel<TEntity>
    {
        protected ResponseRequestRestController(BaseControllerContext<TEntity> context) : base(context)
        {
        }
    }
}
