using System.Data;
using System.Net.NetworkInformation;
using Newtonsoft.Json;
using popingui.Models;
using Terminal.Gui;

namespace popingui;

internal static class Program
{
    public static Config config = null!;
    public static readonly DataTable Dt = new();
    private static readonly List<Task> Tasks = new();

    /**
     * Background task that infinitely pinging 1 given host
     */
    private static Task DoPing(Host host)
    {
        Ping pingSender = new Ping();

        while (true)
        {
            try
            {
                var reply = pingSender.Send(host.Address, host.TimeOut);
                host.NumOfSent++;
                
                if (reply.Status == IPStatus.Success)
                {
                    host.NumOfReceived++;
                    host.LastPingTiming = reply.RoundtripTime;
                    if (host.LastPingTiming < host.MinimalPingTiming) host.MinimalPingTiming = host.LastPingTiming;
                    if (host.LastPingTiming > host.MaximalPingTiming) host.MaximalPingTiming = host.LastPingTiming;
                    host.SumOfPingTimes += host.LastPingTiming;
                    host.AveragePingTiming = host.SumOfPingTimes / host.NumOfReceived;
                }
                else
                {
                    host.NumOfLost++;
                    host.LastPingTiming = -1;
                }
            }
            finally
            {
                Thread.Sleep(host.Interval);
            }
        }
    }

    /**
     * Background task that infinitely refreshing table
     */
    private static void RefreshDataTable()
    {
        while (true)
        {
            try
            {
                Dt.Rows.Clear();
    
                foreach(var host in config.Hosts.Reverse()) {
                    Dt.Rows.Add(new object[]
                    {
                        host.Address,
            
                        host.LastPingTiming,
                        host.MinimalPingTiming,
                        host.MaximalPingTiming,
                        host.AveragePingTiming,
            
                        host.NumOfSent,
                        host.NumOfReceived,
                        host.NumOfLost,
                        host.PercentOfReceived,
                        host.PercentOfLost,
                    });
                }

                Terminal.Gui.Application.Refresh();
            }
            finally
            {
                Thread.Sleep(1000);
            }
        }
    }

    static async Task Main()
    {
        #region Parse config.json
        {
            string configPath = Path.Combine(Environment.CurrentDirectory, "config.json");
            string configContent = await File.ReadAllTextAsync(configPath);
            config = JsonConvert.DeserializeObject<Config>(configContent)!;
        }
        #endregion

        #region Create head of table
        {
            Dt.Columns.Add("target");
        
            Dt.Columns.Add("last");
            Dt.Columns.Add("min");
            Dt.Columns.Add("max");
            Dt.Columns.Add("avg");
            
            Dt.Columns.Add("# sent");
            Dt.Columns.Add("# recv");
            Dt.Columns.Add("# lost");
            Dt.Columns.Add("% recv");
            Dt.Columns.Add("% lost");
        }
        #endregion

        #region Create tasks
        foreach (Host host in config.Hosts)
        {
            Tasks.Add(
                Task.Run(async () =>
                {
                    await DoPing(host);
                })
            );
        }
        
        Tasks.Add(Task.Run(() => {
            Thread.Sleep(1000);
            RefreshDataTable();
        }));
        #endregion

        #region Launch TUI
        Application.Init();

        try
        {
            Application.Run(new MyView());
        }
        finally
        {
            Application.Shutdown();
        }
        #endregion
    }
}