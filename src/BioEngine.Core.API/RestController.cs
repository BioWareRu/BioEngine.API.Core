using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BioEngine.Core.API.Interfaces;
using BioEngine.Core.API.Response;
using BioEngine.Core.DB;
using BioEngine.Core.Interfaces;
using BioEngine.Core.Repository;
using BioEngine.Core.Storage;
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
        
        [HttpGet("count")]
        public virtual async Task<ActionResult<int>> Count()
        {
            var result = await GetRepository().Count(GetQueryContext());
            return Ok(result);
        }

        protected QueryContext<T, TPkType> GetQueryContext()
        {
            var context = new QueryContext<T, TPkType> {IncludeUnpublished = true};
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

            return Errors(StatusCodes.Status422UnprocessableEntity,
                result.Errors.Select(e => new ValidationErrorResponse(e.PropertyName, e.ErrorMessage)));
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

            return Errors(StatusCodes.Status422UnprocessableEntity,
                result.Errors.Select(e => new ValidationErrorResponse(e.PropertyName, e.ErrorMessage)));
        }

        [HttpPost("upload")]
        public virtual Task<ActionResult<StorageItem>> Upload([FromQuery] string name)
        {
            throw new NotImplementedException();
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
}