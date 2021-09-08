using System;
using System.Collections.Generic;
using MonoTorrent.Client;

namespace Torman.Services
{
    public interface ITorrentEngineService : IDisposable
    {
        public event Action Notify;
        bool IsRunning { get; }

        public void StartAllAsync();
        public void StopAllAsync();

        public void AddTorrent(string tor);

        public IList<TorrentManager> GetTorrents();
        public void RemoveTorrent(TorrentManager tm);
        public TorrentSettings CreateSettingsDlSpeed(int dlSpeed, TorrentSettings prevSettings);
        public string GetFlag(string address);
    }
}