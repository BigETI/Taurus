using Newtonsoft.Json;
using System;

/// <summary>
/// Taurus JSON converters
/// </summary>
namespace Taurus.JSONConverters
{
    /// <summary>
    /// A class used for converting enumerator values to JSON or BSON and vice versa
    /// </summary>
    /// <typeparam name="TEnumerator">Enum type</typeparam>
    public abstract class AEnumeratorValueJSONConverter<TEnumerator> : JsonConverter where TEnumerator : struct, Enum
    {
        /// <summary>
        /// Default enumerator value
        /// </summary>
        private readonly TEnumerator defaultEnumeratorValue;

        /// <summary>
        /// Constructs a new JSON converter for enumerator values
        /// </summary>
        /// <param name="defaultEnumeratorValue">Default enumerator value</param>
        public AEnumeratorValueJSONConverter(TEnumerator defaultEnumeratorValue) : base() => this.defaultEnumeratorValue = defaultEnumeratorValue;

        /// <summary>
        /// Is type nullable
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>"true" if type is nullable, otherwise "false"</returns>
        private static bool IsTypeNullable(Type type) => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);

        /// <summary>
        /// Can convert the specified object type
        /// </summary>
        /// <param name="objectType">Object type</param>
        /// <returns>"true" if the specified object can be converted, otherwise "false"</returns>
        public override bool CanConvert(Type objectType) => (IsTypeNullable(objectType) ? Nullable.GetUnderlyingType(objectType) : objectType).IsEnum;

        /// <summary>
        /// Reads JSON
        /// </summary>
        /// <param name="reader">JSON reader</param>
        /// <param name="objectType">Object type</param>
        /// <param name="existingValue">Existing value</param>
        /// <param name="serializer">JSON serializer</param>
        /// <returns>Read object</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer) =>
            ((reader.TokenType == JsonToken.String) && (reader.Value != null) && Enum.TryParse(reader.Value.ToString(), out TEnumerator enumerator_value)) ?
                enumerator_value :
                defaultEnumeratorValue;

        /// <summary>
        /// Writes JSON
        /// </summary>
        /// <param name="writer">JSON writer</param>
        /// <param name="value">JSON value</param>
        /// <param name="serializer">JSON serializer</param>
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) => writer.WriteValue(value?.ToString());
    }
}
