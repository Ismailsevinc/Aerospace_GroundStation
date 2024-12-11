using DevExpress.XtraCharts.Native;
using DevExpress.XtraRichEdit.Export.Rtf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp3.Csv
{
    public static class DenemeVerileriCsv
    {
        public static int paketnumarasi = 1;
        public static string hatakodu = "00000";
        public static int uydustatusu = 2;
        public static string denemegondermesaati = DateTime.Now.ToString("HH:mm:ss");
        public static double basinc1 = 1212.122;
        public static double basinc2 = 333.222;
        public static double yukseklik1 = 23.1;
        public static double yukseklik2 = 1212.42;
        public static double irtifafarkı = 14.4;
        public static double inishizi = 15.5;
        public static double pilgerilimi = 6.6;
        public static float altıtude = 35.1214f;
        public static float Latitude = 39.9208f;
        public static float Longitude = 32.8541f;
        public static double sicaklik = 35.3;
        public static double yukseklik = 250.6;
        public static int pitch = 23;
        public static int roll = 14;
        public static int yaw = 280;
        public static string rhrh = "4t6y";
        public static string iot = "35C %26";
        public static int takimno = 1243151;
        public static void TestMethodu()
        {
            
                paketnumarasi++;
                Random random = new Random();
                
                hatakodu = $"{random.Next(0, 2)}{random.Next(0, 2)}{random.Next(0, 2)}{random.Next(0, 2)}{random.Next(0, 2)}";
                uydustatusu++;
                denemegondermesaati = DateTime.Now.ToString("HH:mm:ss");
                basinc1++;
                basinc2++;
                yukseklik1++;
                yukseklik2++;
                irtifafarkı++;
                inishizi++;
                pilgerilimi++;
                altıtude++;
                Latitude++;
                Longitude++;
                sicaklik++;
                pitch++;
                roll++;
                yaw++;
                iot = $"{random.Next(0,45)}C %{random.Next(0,101)}";

            
        }
    }
}
