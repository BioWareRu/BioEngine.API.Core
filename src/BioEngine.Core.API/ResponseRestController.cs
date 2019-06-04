﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using BioEngine.Core.Abstractions;
using BioEngine.Core.API.Interfaces;
using BioEngine.Core.API.Models;
using BioEngine.Core.API.Response;
using BioEngine.Core.DB.Queries;
using BioEngine.Core.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace BioEngine.Core.API
{
    public abstract class ResponseRestController<TEntity, TRepository, TResponse> : ApiController
        where TResponse : IResponseRestModel<TEntity>
        where TEntity : class, IEntity
        where TRepository : IBioRepository<TEntity>
    {
        protected TRepository Repository { get; }

        protected ResponseRestController(BaseControllerContext<TEntity, TRepository> context) : base(context)
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
        public virtual async Task<ActionResult<ListResponse<TResponse>>> GetAsync([FromQuery] int limit = 20,
            [FromQuery] int offset = 0, [FromQuery] string order = null, [FromQuery] string filter = null)
        {
            var result = await Repository.GetAllAsync(GetQueryContext(limit, offset, order, filter));
            return await ListAsync(result);
        }

        [HttpGet("{id}")]
        public virtual async Task<ActionResult<TResponse>> GetAsync(Guid id)
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
        public virtual async Task<ActionResult<int>> CountAsync([FromQuery] int limit = 20,
            [FromQuery] int offset = 0, [FromQuery] string order = null, [FromQuery] string filter = null)
        {
            var result = await Repository.CountAsync(GetQueryContext(limit, offset, order, filter));
            return Ok(result);
        }

        protected QueryContext<TEntity> GetQueryContext(int limit, int offset, string order, string filter)
        {
            var context = new QueryContext<TEntity>();
            if (limit > 0)
            {
                context.Limit = limit;
            }

            if (offset > 0)
            {
                context.Offset = offset;
            }

            if (!string.IsNullOrEmpty(order))
            {
                context.SetOrderByString(order);
            }

            if (!string.IsNullOrEmpty(filter) &&
                filter != "null")
            {
                var mod4 = filter.Length % 4;
                if (mod4 > 0)
                {
                    filter += new string('=', 4 - mod4);
                }

                var data = Convert.FromBase64String(filter);
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
