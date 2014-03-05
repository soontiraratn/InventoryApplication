using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonData
{

    public class ParseAccounts
    {

        [JsonProperty("Id")]
        public int id { get; set; }

        [JsonProperty("Item")]
        public string item { get; set; }

        [JsonProperty("Quantity")]
        public string quantity { get; set; }
    }

}
