using System;
using System.Collections.Generic;
using MonoTorrent.Client;

namespace Torman.Services
{
    public interface ICreateTorrentService : IDisposable
    {
        public void Create(string directory, string outfile);
    }
}