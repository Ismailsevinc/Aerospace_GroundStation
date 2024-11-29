using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aerospace_GroundStation.veriler
{
    public class AlinacakVeriler
    {
        public int PaketNumarasi { get; set; }
        public int UyduStatusu { get; set; }
        public int HataKodu { get; set; }
        public long GondermeSaati { get; set; }
        public float Basinc1 { get; set; }
        public float Basinc2 { get; set; }
        public float Yukseklik1 { get; set; }
        public float Yukseklik2 { get; set; }
        public float İrtifaFarki { get; set; }
        public double İnisHizi { get; set; }
        public double Sicaklik { get; set; }
        public int PilGerilimi { get; set; }
        public float GPS1Latitude { get; set; }
        public float GPS1Longitude { get; set; }
        public float GPS1Altitude { get;  set; }
        public int Pitch { get; set; }
        public int Roll { get; set; }
        public int Yaw { get; set; }
    }
}
