using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MonoTorrent;
using MonoTorrent.Client;
using MonoTorrent.Tracker;
using MonoTorrent.Tracker.Listeners;


namespace Torman.Services
{
    public class TrackerService : ITrackerService
    {

        protected TrackerServer tracker;
        protected ITrackerListener listener;
        private readonly IConfiguration configuration;
        public event Action Notify;
        public string IsRunning => listener.Status.ToString();

        private readonly FileSystemWatcher watcher;

        public TrackerService(IConfiguration configuration)
        {
            this.configuration = configuration;

            var os = "Win";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                os = "Unix";
            }
            watcher = new FileSystemWatcher(this.configuration.GetValue<string>($"TorrentFileSearchDir{os}"));
            watcher.Created += OnCreated;
            watcher.Filter = "*.torrent";
            watcher.IncludeSubdirectories = false;
            watcher.EnableRaisingEvents = true;

            tracker = new TrackerServer
            {
                AllowUnregisteredTorrents = false
            };

            listener = TrackerListenerFactory.CreateHttp(IPAddress.Parse(configuration.GetValue<string>("Tracker:ListenerAddress")), configuration.GetValue<int>("Tracker:ListenerPort"));
            listener.Start();
            tracker.RegisterListener(listener);

            foreach (var file in Directory.GetFiles(this.configuration.GetValue<string>($"TorrentFileSearchDir{os}"), "*.torrent"))
            {
                var t = Torrent.Load(file);
                tracker.Add(new InfoHashTrackable(t));
            }
        }

        public void Dispose()
        {
            //tracker.UnregisterListener(listener);
            //watcher.Dispose();
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            var t = Torrent.Load(e.FullPath);
            tracker.Add(new InfoHashTrackable(t));
        }

        public IList<ITrackerItem> GetTrackerItems()
        {
            return tracker.GetTrackerItems();
        }

    }
}
