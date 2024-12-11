using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WindowsFormsApp3.Csv
{
    public static class CsvKontrol
    {
        public static void CsvDosyasiİlkSatirKontrolu(string dosyayolu, List<string> Csvdosyabasliklar)
        {
            if (!File.Exists(dosyayolu))
            {
                File.WriteAllText(dosyayolu, string.Join(",", Csvdosyabasliklar) + Environment.NewLine, Encoding.UTF8);

            }
            else
            {
                try
                {
                    using (StreamReader CsvDosyasi = new StreamReader(dosyayolu))
                    {
                        string ilksatir = CsvDosyasi.ReadLine();
                        if (string.IsNullOrWhiteSpace(ilksatir))
                        {
                            File.WriteAllText(dosyayolu, string.Join(",", Csvdosyabasliklar) + Environment.NewLine, Encoding.UTF8);
                        }
                    }
                }
                catch (Exception)// csv dosyası okunamazsa log kaydı ekle
                {

                    throw;
                }
               
            }

        }
        public static async void CsvDeneme(string path, List<string> headers) //csv dosyasına yazma işlemi
        {
            CsvDosyasiİlkSatirKontrolu(path, headers);
            var culture = CultureInfo.InvariantCulture;

            while (true)
            {
                
                string Rows = string.Join(",",
                DenemeVerileriCsv.paketnumarasi,
                DenemeVerileriCsv.uydustatusu,
                DenemeVerileriCsv.hatakodu,                
                DenemeVerileriCsv.denemegondermesaati,
                DenemeVerileriCsv.basinc1.ToString(culture),
                DenemeVerileriCsv.basinc2.ToString(culture),
                DenemeVerileriCsv.yukseklik1.ToString(culture),
                DenemeVerileriCsv.yukseklik2.ToString(culture),
                DenemeVerileriCsv.irtifafarkı.ToString(culture),
                DenemeVerileriCsv.inishizi.ToString(culture),
                DenemeVerileriCsv.pilgerilimi.ToString(culture),
                DenemeVerileriCsv.altıtude.ToString(culture),
                DenemeVerileriCsv.Latitude.ToString(culture),
                DenemeVerileriCsv.Longitude.ToString(culture),
                DenemeVerileriCsv.sicaklik.ToString(culture),
                DenemeVerileriCsv.pitch,
                DenemeVerileriCsv.roll,
                DenemeVerileriCsv.yaw,
                DenemeVerileriCsv.rhrh,
                DenemeVerileriCsv.iot,
                DenemeVerileriCsv.takimno
                );
                DenemeVerileriCsv.TestMethodu();
                File.AppendAllText(path, Rows + Environment.NewLine);

                await Task.Delay(1000);

            }
        }
    }
}
