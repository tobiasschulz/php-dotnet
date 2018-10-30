using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace PHP.Standard
{
    public static class ConfigHelper
    {
        private static ObjectPool<JsonSerializerSettings> _serializerSettingsPool = new ObjectPool<JsonSerializerSettings> (() => _createJsonSerializerSettings ());

        private static JsonSerializerSettings _createJsonSerializerSettings ()
        {
            return new JsonSerializerSettings ()
            {
                ContractResolver = new CamelCaseExceptDictionaryKeysResolver (),
                Converters = { new BetterEnumConverter { CamelCaseText = false } },
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            };
        }

        static void HandleError (object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args, string context)
        {
            Log.Error ($"JSON-related error ({context}): {args.ErrorContext.Error}");

            args.ErrorContext.Handled = true;
        }

        static void IgnoreError (object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args)
        {
        }

        public static object ReadConfig (string content, Type type, bool ignoreNull = true, bool errorHandling = true, string context = null)
        {
            if (content == null)
            {
                throw new ArgumentNullException (nameof (content));
            }
            JsonSerializerSettings ss = _serializerSettingsPool.GetObject ();
            try
            {
                ss.NullValueHandling = ignoreNull ? NullValueHandling.Ignore : NullValueHandling.Include;
                if (errorHandling)
                {
                    ss.Error = (s, a) => HandleError (s, a, context);
                }
                else
                {
                    ss.Error = IgnoreError;
                }
                object stuff = JsonConvert.DeserializeObject (content, type, ss);
                if (stuff == null)
                {
                    try
                    {
                        stuff = Activator.CreateInstance (type: type);
                    }
                    catch
                    {
                    }
                }
                return stuff;
            }
            finally
            {
                _serializerSettingsPool.PutObject (ss);
            }
        }

        public static Config ReadConfig<Config> (string content, bool ignoreNull = true, bool errorHandling = true, string context = null) where Config : class, new()
        {
            if (content == null)
            {
                throw new ArgumentNullException (nameof (content));
            }
            JsonSerializerSettings ss = _serializerSettingsPool.GetObject ();
            try
            {
                ss.NullValueHandling = ignoreNull ? NullValueHandling.Ignore : NullValueHandling.Include;
                if (errorHandling)
                {
                    ss.Error = (s, a) => HandleError (s, a, context);
                }
                else
                {
                    ss.Error = IgnoreError;
                }
                Config stuff = JsonConvert.DeserializeObject<Config> (content, ss) ?? new Config ();
                return stuff;
            }
            finally
            {
                _serializerSettingsPool.PutObject (ss);
            }
        }

        public static string WriteConfig<Config> (Config stuff, bool inline = false, bool ignoreNull = true, bool errorHandling = true, string context = null) where Config : class, new()
        {
            if (stuff == null)
            {
                throw new ArgumentNullException ("object that should be encoded to json is null!");
            }
            JsonSerializerSettings ss = _serializerSettingsPool.GetObject ();
            try
            {
                ss.NullValueHandling = ignoreNull ? NullValueHandling.Ignore : NullValueHandling.Include;
                if (errorHandling)
                {
                    ss.Error = (s, a) => HandleError (s, a, context);
                }
                else
                {
                    ss.Error = IgnoreError;
                }
                string content = JsonConvert.SerializeObject (stuff, inline ? Formatting.None : Formatting.Indented, ss) + "\n";
                return content;
            }
            finally
            {
                _serializerSettingsPool.PutObject (ss);
            }
        }
    }

    public class BetterEnumConverter : StringEnumConverter
    {
        public override object ReadJson (JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (string.IsNullOrEmpty (reader.Value.ToString ()))
            {
                return Enum.GetValues (objectType).GetValue (0);
            }

            return base.ReadJson (reader, objectType, existingValue, serializer);
        }
    }

    class CamelCaseExceptDictionaryKeysResolver : Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver
    {
        protected override JsonDictionaryContract CreateDictionaryContract (Type objectType)
        {
            JsonDictionaryContract contract = base.CreateDictionaryContract (objectType);
            contract.DictionaryKeyResolver = propertyName => propertyName;
            return contract;
        }
    }
}
