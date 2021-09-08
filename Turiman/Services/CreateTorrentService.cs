using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MonoTorrent;
using MonoTorrent.BEncoding;
using Torman.Services;

namespace Turiman.Services
{
    public class CreateTorrentService : ICreateTorrentService
    {
        private readonly IConfiguration configuration;
        public CreateTorrentService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void Dispose()
        {
        }

        public void Create(string directory, string outfile)
        {
            var tc = new MonoTorrent.TorrentCreator
            {
                Announce = configuration.GetValue<string>($"Creator:Announce")
            };
            var bck = new BEncodedString("url-list");
            var vals = new List<BEncodedValue>();
            vals.Add(new BEncodedString(configuration.GetValue<string>($"Creator:WebSeed")));
            var bcv = new BEncodedList(vals);
            tc.SetCustom(bck, bcv);
            var tfs = new TorrentFileSource(directory, false);
           
            tc.Create(tfs, outfile);
        }
    }
}
