using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BioEngine.Core.Abstractions;
using BioEngine.Core.API.Entities;
using BioEngine.Core.Properties;
using Newtonsoft.Json;

namespace BioEngine.Core.API.Models
{
    public abstract class RestModel<TEntity> where TEntity : class, IEntity
    {
        public Guid Id { get; set; }
        public DateTimeOffset DateAdded { get; set; }
        public DateTimeOffset DateUpdated { get; set; }

        public string Title { get; set; }
        public string Url { get; set; }

        [JsonIgnore] public List<PropertiesEntry> Properties { get; set; }
        public List<PropertiesGroup> PropertiesGroups { get; set; }

        protected virtual Task ParseEntityAsync(TEntity entity)
        {
            Id = entity.Id;
            DateAdded = entity.DateAdded;
            DateUpdated = entity.DateUpdated;
            
            Title = entity.Title;
            Url = entity.Url;
            PropertiesGroups =
                entity.Properties.Select(p => PropertiesGroup.Create(p, PropertiesProvider.GetSchema(p.Key))).ToList();
            return Task.CompletedTask;
        }

        protected virtual Task<TEntity> FillEntityAsync(TEntity entity)
        {
            entity ??= CreateEntity();
            entity.Id = Id;
            entity.Title = Title;
            entity.Url = Url;
            entity.Properties = PropertiesGroups?.Select(s => s.GetPropertiesEntry()).ToList();
            return Task.FromResult(entity);
        }

        protected virtual TEntity CreateEntity()
        {
            return Activator.CreateInstance<TEntity>();
        }
    }
}
