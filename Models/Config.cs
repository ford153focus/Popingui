using System.Collections.Concurrent;
using Newtonsoft.Json;

namespace popingui.Models;

public class Host
{
    public int Id { get; set; }

    [JsonProperty("address")]
    public string Address = "";

    public long LastPingTiming = 0;
    public long MinimalPingTiming = 65535;
    public long MaximalPingTiming = 0;
    public long AveragePingTiming = 0;
    public long SumOfPingTimes = 0;

    public int NumOfSent = 0;
    public int NumOfReceived = 0;
    public int NumOfLost = 0;

    public string PercentOfReceived {
        get {
            var ratio = (float)NumOfReceived/(float)NumOfSent;
            var percentage = ratio*100;
            return String.Format("{0:0.00}", percentage);
        }
    }
    public string PercentOfLost {
        get {
            var ratio = (float)NumOfLost/(float)NumOfSent;
            var percentage = ratio*100;
            return String.Format("{0:0.00}", percentage);
        }
    }

    [JsonProperty("bytes")]
    public byte RequestBytesAmount = 32;
    [JsonProperty("time_to_life")]
    public byte TimeToLife = 64;
    [JsonProperty("timeout")]
    public ushort TimeOut = 8192;
    [JsonProperty("interval")]
    public ushort Interval = 1024;
}

public class Config
{
    [JsonProperty("hosts")]
    public ConcurrentBag<Host> Hosts = null!;
}
