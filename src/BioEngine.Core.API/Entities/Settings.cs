using System;
using System.Collections.Generic;
using System.Linq;
using BioEngine.Core.Providers;
using Newtonsoft.Json;

namespace BioEngine.Core.API.Entities
{
    public class Settings
    {
        public string Name { get; }
        public string Key { get; }
        public bool IsEditable { get; }
        public SettingMode Mode { get; }
        public List<SettingsProperty> Properties { get; } = new List<SettingsProperty>();

        [JsonConstructor]
        private Settings(string name, string key, bool isEditable, SettingMode mode)
        {
            Name = name;
            Key = key;
            IsEditable = isEditable;
            Mode = mode;
        }

        public static Settings Create(SettingsEntry settingsEntry)
        {
            var restModel = new Settings(settingsEntry.Schema.Name, settingsEntry.Schema.Key.Replace(".", "-"),
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
                    propertyInfo.Type, values);
                restModel.Properties.Add(property);
            }

            return restModel;
        }

        public SettingsEntry GetSettings()
        {
            var key = Key.Replace("-", ".");
            var settings = SettingsProvider.GetInstance(key);
            if (settings == null)
            {
                throw new ArgumentException($"Class {Key} is not registered in settings provider");
            }

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
            object parsedValue = null;
            if (propertyType.IsEnum)
            {
                var enumType = propertyType;
                if (Enum.IsDefined(enumType, value.ToString()))
                    parsedValue = Enum.Parse(enumType, value.ToString());
            }

            if (propertyType == typeof(bool))
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

    public class SettingsProperty
    {
        public string Name { get; }
        public string Key { get; }
        public SettingType Type { get; }
        public List<SettingsPropertyValue> Values { get; }

        public SettingsProperty(string key, string name, SettingType type, List<SettingsPropertyValue> values)
        {
            Key = key;
            Name = name;
            Type = type;
            Values = values;
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