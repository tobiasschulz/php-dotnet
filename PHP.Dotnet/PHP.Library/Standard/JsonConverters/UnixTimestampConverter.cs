using System;
using System.Globalization;
using Newtonsoft.Json;

namespace PHP.Standard
{
    public class UnixTimestampConverter : JsonConverter
    {
        public override void WriteJson (JsonWriter writer, object value, JsonSerializer serializer)
        {
            UnixTimestamp _value = (UnixTimestamp)value;
            writer.WriteValue (_value.IsNotNull () ? _value.DateTime.ToString ("yyyy-MM-dd HH:mm:ss") : null);
        }

        public override object ReadJson (JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null || reader.Value.ToString () == "False")
            {
                return UnixTimestamp.Epoch;
            }
            string dateString = reader.Value.ToString ();
            if (dateString == "0000-00-00 00:00:00" || dateString == "1979-01-01 00:00:00" || dateString == "null")
            {
                return UnixTimestamp.Epoch;
            }
            if (DateTime.TryParseExact (s: dateString, format: "yyyy-MM-dd HH:mm:ss", provider: CultureInfo.InvariantCulture, style: DateTimeStyles.AssumeUniversal, result: out DateTime result))
            {
                result = result.ToUniversalTime ();
                //DK_Log.Debug ("TryParseExact: {0} - {1}", result, result.Kind);
                return UnixTimestamp.FromDateTime (result);
            }
            if (DateTime.TryParseExact (s: dateString, format: "yyyy-MM-dd", provider: CultureInfo.InvariantCulture, style: DateTimeStyles.AssumeUniversal, result: out result))
            {
                result = result.ToUniversalTime ();
                //DK_Log.Debug ("TryParseExact: {0} - {1}", result, result.Kind);
                return UnixTimestamp.FromDateTime (result);
            }
            if (long.TryParse (dateString, out var l))
            {
                return UnixTimestamp.FromUnixTimeSeconds (l);
            }
            return UnixTimestamp.Epoch;
        }

        public override bool CanConvert (Type objectType)
        {
            return objectType == typeof (UnixTimestamp);
        }
    }
}
