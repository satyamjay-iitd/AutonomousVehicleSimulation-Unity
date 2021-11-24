using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Localization
{
    public class LocalizationOutput
    {
        public float NextNodeRx  { get; set; }
        public float NextNodeRy  { get; set; }
        
        public float NextNodeLx  { get; set; }
        public float NextNodeLy  { get; set; }
        
        public float CarPosX    { get; set; }
        public float CarPosY    { get; set; }
        
        public float CarDirX    { get; set; }
        public float CarDirY    { get; set; }

        public bool HasReachedDestination { get; set; }
    }

    public class LocalizationOutputDeserializer: JsonConverter<Dictionary<string, string>>
    {
        public override bool CanRead  => true;
        public override bool CanWrite => false;
        
        public override Dictionary<string, string> ReadJson(JsonReader reader, Type objectType, Dictionary<string, string> existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            var dict = new Dictionary<string, string>();
            var jsonObject = JObject.Load(reader);
            var carPosX = jsonObject.GetValue("CurrPosX") as JValue;
            var carPosY = jsonObject.GetValue("CurrPosY") as JValue;
            var nextNodeRx = jsonObject.GetValue("NextNodeRX") as JValue;
            var nextNodeRy = jsonObject.GetValue("NextNodeRY") as JValue;
            var nextNodeLx = jsonObject.GetValue("NextNodeLX") as JValue;
            var nextNodeLy = jsonObject.GetValue("NextNodeLY") as JValue;
            var hasReachedDestination = jsonObject.GetValue("HasReached") as JValue;
            dict["CurrPosX"] = (string) carPosX;
            dict["CurrPosY"] = (string) carPosY;
            dict["NextNodeRX"] = (string) nextNodeRx;
            dict["NextNodeRY"] = (string) nextNodeRy;
            dict["NextNodeLX"] = (string) nextNodeLx;
            dict["NextNodeLY"] = (string) nextNodeLy;
            dict["HasReachedDestination"] = (string) hasReachedDestination;
            return dict;
        }
        
        public override void WriteJson(JsonWriter writer, Dictionary<string, string> value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

}
