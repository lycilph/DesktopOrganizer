using System.Collections.Generic;
using Newtonsoft.Json;

namespace Core.Data
{
    public class Program
    {
        public string ProcessName { get; set; }
        public string ProcessPath { get; set; }
        public bool Admin { get; set; }
        public User32.WindowPlacement Placement { get; set; }

        [JsonIgnore]
        public List<Window> Windows { get; set; }

        public Program()
        {
            Windows = new List<Window>();
        }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static Program Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<Program>(json);
        }
    }
}
