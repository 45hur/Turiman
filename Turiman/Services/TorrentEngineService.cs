using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using IPGeolocation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using MonoTorrent;
using MonoTorrent.Client;


namespace Torman.Services
{
    public class TorrentEngineService : ITorrentEngineService
    {
        protected ClientEngine engine;
        private readonly IConfiguration configuration;
        public event Action Notify;
        private readonly FileSystemWatcher watcher;

        public bool IsRunning => engine.IsRunning;
        IPGeolocationAPI geolocationApi;

        public TorrentEngineService(IConfiguration configuration)
        {
            this.configuration = configuration;

            var esb = new EngineSettingsBuilder();
            esb.AllowLocalPeerDiscovery = this.configuration.GetValue<bool>("Engine:AllowLocalPeerDiscovery");
            if (!this.configuration.GetValue<bool>("Engine:AllowPlainTextEncryption"))
            {
                esb.AllowedEncryption.Remove(EncryptionType.PlainText);
            }

            var engineSettings = esb.ToSettings();
            engine = new ClientEngine(engineSettings);
            engine.StatsUpdate += EngineOnStatsUpdate;

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

            foreach (var file in Directory.GetFiles(this.configuration.GetValue<string>($"TorrentFileSearchDir{os}"), "*.torrent"))
            {
                DownloadTorrent(file);
            }

            geolocationApi = new IPGeolocationAPI(configuration.GetValue<string>("Geolocator:Key"));

            engine.StartAllAsync();
        }

        public void Dispose()
        {
            watcher.Dispose();
            engine?.Dispose();
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            DownloadTorrent(e.FullPath);
        }

        private void EngineOnStatsUpdate(object? sender, StatsUpdateEventArgs e)
        {
            Notify?.Invoke();
        }

        private void DownloadTorrent(string path)
        {
            var t = Torrent.Load(path);
            var ts = new TorrentSettings();
            engine.AddAsync(
                t,
                this.configuration.GetValue<string>("TorrentOutDir"),
                ts);
        }

        public void StartAllAsync()
        {
            engine.StartAllAsync();
        }

        public void StopAllAsync()
        {
            engine.StopAllAsync();
        }

        public void AddTorrent(string path)
        {
            DownloadTorrent(path);

            engine.StartAllAsync();
        }


        public IList<TorrentManager> GetTorrents()
        {
            return engine.Torrents;
        }

        public void RemoveTorrent(TorrentManager tm)
        {
            engine.RemoveAsync(tm);
        }

        public TorrentSettings CreateSettingsDlSpeed(int dlSpeed, TorrentSettings prevSettings)
        {
            var tsb = new TorrentSettingsBuilder(prevSettings);
            tsb.MaximumDownloadSpeed = dlSpeed;
            tsb.AllowDht = configuration.GetValue<bool>("Engine:AllowDht");
            tsb.AllowPeerExchange = configuration.GetValue<bool>("Engine:AllowPeerExchange");
            return tsb.ToSettings();
        }

        public string GetFlag(string address)
        {
            var gp = new GeolocationParams();
            gp.SetIp(address);
            var geo = geolocationApi.GetGeolocation(gp);
            return geo.GetCountryFlag();
        }
    }
}
