using System;
using System.Collections.Generic;
using BioEngine.Core.Providers;
using Newtonsoft.Json;

namespace BioEngine.Core.API.Entities
{
    public class Settings
    {
        public string Name { get; }
        public string Key { get; }
        public bool IsEditable { get; }
        public List<SettingsProperty> Properties { get; } = new List<SettingsProperty>();

        [JsonConstructor]
        private Settings(string name, string key, bool isEditable)
        {
            Name = name;
            Key = key;
            IsEditable = isEditable;
        }

        public static Settings Create(SettingsBase settings)
        {
            var schema = SettingsProvider.GetSchema(settings.GetType());
            var restModel = new Settings(schema.name, settings.GetType().FullName.Replace(".", "-"), schema.isEditable);

            foreach (var propertyInfo in schema.properties)
            {
                var property = new SettingsProperty(propertyInfo.Key.Replace(".", "-"), propertyInfo.Value.name,
                    propertyInfo.Value.type,
                    settings.GetType().GetProperty(propertyInfo.Key)?.GetValue(settings, null));
                restModel.Properties.Add(property);
            }

            return restModel;
        }

        public SettingsBase GetSettings()
        {
            var settings = SettingsProvider.GetInstance(Key.Replace("-", "."));
            if (settings == null)
            {
                throw new ArgumentException($"Class {Key} is not registered in settings provider");
            }

            foreach (var settingsProperty in Properties)
            {
                var property = settings.GetType().GetProperty(settingsProperty.Key.Replace("-", "."));
                if (property != null)
                {
                    object value = null;
                    if (property.PropertyType.IsEnum)
                    {
                        var enumType = property.PropertyType;
                        if (Enum.IsDefined(enumType, settingsProperty.Value.ToString()))
                            value = Enum.Parse(enumType, settingsProperty.Value.ToString());
                    }

                    if (property.PropertyType == typeof(bool))
                        value = settingsProperty.Value.ToString() == "1" ||
                                settingsProperty.Value.ToString() == "true" ||
                                settingsProperty.Value.ToString() == "on" ||
                                settingsProperty.Value.ToString() == "checked";
                    else if (property.PropertyType == typeof(Uri))
                        value = new Uri(Convert.ToString(value));
                    else value = Convert.ChangeType(settingsProperty.Value, property.PropertyType);

                    property.SetValue(settings, value);
                }
            }

            return settings;
        }
    }

    public class SettingsProperty
    {
        public string Name { get; }
        public string Key { get; }
        public SettingType Type { get; }
        public object Value { get; }

        public SettingsProperty(string key, string name, SettingType type, object value)
        {
            Key = key;
            Name = name;
            Type = type;
            Value = value;
        }
    }
}