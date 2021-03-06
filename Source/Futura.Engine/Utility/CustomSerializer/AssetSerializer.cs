﻿using Futura.Engine.Resources;
using System;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace Futura.Engine.Utility.CustomSerializer
{
    class AssetSerializer<T> : JsonConverter<T> where T : Asset
    {
        public override T ReadJson(JsonReader reader, Type objectType, [AllowNull] T existingValue, bool hasExistingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            if (reader.Value == null) return null;
            Guid identifier = new Guid((string)reader.Value);
            return ResourceManager.Instance.GetAsset<T>(identifier);
        }

        public override void WriteJson(JsonWriter writer, [AllowNull] T value, Newtonsoft.Json.JsonSerializer serializer)
        {
            if (value == null) writer.WriteValue(Guid.Empty);
            else 
                writer.WriteValue(value.Identifier.ToString());
        }
    }
}
