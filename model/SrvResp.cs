using System.Collections.Generic;

namespace Agent
{
    public class SrvResp
    {
        public string status { get; set; }
        public List<string> arrTimestamps { get; set; }

        public SrvResp()
        {
            // arrTimestamp = new List<string>();
        }
    }
}