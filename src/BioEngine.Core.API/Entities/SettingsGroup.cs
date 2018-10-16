using System;
using System.Collections.Generic;
using System.Linq;
using BioEngine.Core.Settings;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace BioEngine.Core.API.Entities
{
    [PublicAPI]
    public class SettingsGroup
    {
        public string Name { get; }
        public string Key { get; }
        public bool IsEditable { get; }
        public SettingsMode Mode { get; }
        public List<SettingsProperty> Properties { get; } = new List<SettingsProperty>();

        [JsonConstructor]
        private SettingsGroup(string name, string key, bool isEditable, SettingsMode mode)
        {
            Name = name;
            Key = key;
            IsEditable = isEditable;
            Mode = mode;
        }

        public static SettingsGroup Create(SettingsEntry settingsEntry)
        {
            var restModel = new SettingsGroup(settingsEntry.Schema.Name, settingsEntry.Schema.Key.Replace(".", "-"),
                settingsEntry.Schema.IsEditable, settingsEntry.Schema.Mode);

            foreach (var propertyInfo in settingsEntry.Schema.Properties)
            {
                var values = new List<SettingsPropertyValue>();
                foreach (var settings in settingsEntry.Settings)
                {
                    var value = settings.Value.GetType().GetProperty(propertyInfo.Key)?.GetValue(settings.Value, null);
                    values.Add(new SettingsPropertyValue(settings.SiteId, value));
                }

                var property = new SettingsProperty(propertyInfo.Key.Replace(".", "-"), propertyInfo.Name,
                    propertyInfo.Type, values, propertyInfo.IsRequired);
                restModel.Properties.Add(property);
            }

            return restModel;
        }

        public SettingsEntry GetSettings()
        {
            var key = Key.Replace("-", ".");
            var settings = SettingsProvider.GetInstance(key);

            var entry = new SettingsEntry(key, SettingsProvider.GetSchema(settings.GetType()));
            var settingsValues = new List<SettingsValue>();

            foreach (var settingsProperty in Properties)
            {
                var property = settings.GetType().GetProperty(settingsProperty.Key.Replace("-", "."));
                if (property != null)
                {
                    foreach (var propertyValue in settingsProperty.Values)
                    {
                        var value = ParsePropertyValue(property.PropertyType, propertyValue.Value);
                        var settingsValue = settingsValues.FirstOrDefault(v => v.SiteId == propertyValue.SiteId);
                        if (settingsValue == null)
                        {
                            settingsValue = new SettingsValue(propertyValue.SiteId, SettingsProvider.GetInstance(key));
                            settingsValues.Add(settingsValue);
                        }

                        property.SetValue(settingsValue.Value, value);
                    }
                }
            }

            entry.Settings.AddRange(settingsValues);

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
    public class SettingsProperty
    {
        public string Name { get; }
        public string Key { get; }
        public SettingType Type { get; }
        public bool IsRequired { get; }
        public List<SettingsPropertyValue> Values { get; }

        public SettingsProperty(string key, string name, SettingType type, List<SettingsPropertyValue> values,
            bool isRequired)
        {
            Key = key;
            Name = name;
            Type = type;
            Values = values;
            IsRequired = isRequired;
        }
    }

    public class SettingsPropertyValue
    {
        public SettingsPropertyValue(int? siteId, object value)
        {
            SiteId = siteId;
            Value = value;
        }

        public int? SiteId { get; }
        public object Value { get; }
    }
}