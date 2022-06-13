using System.Collections.Generic;

namespace ZagerOrderFixWorker.Models.Zager
{
    public class Response
    {
        public string success { get; set; }
        public List<object> error { get; set; }
    }
}
