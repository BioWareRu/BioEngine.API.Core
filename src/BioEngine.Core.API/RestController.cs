using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BioEngine.Core.API.Interfaces;
using BioEngine.Core.API.Response;
using BioEngine.Core.DB;
using BioEngine.Core.Entities;
using BioEngine.Core.Interfaces;
using BioEngine.Core.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BioEngine.Core.API
{
    public abstract class RestController : BaseController
    {
        protected RestController(BaseControllerContext context) : base(context)
        {
        }
    }


    public abstract class RestController<T, TPkType> : RestController where T : class, IEntity<TPkType>
    {
        [HttpGet]
        public virtual async Task<ActionResult<ListResponse<T, TPkType>>> Get()
        {
            var result = await GetRepository().GetAll(GetQueryContext());
            return List(result);
        }

        protected QueryContext<T, TPkType> GetQueryContext()
        {
            var context = new QueryContext<T, TPkType>();
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

        [HttpGet("{id}")]
        public virtual async Task<ActionResult<T>> Get(TPkType id)
        {
            return await GetRepository().GetById(id);
        }

        [HttpPost]
        public virtual async Task<ActionResult<T>> Add(T item)
        {
            var entity = MapEntity(Activator.CreateInstance<T>(), item);

            var result = await GetRepository().Add(entity);
            if (result.IsSuccess)
            {
                return Created(result.Entity);
            }

            return Errors(500, result.Errors.Select(e => new ValidationErrorResponse(e.PropertyName, e.ErrorMessage)));
        }

        [HttpPut("{id}")]
        public virtual async Task<ActionResult<T>> Update(TPkType id, T item)
        {
            var entity = await GetRepository().GetById(id);
            if (entity == null)
            {
                return NotFound();
            }

            entity = MapEntity(entity, item);

            var result = await GetRepository().Update(entity);
            if (result.IsSuccess)
            {
                return Updated(result.Entity);
            }

            return Errors(500, result.Errors.Select(e => new ValidationErrorResponse(e.PropertyName, e.ErrorMessage)));
        }

        protected abstract T MapEntity(T entity, T newData);

        [HttpDelete("{id}")]
        public virtual async Task<ActionResult<T>> Delete(TPkType id)
        {
            var result = await GetRepository().Delete(id);
            if (result) return Deleted();
            return BadRequest();
        }

        protected abstract BioRepository<T, TPkType> GetRepository();

        protected new ActionResult<T> NotFound()
        {
            return Errors(StatusCodes.Status404NotFound, new[] {new RestErrorResponse("Not Found")});
        }

        protected ActionResult<T> Created(T model)
        {
            return SaveResponse(StatusCodes.Status201Created, model);
        }

        protected ActionResult<T> Updated(T model)
        {
            return SaveResponse(StatusCodes.Status202Accepted, model);
        }

        protected ActionResult<T> Deleted()
        {
            return SaveResponse(StatusCodes.Status204NoContent, null);
        }

        protected ActionResult<T> Errors(int code, IEnumerable<IErrorInterface> errors)
        {
            return new ObjectResult(new RestResponse(code, errors)) {StatusCode = code};
        }

        private ActionResult<T> SaveResponse(int code, T model)
        {
            return new ObjectResult(new SaveModelResponse<T>(code, model)) {StatusCode = code};
        }

        protected ActionResult<ListResponse<T, TPkType>> List((IEnumerable<T> items, int itemsCount) result)
        {
            return Ok(new ListResponse<T, TPkType>(result.items, result.itemsCount));
        }

        protected ActionResult<T> Model(T model)
        {
            if (model == null)
            {
                return NotFound();
            }

            return Ok(model);
        }

        protected RestController(BaseControllerContext context) : base(context)
        {
        }
    }

    public abstract class SectionController<T, TId> : RestController<T, TId> where T : Section, IEntity<TId>
    {
        protected SectionController(BaseControllerContext context) : base(context)
        {
        }

        protected T MapSectionData(T entity, T newData)
        {
            entity.ParentId = newData.ParentId;
            entity.ForumId = newData.ForumId;
            entity.Title = newData.Title;
            entity.Url = newData.Url;
            entity.Logo = newData.Logo;
            entity.LogoSmall = newData.LogoSmall;
            entity.Description = newData.Description;
            entity.ShortDescription = newData.ShortDescription;
            entity.Keywords = newData.Keywords;
            entity.Hashtag = newData.Hashtag;
            entity.SiteIds = newData.SiteIds;

            return entity;
        }
    }

    public abstract class ContentController<T, TId> : RestController<T, TId> where T : ContentItem, IEntity<TId>
    {
        protected ContentController(BaseControllerContext context) : base(context)
        {
        }

        protected T MapContentData(T entity, T newData)
        {
            if (entity.AuthorId == 0)
            {
                entity.AuthorId = CurrentUser.Id;
            }

            entity.Title = newData.Title;
            entity.Url = newData.Url;
            entity.Description = newData.Description;
            entity.SectionIds = newData.SectionIds;

            return entity;
        }
    }
}