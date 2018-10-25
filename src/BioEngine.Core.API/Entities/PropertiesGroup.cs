using System;
using System.Collections.Generic;
using System.Linq;
using BioEngine.Core.Properties;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace BioEngine.Core.API.Entities
{
    [PublicAPI]
    public class PropertiesGroup
    {
        public string Name { get; }
        public string Key { get; }
        public bool IsEditable { get; }
        public PropertiesQuantity Mode { get; }
        public List<PropertiesElement> Properties { get; } = new List<PropertiesElement>();

        [JsonConstructor]
        private PropertiesGroup(string name, string key, bool isEditable, PropertiesQuantity mode)
        {
            Name = name;
            Key = key;
            IsEditable = isEditable;
            Mode = mode;
        }

        public static PropertiesGroup Create(PropertiesEntry propertiesEntry)
        {
            var restModel = new PropertiesGroup(propertiesEntry.Schema.Name, propertiesEntry.Schema.Key.Replace(".", "-"),
                propertiesEntry.Schema.IsEditable, propertiesEntry.Schema.Mode);

            foreach (var propertyInfo in propertiesEntry.Schema.Properties)
            {
                var values = new List<PropertiesElementValue>();
                foreach (var propertiesValue in propertiesEntry.Properties)
                {
                    var value = propertiesValue.Value.GetType().GetProperty(propertyInfo.Key)?.GetValue(propertiesValue.Value, null);
                    values.Add(new PropertiesElementValue(propertiesValue.SiteId, value));
                }

                var property = new PropertiesElement(propertyInfo.Key.Replace(".", "-"), propertyInfo.Name,
                    propertyInfo.Type, values, propertyInfo.IsRequired);
                restModel.Properties.Add(property);
            }

            return restModel;
        }

        public PropertiesEntry GetPropertiesEntry()
        {
            var key = Key.Replace("-", ".");
            var propertiesSet = PropertiesProvider.GetInstance(key);

            var entry = new PropertiesEntry(key, PropertiesProvider.GetSchema(propertiesSet.GetType()));
            var propertiesValues = new List<PropertiesValue>();

            foreach (var propertiesElement in Properties)
            {
                var property = propertiesSet.GetType().GetProperty(propertiesElement.Key.Replace("-", "."));
                if (property != null)
                {
                    foreach (var propertyValue in propertiesElement.Values)
                    {
                        var value = ParsePropertyValue(property.PropertyType, propertyValue.Value);
                        var propertiesValue = propertiesValues.FirstOrDefault(v => v.SiteId == propertyValue.SiteId);
                        if (propertiesValue == null)
                        {
                            propertiesValue = new PropertiesValue(propertyValue.SiteId, PropertiesProvider.GetInstance(key));
                            propertiesValues.Add(propertiesValue);
                        }

                        property.SetValue(propertiesValue.Value, value);
                    }
                }
            }

            entry.Properties.AddRange(propertiesValues);

            return entry;
        }

        private static object ParsePropertyValue(Type propertyType, object value)
        {
            if (value == null) return null;
            object parsedValue = null;
            if (propertyType.IsEnum)
            {
                var enumType = propertyType;
                if (Enum.IsDefined(enumType, value.ToString()))
                    parsedValue = Enum.Parse(enumType, value.ToString());
            }

            else if (propertyType == typeof(bool))
                parsedValue = value.ToString() == "1" ||
                              value.ToString() == "true" ||
                              value.ToString() == "on" ||
                              value.ToString() == "checked";
            else if (propertyType == typeof(Uri))
                parsedValue = new Uri(Convert.ToString(value));
            else parsedValue = Convert.ChangeType(value, propertyType);

            return parsedValue;
        }
    }

    [PublicAPI]
    public class PropertiesElement
    {
        public string Name { get; }
        public string Key { get; }
        public PropertyElementType Type { get; }
        public bool IsRequired { get; }
        public List<PropertiesElementValue> Values { get; }

        public PropertiesElement(string key, string name, PropertyElementType type, List<PropertiesElementValue> values,
            bool isRequired)
        {
            Key = key;
            Name = name;
            Type = type;
            Values = values;
            IsRequired = isRequired;
        }
    }

    public class PropertiesElementValue
    {
        public PropertiesElementValue(int? siteId, object value)
        {
            SiteId = siteId;
            Value = value;
        }

        public int? SiteId { get; }
        public object Value { get; }
    }
}