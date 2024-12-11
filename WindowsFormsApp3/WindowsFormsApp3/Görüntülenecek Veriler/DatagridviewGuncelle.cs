using Aerospace_GroundStation.veriler;
using Emgu.CV.Features2D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp3.Csv;

namespace WindowsFormsApp3.Görüntülenecek_Veriler
{
    public static class DatagridviewGuncelle
    {
        public static void Guncelle(DataGridView dataGridView,AlinacakVeriler alinacakVeriler,GonderilecekVeriler gonderilecekVeriler)
        {

            try
            {
                dataGridView.Rows.Add(
                alinacakVeriler.PaketNumarasi,
                alinacakVeriler.UyduStatusu,
                alinacakVeriler.HataKodu,
                alinacakVeriler.GondermeSaati,
                alinacakVeriler.Basinc1,
                alinacakVeriler.Basinc2,
                alinacakVeriler.Yukseklik1,
                alinacakVeriler.Yukseklik2,
                alinacakVeriler.İrtifaFarki,
                alinacakVeriler.İnisHizi,
                alinacakVeriler.Sicaklik,
                alinacakVeriler.PilGerilimi,
                alinacakVeriler.GPS1Latitude,
                alinacakVeriler.GPS1Longitude,
                alinacakVeriler.GPS1Altitude,
                alinacakVeriler.Pitch, alinacakVeriler.Roll,
                alinacakVeriler.Yaw,
                gonderilecekVeriler.RHRH,
                gonderilecekVeriler.IOTData,
                alinacakVeriler.TakimNo
                );
                
            }
            catch (Exception)
            {

                throw;// datagridviewe veri eklenirken sorun çıkarsa
            }
            
        }
        public static void DenemeGuncellesi(DataGridView dataGridView)
        {

            try
            {
                dataGridView.Rows.Add(
                    DenemeVerileriCsv.paketnumarasi,
                    DenemeVerileriCsv.uydustatusu,
                    DenemeVerileriCsv.hatakodu,
                    DenemeVerileriCsv.denemegondermesaati,
                    DenemeVerileriCsv.basinc1,
                    DenemeVerileriCsv.basinc2,
                    DenemeVerileriCsv.yukseklik1,
                    DenemeVerileriCsv.yukseklik2,
                    DenemeVerileriCsv.irtifafarkı,
                    DenemeVerileriCsv.inishizi,
                    DenemeVerileriCsv.sicaklik,
                    DenemeVerileriCsv.pilgerilimi,
                    DenemeVerileriCsv.Latitude,
                    DenemeVerileriCsv.Longitude,
                    DenemeVerileriCsv.altıtude,
                    DenemeVerileriCsv.pitch,
                    DenemeVerileriCsv.roll,
                    DenemeVerileriCsv.yaw,
                    DenemeVerileriCsv.rhrh,
                    DenemeVerileriCsv.iot,
                    DenemeVerileriCsv.takimno                
                );

            }
            catch (Exception)
            {

                throw;// datagridviewe veri eklenirken sorun çıkarsa
            }

        }
    }
}
