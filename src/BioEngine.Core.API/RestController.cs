using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BioEngine.Core.API.Entities;
using BioEngine.Core.API.Interfaces;
using BioEngine.Core.API.Request;
using BioEngine.Core.API.Response;
using BioEngine.Core.DB;
using BioEngine.Core.Interfaces;
using BioEngine.Core.Storage;
using BioEngine.Core.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BioEngine.Core.API
{
    [ApiController]
    [Authorize]
    [Route("v1/[controller]")]
    public abstract class RestController<TRestModel, TEntity, TEntityPk> : BaseController
        where TEntity : class, IEntity<TEntityPk> where TRestModel : RestModel<TEntityPk>
    {
        protected virtual Task<TRestModel> MapRestModel(TEntity domainModel)
        {
            var restModel = Activator.CreateInstance<TRestModel>();
            restModel.Id = domainModel.Id;
            restModel.DateAdded = domainModel.DateAdded;
            restModel.DateUpdated = domainModel.DateUpdated;
            restModel.IsPublished = domainModel.IsPublished;
            restModel.DatePublished = domainModel.DatePublished;
            restModel.SettingsGroups =
                domainModel.Settings.Select(SettingsGroup.Create).ToList();
            return Task.FromResult(restModel);
        }

        protected virtual Task<TEntity> MapDomainModel(TRestModel restModel, TEntity domainModel = null)
        {
            domainModel = domainModel ?? Activator.CreateInstance<TEntity>();
            domainModel.Settings = restModel.SettingsGroups?.Select(s => s.GetSettings()).ToList();
            return Task.FromResult(domainModel);
        }

        [HttpGet]
        public virtual async Task<ActionResult<ListResponse<TRestModel>>> Get()
        {
            var result = await Repository.GetAll(GetQueryContext());
            return await List(result);
        }

        [HttpGet("{id}")]
        public virtual async Task<ActionResult<TRestModel>> Get(TEntityPk id)
        {
            return await MapRestModel(await Repository.GetById(id));
        }

        [HttpGet("new")]
        public virtual async Task<ActionResult<TRestModel>> New()
        {
            return await MapRestModel(await Repository.New());
        }

        [HttpPost("publish/{id}")]
        public virtual async Task<ActionResult<TRestModel>> Publish(TEntityPk id)
        {
            var entity = await Repository.GetById(id);
            if (entity != null)
            {
                if (entity.IsPublished)
                {
                    return BadRequest();
                }

                await Repository.Publish(entity);
                return await MapRestModel(entity);
            }

            return NotFound();
        }

        [HttpPost("hide/{id}")]
        public virtual async Task<ActionResult<TRestModel>> Hide(TEntityPk id)
        {
            var entity = await Repository.GetById(id);
            if (entity != null)
            {
                if (!entity.IsPublished)
                {
                    return BadRequest();
                }

                await Repository.UnPublish(entity);
                return await MapRestModel(entity);
            }

            return NotFound();
        }

        [HttpPost]
        public virtual async Task<ActionResult<TRestModel>> Add(TRestModel item)
        {
            var entity = await MapDomainModel(item, Activator.CreateInstance<TEntity>());

            var result = await Repository.Add(entity);
            if (result.IsSuccess)
            {
                await AfterSave(item, result.Entity);
                return Created(await MapRestModel(result.Entity));
            }

            return Errors(StatusCodes.Status422UnprocessableEntity,
                result.Errors.Select(e => new ValidationErrorResponse(e.PropertyName, e.ErrorMessage)));
        }

        [HttpPut("{id}")]
        public virtual async Task<ActionResult<TRestModel>> Update(TEntityPk id,
            TRestModel item)
        {
            var entity = await Repository.GetById(id);
            if (entity == null)
            {
                return NotFound();
            }

            entity = await MapDomainModel(item, entity);

            var result = await Repository.Update(entity);
            if (result.IsSuccess)
            {
                await AfterSave(item, result.Entity);
                return Updated(await MapRestModel(result.Entity));
            }

            return Errors(StatusCodes.Status422UnprocessableEntity,
                result.Errors.Select(e => new ValidationErrorResponse(e.PropertyName, e.ErrorMessage)));
        }

        [HttpPost("upload")]
        public virtual Task<ActionResult<StorageItem>> Upload([FromQuery] string name)
        {
            throw new NotImplementedException();
        }

        //protected abstract TRest MapEntity(TRest entity, TRest newData);

        [HttpDelete("{id}")]
        public virtual async Task<ActionResult<TRestModel>> Delete(TEntityPk id)
        {
            var result = await Repository.Delete(id);
            if (result) return Deleted();
            return BadRequest();
        }

        protected QueryContext<TEntity, TEntityPk> GetQueryContext()
        {
            var context = new QueryContext<TEntity, TEntityPk> {IncludeUnpublished = true};
            if (ControllerContext.HttpContext.Request.Query.ContainsKey("limit"))
            {
                context.Limit = int.Parse(ControllerContext.HttpContext.Request.Query["limit"]);
            }

            if (ControllerContext.HttpContext.Request.Query.ContainsKey("offset"))
            {
                context.Offset = int.Parse(ControllerContext.HttpContext.Request.Query["offset"]);
            }

            if (ControllerContext.HttpContext.Request.Query.ContainsKey("order"))
            {
                context.SetOrderByString(ControllerContext.HttpContext.Request.Query["order"]);
            }

            return context;
        }


        protected new ActionResult<TRestModel> NotFound()
        {
            return Errors(StatusCodes.Status404NotFound, new[] {new RestErrorResponse("Not Found")});
        }

        protected ActionResult<TRestModel> Created(TRestModel model)
        {
            return SaveResponse(StatusCodes.Status201Created, model);
        }

        protected ActionResult<TRestModel> Updated(TRestModel model)
        {
            return SaveResponse(StatusCodes.Status202Accepted, model);
        }

        protected ActionResult<TRestModel> Deleted()
        {
            return SaveResponse(StatusCodes.Status204NoContent, null);
        }

        protected ActionResult<TRestModel> Errors(int code, IEnumerable<IErrorInterface> errors)
        {
            return new ObjectResult(new RestResponse(code, errors)) {StatusCode = code};
        }

        private ActionResult<TRestModel> SaveResponse(int code, TRestModel model)
        {
            return new ObjectResult(new SaveModelResponse<TRestModel>(code, model))
                {StatusCode = code};
        }

        protected async Task<ActionResult<ListResponse<TRestModel>>> List(
            (IEnumerable<TEntity> items, int itemsCount) result)
        {
            var restModels = new List<TRestModel>();
            foreach (var item in result.items)
            {
                restModels.Add(await MapRestModel(item));
            }

            return Ok(new ListResponse<TRestModel>(restModels, result.itemsCount));
        }

        protected ActionResult<TRestModel> Model(TRestModel model)
        {
            if (model == null)
            {
                return NotFound();
            }

            return Ok(model);
        }

        protected RestController(BaseControllerContext<TEntity, TEntityPk> context) : base(context)
        {
            Repository = context.Repository;
        }

        protected IBioRepository<TEntity, TEntityPk> Repository { get; }

        [HttpGet("count")]
        public virtual async Task<ActionResult<int>> Count()
        {
            var result = await Repository.Count(GetQueryContext());
            return Ok(result);
        }

        protected virtual Task AfterSave(TRestModel restModel, TEntity domainModel)
        {
            return Task.CompletedTask;
        }
    }
}