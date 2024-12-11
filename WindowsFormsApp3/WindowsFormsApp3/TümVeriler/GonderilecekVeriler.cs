using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aerospace_GroundStation.veriler
{
    public class GonderilecekVeriler//iot nem sıcaklık verileri
    {
        public string RHRH { get; set; }//rakam harf rakam harf kodu
        public string IOTData { get; set; }
        public double IOTSicaklik { get; set; }
        public int IOTNem { get; set; }
    }
}
