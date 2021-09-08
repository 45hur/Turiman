using System;
using System.Collections.Generic;
using MonoTorrent.Client;
using MonoTorrent.Tracker;

namespace Torman.Services
{
    public interface ITrackerService : IDisposable
    {
        public event Action Notify;
        string IsRunning { get; }
        IList<ITrackerItem> GetTrackerItems();
    }
}