using System;
using System.Json;
namespace Framework.Util {

    public static class JsonSerializer {

        public static JsonValue Deserialize(string jsonString) {
            try {
                if (string.IsNullOrEmpty(jsonString)) 
                    return new JsonObject();
                var re = JsonValue.Parse(jsonString);
                return re;
            } catch (Exception ex) {
                DebugHelper.LogError(string.Format("{0}{1}", jsonString, ex), true);
                return null;
            }
        }
    }
}
