using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthesisQBOnline
{
    public class JWKS
    {
        [JsonProperty("keys")]
        public List<Key> Keys { get; set; }
    }

    public class Key
    {
        [JsonProperty("kty")]
        public string Kty { get; set; }

        //exponent value
        [JsonProperty("e")]
        public string E { get; set; }


        [JsonProperty("use")]
        public string Use { get; set; }


        [JsonProperty("kid")]
        public string Kid { get; set; }

        [JsonProperty("alg")]
        public string Alg { get; set; }

        //modulus value
        [JsonProperty("n")]
        public string N { get; set; }
    }
}
