using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MonoTorrent.Client;

namespace Turiman.Models
{
    public class PeerInfo
    {
        public bool AmChoking { get; set; }
        public bool AmInterested { get; set; }
        public int AmdRequestingPiecesCount { get; set; }
        public string ClientApp { get; set; }
        public string Encryption { get; set; }
        public bool IsSeeder{ get; set; }
        public long DL { get; set; }
        public long UP { get; set; }
        public string Desc { get; set; }
        public double Progress { get; set; }
        public string Flag { get; set; }

        public PeerInfo(PeerId pid)
        {
            this.AmChoking = pid.AmChoking;
            this.AmInterested = pid.AmInterested;
            this.AmdRequestingPiecesCount = pid.AmRequestingPiecesCount;
            this.ClientApp = pid.ClientApp.ShortId;
            this.Encryption = pid.EncryptionType.ToString();
            this.IsSeeder = pid.IsSeeder;
            this.DL = pid.Monitor.DownloadSpeed;
            this.UP = pid.Monitor.UploadSpeed;
            this.Desc = pid.Uri.Host;
            this.Progress = pid.BitField.PercentComplete;
        }
    }
}
