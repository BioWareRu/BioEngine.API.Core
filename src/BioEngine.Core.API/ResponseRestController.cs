using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using BioEngine.Core.API.Interfaces;
using BioEngine.Core.API.Models;
using BioEngine.Core.API.Response;
using BioEngine.Core.DB;
using BioEngine.Core.Interfaces;
using BioEngine.Core.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace BioEngine.Core.API
{
    public abstract class ResponseRestController<TEntity, TEntityPk, TResponse> : ApiController
        where TResponse : IResponseRestModel<TEntity, TEntityPk> where TEntity : class, IEntity<TEntityPk>
    {
        protected IBioRepository<TEntity, TEntityPk> Repository { get; }

        protected ResponseRestController(BaseControllerContext<TEntity, TEntityPk> context) : base(context)
        {
            Repository = context.Repository;
        }

        protected virtual async Task<TResponse> MapRestModelAsync(TEntity domainModel)
        {
            var restModel = HttpContext.RequestServices.GetRequiredService<TResponse>();
            await restModel.SetEntityAsync(domainModel);
            return restModel;
        }

        [HttpGet]
        public virtual async Task<ActionResult<ListResponse<TResponse>>> GetAsync()
        {
            var result = await Repository.GetAllAsync(GetQueryContext());
            return await ListAsync(result);
        }

        [HttpGet("{id}")]
        public virtual async Task<ActionResult<TResponse>> GetAsync(TEntityPk id)
        {
            var entity = await Repository.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            return Model(await MapRestModelAsync(entity));
        }

        [HttpGet("new")]
        public virtual async Task<ActionResult<TResponse>> NewAsync()
        {
            return Model(await MapRestModelAsync(await Repository.NewAsync()));
        }


        [HttpGet("count")]
        public virtual async Task<ActionResult<int>> CountAsync()
        {
            var result = await Repository.CountAsync(GetQueryContext());
            return Ok(result);
        }

        protected QueryContext<TEntity, TEntityPk> GetQueryContext()
        {
            var context = new QueryContext<TEntity, TEntityPk> {IncludeUnpublished = true};
            if (Request.Query.ContainsKey("limit"))
            {
                context.Limit = int.Parse(ControllerContext.HttpContext.Request.Query["limit"]);
            }

            if (Request.Query.ContainsKey("offset"))
            {
                context.Offset = int.Parse(ControllerContext.HttpContext.Request.Query["offset"]);
            }

            if (Request.Query.ContainsKey("order"))
            {
                context.SetOrderByString(ControllerContext.HttpContext.Request.Query["order"]);
            }

            if (HttpContext.Request.Query.ContainsKey("filter") &&
                !string.IsNullOrEmpty(ControllerContext.HttpContext.Request.Query["filter"]) &&
                ControllerContext.HttpContext.Request.Query["filter"] != "null")
            {
                var str = ControllerContext.HttpContext.Request.Query["filter"].ToString();
                var mod4 = str.Length % 4;
                if (mod4 > 0)
                {
                    str += new string('=', 4 - mod4);
                }

                var data = Convert.FromBase64String(str);
                var decodedString = HttpUtility.UrlDecode(Encoding.UTF8.GetString(data));
                var where = JsonConvert.DeserializeObject<List<QueryContextConditionsGroup>>(decodedString);
                if (where != null)
                {
                    context.SetWhere(where);
                }
            }

            return context;
        }

        protected async Task<ActionResult<ListResponse<TResponse>>> ListAsync(
            (IEnumerable<TEntity> items, int itemsCount) result)
        {
            var restModels = new List<TResponse>();
            foreach (var item in result.items)
            {
                restModels.Add(await MapRestModelAsync(item));
            }

            return Ok(new ListResponse<TResponse>(restModels, result.itemsCount));
        }

        protected ActionResult<TResponse> Model(
            TResponse model)
        {
            if (model == null)
            {
                return NotFound();
            }

            return Ok(model);
        }

        protected new ActionResult<TResponse> NotFound()
        {
            return Errors(StatusCodes.Status404NotFound, new[] {new RestErrorResponse("Not Found")});
        }

        protected ActionResult<TResponse> Errors(int code,
            IEnumerable<IErrorInterface> errors)
        {
            return new ObjectResult(new RestResponse(code, errors)) {StatusCode = code};
        }
    }
}