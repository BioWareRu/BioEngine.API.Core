using System;
using System.Collections.Generic;
using BioEngine.Core.API.Entities;
using BioEngine.Core.Interfaces;
using BioEngine.Core.Providers;
using BioEngine.Core.Storage;
using Newtonsoft.Json;

namespace BioEngine.Core.API.Request
{
    public abstract class RestModel<T, TPk> : IEntity<TPk> where T : class, IEntity<TPk>
    {
        public object GetId() => Id;

        public TPk Id { get; set; }
        public DateTimeOffset DateAdded { get; set; }
        public DateTimeOffset DateUpdated { get; set; }
        public bool IsPublished { get; set; }
        public DateTimeOffset? DatePublished { get; set; }

        [JsonIgnore] public List<SettingsEntry> Settings { get; set; }
        public List<Settings> SettingsGroups { get; set; }
    }

    public abstract class SiteEntityRestModel<T, TPk> : RestModel<T, TPk>, ISiteEntity<TPk>
        where T : class, IEntity<TPk>
    {
        public int[] SiteIds { get; set; }
    }

    public abstract class SectionEntityRestModel<T, TPk> : SiteEntityRestModel<T, TPk>, ISectionEntity<TPk>
        where T : class, IEntity<TPk>
    {
        public int[] SectionIds { get; set; }
        public int[] TagIds { get; set; }
    }

    public abstract class SectionRestModel<T, TPk> : SiteEntityRestModel<T, TPk> where T : class, IEntity<TPk>
    {
        public virtual int Type { get; set; }
        public string TypeTitle { get; set; }
        public virtual string Title { get; set; }
        public virtual string Url { get; set; }
        public virtual StorageItem Logo { get; set; }
        public virtual StorageItem LogoSmall { get; set; }
        public virtual string ShortDescription { get; set; }
        public virtual string Hashtag { get; set; }
    }

    public abstract class SectionRestModel<T, TPk, TData> : SectionRestModel<T, TPk>, ITypedEntity<TData>
        where T : class, IEntity<TPk> where TData : TypedData, new()
    {
        public TData Data { get; set; }
    }

    public abstract class ContentEntityRestModel<T, TPk> : SectionEntityRestModel<T, TPk>, IContentEntity<TPk>
        where T : class, IEntity<TPk>
    {
        public int Type { get; set; }
        public int AuthorId { get; set; }
        public string Title { get; set; }
        public string TypeTitle { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public bool IsPinned { get; set; }
    }

    public abstract class ContentEntityRestModel<T, TPk, TData> : ContentEntityRestModel<T, TPk>
        where T : class, IEntity<TPk> where TData : TypedData, new()
    {
        public TData Data { get; set; }
    }
}