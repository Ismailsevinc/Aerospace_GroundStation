using Emgu.CV;
using Emgu.CV.Reg;
using GMap.NET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using System.Threading;
using System.Security.Cryptography.X509Certificates;

using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using Aerospace_GroundStation.veriler;
using System.IO;
using System.IO.Ports;

using DevExpress.XtraCharts;
using DevExpress.Charts.Native;
using System.Data.Entity.Infrastructure;
using static GMap.NET.Entity.OpenStreetMapGraphHopperRouteEntity;
using WindowsFormsApp3.Csv;
using DevExpress.Utils;
using WindowsFormsApp3.Görüntülenecek_Veriler;


namespace Aerospace_GroundStation
{
    public partial class Form1 : Form
    {

        float x = 0, y = 0, z = 0; // pitch roll ve yaw değerleri
        SerialPort serialport;

        double altıtude = 35.1214;
        double Latitude = 39.9208;
        double Longitude = 32.8541;
        double sicaklik = 35.3;
        double yukseklik = 250;

       
        private double time = 0; // Zamanı takip etmek için
        string CsvDosyaYolu = "C:\\Users\\ismai\\OneDrive\\Masaüstü\\veriler.csv";//csv dosya yolu
        

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)//seri porttan gelecek verileri alma
        {
            var Veriler = serialPort1.ReadLine();
            AlinacakVeriler SonVeriler = VeriDonusumu(Veriler);


            this.BeginInvoke(new Action(() =>
            {
                string dateTime = DateTime.Now.ToString("HH:mm:ss");
                ChartGrafikGüncelleme(dateTime, SonVeriler.Sicaklik, chartcontrolSİCAKLİK, "Sicaklik");
                ChartGrafikGüncelleme(dateTime, SonVeriler.Yukseklik1, chartControlYUKSEKLİK, "Yukseklik1");
                ChartGrafikGüncelleme(dateTime, SonVeriler.Yukseklik2, chartControlYUKSEKLİK, "Yukseklik2");
                ChartGrafikGüncelleme(dateTime, SonVeriler.GPS1Altitude, chartControlGPSALTITUDE, "Gpsaltitude");
                ChartGrafikGüncelleme(dateTime, SonVeriler.Basinc1, chartControlBASİNC, "Basinc1");
                ChartGrafikGüncelleme(dateTime, SonVeriler.Basinc2, chartControlBASİNC, "Basinc2");
                ChartGrafikGüncelleme(dateTime, SonVeriler.PilGerilimi, chartControlPİLGERİLİMİ, "Pilgerilimi");
                ChartGrafikGüncelleme(dateTime, SonVeriler.İnisHizi, chartControlİNİSHİZİ, "İnishizi");
                labelROLL.Text = SonVeriler.Roll.ToString();
                labelYAW.Text = SonVeriler.Yaw.ToString();
                labelPİTCH.Text = SonVeriler.Pitch.ToString();
                labelGONDERMESAATİ.Text = SonVeriler.GondermeSaati.ToString();
            }
            ));

        }
        private AlinacakVeriler VeriDonusumu(string Veriler)
        {
            string[] VeriDizisi = Veriler.Split(';');

            // Güvenli dönüşüm 
            int.TryParse(GetValue(VeriDizisi, 0), out int paketNumarasi);
            int.TryParse(GetValue(VeriDizisi, 1), out int uyduStatusu);
            int.TryParse(GetValue(VeriDizisi, 2), out int hataKodu);
            long.TryParse(GetValue(VeriDizisi, 3), out long gondermeSaati);
            float.TryParse(GetValue(VeriDizisi, 4), out float basinc1);
            float.TryParse(GetValue(VeriDizisi, 5), out float basinc2);
            float.TryParse(GetValue(VeriDizisi, 6), out float yukseklik1);
            float.TryParse(GetValue(VeriDizisi, 7), out float yukseklik2);
            float.TryParse(GetValue(VeriDizisi, 8), out float irtifaFarki);
            double.TryParse(GetValue(VeriDizisi, 9), out double inisHizi);
            double.TryParse(GetValue(VeriDizisi, 10), out double sicaklik);
            int.TryParse(GetValue(VeriDizisi, 11), out int pilGerilimi);
            float.TryParse(GetValue(VeriDizisi, 12), out float gps1Latitude);
            float.TryParse(GetValue(VeriDizisi, 13), out float gps1Longitude);
            float.TryParse(GetValue(VeriDizisi, 14), out float gps1Altitude);
            int.TryParse(GetValue(VeriDizisi, 15), out int pitch);
            int.TryParse(GetValue(VeriDizisi, 16), out int roll);
            int.TryParse(GetValue(VeriDizisi, 17), out int yaw);

            // Sonuç sınıfını oluştur ve geri döndür
            return new AlinacakVeriler
            {
                PaketNumarasi = paketNumarasi,
                UyduStatusu = uyduStatusu,
                HataKodu = hataKodu,
                GondermeSaati = gondermeSaati,
                Basinc1 = basinc1,
                Basinc2 = basinc2,
                Yukseklik1 = yukseklik1,
                Yukseklik2 = yukseklik2,
                İrtifaFarki = irtifaFarki,
                İnisHizi = inisHizi,
                Sicaklik = sicaklik,
                PilGerilimi = pilGerilimi,
                GPS1Latitude = gps1Latitude,
                GPS1Longitude = gps1Longitude,
                GPS1Altitude = gps1Altitude,
                Pitch = pitch,
                Roll = roll,
                Yaw = yaw,
            };
        }

        // Dizi sınırlarını kontrol eden yardımcı metot
        private string GetValue(string[] veriDizisi, int index)
        {
            return index < veriDizisi.Length ? veriDizisi[index] : null;
        }
        public Form1()
        {
            InitializeComponent();
            İnitializeGmap(); // Konum Bilgisini Getirir
            PitchRollYaw();
            ComboboxPORTandBAUDRATE();
        }
        private void PitchRollYaw()
        {
            labelPİTCH.Text = $"PITCH:  {x}";
            labelROLL.Text = $"ROLL:  {y}";
            labelYAW.Text = $"YAW:  {z}";
        }
        private async void Form1_Load(object sender, EventArgs e)
        {
            GL.ClearColor(Color.Gray);//Color.FromArgb(143, 212, 150)

            this.WindowState = FormWindowState.Maximized; // Uygulama tam ekran olur
            // Arayüz Alarm Sistemi 
            arasuyduinishizilabel1.BackColor = Color.Green;
            arasgorevyukuinishizilabel2.BackColor = Color.Green;
            arasbasincverisilabel3.BackColor = Color.Green;
            arasgorevyukukonumlabel4.BackColor = Color.Green;
            arasayrılmadurumulabel5.BackColor = Color.Green;
            panelUCUSAHAZİR.BackColor = Color.Green;


            ChartZamanAyari(chartcontrolSİCAKLİK);
            ChartZamanAyari(chartControlYUKSEKLİK);
            ChartZamanAyari(chartControlGPSALTITUDE);
            ChartZamanAyari(chartControlİNİSHİZİ);
            ChartZamanAyari(chartControlBASİNC);
            ChartZamanAyari(chartControlPİLGERİLİMİ);
            AlinacakVeriler SonVeriler = new AlinacakVeriler();
            SonVeriler.Sicaklik = 35.4;
            SonVeriler.Yukseklik1 = 500.34f;
            SonVeriler.PilGerilimi = 10;
            SonVeriler.GPS1Altitude = 100.3434f;
            SonVeriler.Basinc1 = 1044.4f;
            SonVeriler.İnisHizi = 34.5;
            SonVeriler.Yukseklik2 = 300.23f;
            SonVeriler.Basinc2 = 3040.54f;

            while (true)//  DENEME DENEME DENEME
            {

                SonVeriler.Yukseklik1 += 10;
                SonVeriler.Yukseklik2 += 25;

                SonVeriler.Sicaklik += 5;
                SonVeriler.PilGerilimi += 1;
                SonVeriler.GPS1Altitude += 10f;
                SonVeriler.Basinc1 += 15f;
                SonVeriler.İnisHizi += 3;
                SonVeriler.Basinc2 -= 25;
                x++;
                y++;
                z++;
                this.BeginInvoke(new Action(() =>
                {
                    glControl1.Invalidate();// 3d görüntüyü pitch roll ve yaw her değiştiğinde çizdirir
                    labelPİTCH.Text = $"PITCH:{x}";
                    labelROLL.Text = $"ROLL:{y}";
                    labelYAW.Text = $"YAW:{z}";
                    string dateTime = DateTime.Now.ToString("HH:mm:ss");
                    DatagridviewGuncelle.DenemeGuncellesi(dataGridView1);
                    DenemeVerileriCsv.TestMethodu();
                    ChartGrafikGüncelleme(dateTime, SonVeriler.Sicaklik, chartcontrolSİCAKLİK, "Sicaklik");
                    ChartGrafikGüncelleme(dateTime, SonVeriler.Yukseklik1, chartControlYUKSEKLİK, "Yukseklik1");
                    ChartGrafikGüncelleme(dateTime, SonVeriler.Yukseklik2, chartControlYUKSEKLİK, "Yukseklik2");
                    ChartGrafikGüncelleme(dateTime, SonVeriler.GPS1Altitude, chartControlGPSALTITUDE, "Gpsaltitude");
                    ChartGrafikGüncelleme(dateTime, SonVeriler.Basinc1, chartControlBASİNC, "Basinc1");
                    ChartGrafikGüncelleme(dateTime, SonVeriler.Basinc2, chartControlBASİNC, "Basinc2");
                    ChartGrafikGüncelleme(dateTime, SonVeriler.PilGerilimi, chartControlPİLGERİLİMİ, "Pilgerilimi");
                    ChartGrafikGüncelleme(dateTime, SonVeriler.İnisHizi, chartControlİNİSHİZİ, "İnishizi");

                }
           ));

                await Task.Delay(1000);

            }
        }


        private void UyduStatusuChanges(int uydustatusu)
        {

        }

        private void ArayuzAlarmSistemi(double ModelUyduinishizi, double GorevYukuinishizi, float Tasiyicibasinc, float Gorevyukukonum, bool Ayrılmadurumu)
        {

        }
        private void ChartGrafikGüncelleme(string dateTime, object SonVeriler, ChartControl chartcontrol, string Series) //grafikleri tek bir method ile güncelleme
        {
            try
            {

                chartcontrol.Series[Series].Points.Add(new SeriesPoint(dateTime, SonVeriler));
                if (chartcontrol.Series[Series].Points.Count > 4)
                {
                    chartcontrol.Series[Series].Points.RemoveAt(0);
                }

            }
            catch (Exception)
            {

                throw;//hata oluşursa
            }
        }


        private void ChartZamanAyari(ChartControl chartControl)
        {
            XYDiagram diagram = (XYDiagram)chartControl.Diagram;
            diagram.AxisX.DateTimeScaleOptions.MeasureUnit = DateTimeMeasureUnit.Second;
            diagram.AxisX.DateTimeScaleOptions.GridAlignment = DateTimeGridAlignment.Second;
            diagram.AxisX.Label.TextPattern = "{A:HH:mm:ss}";


        }

        private void gmapKONUM_Load(object sender, EventArgs e)
        {

        }

        private void İnitializeGmap()
        {
            // Konum bilgisini GMAP kullanarak göstermek
            gmapKONUM.DragButton = MouseButtons.Left;
            gmapKONUM.MapProvider = GMapProviders.GoogleMap;
            gmapKONUM.Position = new GMap.NET.PointLatLng(Latitude, Longitude);
            gmapKONUM.MaxZoom = 18;
            gmapKONUM.MinZoom = 10;
            gmapKONUM.Zoom = 18;
            labelGPSALTITUDE.Text = $"GPS LATITUDE:{Latitude}";
            labelGPSLONGITUDE.Text = $"GPS LONGTITUDE:{Longitude}";
        }

        private void gmapKONUM_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void gmapKONUM_OnMapClick(PointLatLng point, MouseEventArgs e)
        {
            // Tıklanan konumu label'lara yazdır
            labelGPSALTITUDE.Text = $"GPS LATITUDE:{point.Lat:F6}";  // Latitude değeri
            labelGPSLONGITUDE.Text = $"GPS LONGTITUDE:{point.Lng:F6}"; // Longitude değeri
        }

        private void buttonayrılmakomutu_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Taşıyıcıyı Görev Yükünden Ayırmak İstiyor Musunuz?", "Evet", MessageBoxButtons.YesNo, MessageBoxIcon.Question);//ayrılma komutu ayarları
            if (result == DialogResult.Yes)
            {
                // Taşıyıcıyı görev yükünden ayırma kodlarını buraya yaz
            }
            else
            {
                return;
            }
        }

        private void button1_Click(object sender, EventArgs e) //mekanik filtreleme modülü
        {
            string Filter = textBoxMEKANİKFİLTRELEME.Text;
            char[] charArray = Filter.ToCharArray();
            int index1 = (int)char.GetNumericValue(charArray[0]);
            int index3 = (int)char.GetNumericValue(charArray[2]);

            //burda mekanik filtreleme modülüne girilenlerin sözcük sözcük kontrolü yapılıyor gerek varsa açın
            if (!char.IsLetter(charArray[1]) || !char.IsLetter(charArray[3]) || (index1 + index3 != 10))

            {
                MessageBox.Show("uygunsuz");
            }
            else
            {
                MessageBox.Show("uygun");
            }


            if (!string.IsNullOrWhiteSpace(textBoxMEKANİKFİLTRELEME.Text))
            {
                DialogResult result = MessageBox.Show($"{textBoxMEKANİKFİLTRELEME.Text} Filtresini Göndermek İstediğinizden Emin Misiniz?", "Evet", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    //mekanik filtreleme modülünden gönderilecek değer için yapılacak işlemler
                }
                else
                {
                    return;
                }
            }
            else
            {
                MessageBox.Show("Geçersiz Değer");
            }

        }


        private void ComboboxPORTandBAUDRATE()//bağlı seri port ve baudrate değerlerini comboboxlara yazar
        {
            string[] Portname = SerialPort.GetPortNames();
            if (Portname is null)
            {

            }
            else
            {
                foreach (var item in Portname)
                {
                    comboBoxSERİALPORT.Items.Add(item);
                }
            }


        }

        private void glControl1_Paint_1(object sender, PaintEventArgs e)
        {
            float step = 1.0f;
            float topla = step;
            float radius = 5.0f;
            float dikey1 = radius, dikey2 = -radius;
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Clear(ClearBufferMask.DepthBufferBit);

            Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView(1.04f, 4 / 3, 1, 10000);
            Matrix4 lookat = Matrix4.LookAt(25, 0, 0, 0, 0, 0, 0, 1, 0);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.LoadMatrix(ref perspective);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.LoadMatrix(ref lookat);
            GL.Viewport(0, 0, glControl1.Width, glControl1.Height);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);

            GL.Rotate(x, 1.0, 0.0, 0.0);//ÖNEMLİ
            GL.Rotate(z, 0.0, 1.0, 0.0);
            GL.Rotate(y, 0.0, 0.0, 1.0);

            silindir(step, topla, radius, 3, -5);
            silindir(0.01f, topla, 0.5f, 9, 9.7f);
            silindir(0.01f, topla, 0.1f, 5, dikey1 + 5);
            koni(0.01f, 0.01f, radius, 3.0f, 3, 5);
            koni(0.01f, 0.01f, radius, 2.0f, -5.0f, -10.0f);
            Pervane(9.0f, 11.0f, 0.2f, 0.5f);

            GL.Begin(BeginMode.Lines);

            GL.Color3(Color.FromArgb(0, 0, 0));
            GL.Vertex3(-30.0, 0.0, 0.0);
            GL.Vertex3(30.0, 0.0, 0.0);


            GL.Color3(Color.FromArgb(0, 0, 0));
            GL.Vertex3(0.0, 30.0, 0.0);
            GL.Vertex3(0.0, -30.0, 0.0);

            GL.Color3(Color.FromArgb(0, 0, 0));
            GL.Vertex3(0.0, 0.0, 30.0);
            GL.Vertex3(0.0, 0.0, -30.0);

            GL.End();
            //GraphicsContext.CurrentContext.VSync = true;
            glControl1.SwapBuffers();
        }

        private void glControl1_Load_1(object sender, EventArgs e)
        {
            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            GL.Enable(EnableCap.DepthTest);//sonradan yazdık
        }


        private void silindir(float step, float topla, float radius, float dikey1, float dikey2)
        {
            float eski_step = 0.1f;
            GL.Begin(BeginMode.Quads);//Y EKSEN CIZIM DAİRENİN
            while (step <= 360)
            {
                if (step < 45)
                    GL.Color3(Color.FromArgb(0, 0, 0));
                else if (step < 90)
                    GL.Color3(Color.FromArgb(255, 255, 255));
                else if (step < 135)
                    GL.Color3(Color.FromArgb(0, 0, 0));
                else if (step < 180)
                    GL.Color3(Color.FromArgb(255, 255, 255));
                else if (step < 225)
                    GL.Color3(Color.FromArgb(0, 0, 0));
                else if (step < 270)
                    GL.Color3(Color.FromArgb(255, 255, 255));
                else if (step < 315)
                    GL.Color3(Color.FromArgb(0, 0, 0));
                else if (step < 360)
                    GL.Color3(Color.FromArgb(255, 255, 255));


                float ciz1_x = (float)(radius * Math.Cos(step * Math.PI / 180F));
                float ciz1_y = (float)(radius * Math.Sin(step * Math.PI / 180F));
                GL.Vertex3(ciz1_x, dikey1, ciz1_y);

                float ciz2_x = (float)(radius * Math.Cos((step + 2) * Math.PI / 180F));
                float ciz2_y = (float)(radius * Math.Sin((step + 2) * Math.PI / 180F));
                GL.Vertex3(ciz2_x, dikey1, ciz2_y);

                GL.Vertex3(ciz1_x, dikey2, ciz1_y);
                GL.Vertex3(ciz2_x, dikey2, ciz2_y);
                step += topla;
            }
            GL.End();
            GL.Begin(BeginMode.Lines);
            step = eski_step;
            topla = step;
            while (step <= 180)// UST KAPAK
            {
                if (step < 45)
                    GL.Color3(Color.FromArgb(1, 1, 1));
                else if (step < 90)
                    GL.Color3(Color.FromArgb(250, 250, 200));
                else if (step < 135)
                    GL.Color3(Color.FromArgb(1, 1, 1));
                else if (step < 180)
                    GL.Color3(Color.FromArgb(250, 250, 200));
                else if (step < 225)
                    GL.Color3(Color.FromArgb(1, 1, 1));
                else if (step < 270)
                    GL.Color3(Color.FromArgb(250, 250, 200));
                else if (step < 315)
                    GL.Color3(Color.FromArgb(1, 1, 1));
                else if (step < 360)
                    GL.Color3(Color.FromArgb(250, 250, 200));


                float ciz1_x = (float)(radius * Math.Cos(step * Math.PI / 180F));
                float ciz1_y = (float)(radius * Math.Sin(step * Math.PI / 180F));
                GL.Vertex3(ciz1_x, dikey1, ciz1_y);

                float ciz2_x = (float)(radius * Math.Cos((step + 180) * Math.PI / 180F));
                float ciz2_y = (float)(radius * Math.Sin((step + 180) * Math.PI / 180F));
                GL.Vertex3(ciz2_x, dikey1, ciz2_y);

                GL.Vertex3(ciz1_x, dikey1, ciz1_y);
                GL.Vertex3(ciz2_x, dikey1, ciz2_y);
                step += topla;
            }
            step = eski_step;
            topla = step;
            while (step <= 180)//ALT KAPAK
            {
                if (step < 45)
                    GL.Color3(Color.FromArgb(1, 1, 1));
                else if (step < 90)
                    GL.Color3(Color.FromArgb(250, 250, 200));
                else if (step < 135)
                    GL.Color3(Color.FromArgb(1, 1, 1));
                else if (step < 180)
                    GL.Color3(Color.FromArgb(250, 250, 200));
                else if (step < 225)
                    GL.Color3(Color.FromArgb(1, 1, 1));
                else if (step < 270)
                    GL.Color3(Color.FromArgb(250, 250, 200));
                else if (step < 315)
                    GL.Color3(Color.FromArgb(1, 1, 1));
                else if (step < 360)
                    GL.Color3(Color.FromArgb(250, 250, 200));

                float ciz1_x = (float)(radius * Math.Cos(step * Math.PI / 180F));
                float ciz1_y = (float)(radius * Math.Sin(step * Math.PI / 180F));
                GL.Vertex3(ciz1_x, dikey2, ciz1_y);

                float ciz2_x = (float)(radius * Math.Cos((step + 180) * Math.PI / 180F));
                float ciz2_y = (float)(radius * Math.Sin((step + 180) * Math.PI / 180F));
                GL.Vertex3(ciz2_x, dikey2, ciz2_y);

                GL.Vertex3(ciz1_x, dikey2, ciz1_y);
                GL.Vertex3(ciz2_x, dikey2, ciz2_y);
                step += topla;
            }
            GL.End();
        }

        private void buttonBASLA_Click(object sender, EventArgs e)
        {
            if (comboBoxSERİALPORT.Text == "" && comboBoxBAUDRATE.Text == "")
            {
                MessageBox.Show("geçersiz port ve baudrate");
            }
            else
            {

                //serialport.Open();
                //seri porttan bağlandığında yapılacaklar
                // SerialportConfigure.SerialPortOptions(serialport, comboBoxSERİALPORT, comboBoxBAUDRATE);//seri port ayarları yapmaya yarar

            }
        }


        private void buttonDUR_Click(object sender, EventArgs e) //programı durdurmak için yapılacaklar
        {

        }

        private void buttonVİDEOBASLAT_Click(object sender, EventArgs e)
        {
            CsvKontrol.CsvDeneme(CsvDosyaYolu, CsvDosyaBasliklar.CsvDosyaBasliklari);
        }

        private void koni(float step, float topla, float radius1, float radius2, float dikey1, float dikey2)
        {
            float eski_step = 0.1f;
            GL.Begin(BeginMode.Lines);//Y EKSEN CIZIM DAİRENİN
            while (step <= 360)
            {
                if (step < 45)
                    GL.Color3(1.0, 1.0, 1.0);
                else if (step < 90)
                    GL.Color3(0.0, 0.0, 0.0);
                else if (step < 135)
                    GL.Color3(1.0, 1.0, 1.0);
                else if (step < 180)
                    GL.Color3(0.0, 0.0, 0.0);
                else if (step < 225)
                    GL.Color3(1.0, 1.0, 1.0);
                else if (step < 270)
                    GL.Color3(0.0, 0.0, 0.0);
                else if (step < 315)
                    GL.Color3(1.0, 1.0, 1.0);
                else if (step < 360)
                    GL.Color3(0.0, 0.0, 0.0);


                float ciz1_x = (float)(radius1 * Math.Cos(step * Math.PI / 180F));
                float ciz1_y = (float)(radius1 * Math.Sin(step * Math.PI / 180F));
                GL.Vertex3(ciz1_x, dikey1, ciz1_y);

                float ciz2_x = (float)(radius2 * Math.Cos(step * Math.PI / 180F));
                float ciz2_y = (float)(radius2 * Math.Sin(step * Math.PI / 180F));
                GL.Vertex3(ciz2_x, dikey2, ciz2_y);
                step += topla;
            }
            GL.End();

            GL.Begin(BeginMode.Lines);
            step = eski_step;
            topla = step;
            while (step <= 180)// UST KAPAK
            {
                if (step < 45)
                    GL.Color3(Color.FromArgb(0, 0, 0));
                else if (step < 90)
                    GL.Color3(Color.FromArgb(250, 250, 200));
                else if (step < 135)
                    GL.Color3(Color.FromArgb(0, 0, 0));
                else if (step < 180)
                    GL.Color3(Color.FromArgb(250, 250, 200));
                else if (step < 225)
                    GL.Color3(Color.FromArgb(0, 0, 0));
                else if (step < 270)
                    GL.Color3(Color.FromArgb(250, 250, 200));
                else if (step < 315)
                    GL.Color3(Color.FromArgb(1, 1, 1));
                else if (step < 360)
                    GL.Color3(Color.FromArgb(250, 250, 200));


                float ciz1_x = (float)(radius2 * Math.Cos(step * Math.PI / 180F));
                float ciz1_y = (float)(radius2 * Math.Sin(step * Math.PI / 180F));
                GL.Vertex3(ciz1_x, dikey2, ciz1_y);

                float ciz2_x = (float)(radius2 * Math.Cos((step + 180) * Math.PI / 180F));
                float ciz2_y = (float)(radius2 * Math.Sin((step + 180) * Math.PI / 180F));
                GL.Vertex3(ciz2_x, dikey2, ciz2_y);

                GL.Vertex3(ciz1_x, dikey2, ciz1_y);
                GL.Vertex3(ciz2_x, dikey2, ciz2_y);
                step += topla;
            }
            step = eski_step;
            topla = step;
            GL.End();
        }
        private void Pervane(float yukseklik, float uzunluk, float kalinlik, float egiklik)
        {
            float radius = 10, angle = 45.0f;
            GL.Begin(BeginMode.Quads);

            GL.Color3(Color.Black);
            GL.Vertex3(uzunluk, yukseklik, kalinlik);
            GL.Vertex3(uzunluk, yukseklik + egiklik, -kalinlik);
            GL.Vertex3(0.0, yukseklik + egiklik, -kalinlik);
            GL.Vertex3(0.0, yukseklik, kalinlik);

            GL.Color3(Color.Black);
            GL.Vertex3(-uzunluk, yukseklik + egiklik, kalinlik);
            GL.Vertex3(-uzunluk, yukseklik, -kalinlik);
            GL.Vertex3(0.0, yukseklik, -kalinlik);
            GL.Vertex3(0.0, yukseklik + egiklik, kalinlik);

            GL.Color3(Color.White);
            GL.Vertex3(kalinlik, yukseklik, -uzunluk);
            GL.Vertex3(-kalinlik, yukseklik + egiklik, -uzunluk);
            GL.Vertex3(-kalinlik, yukseklik + egiklik, 0.0);//+
            GL.Vertex3(kalinlik, yukseklik, 0.0);//-

            GL.Color3(Color.White);
            GL.Vertex3(kalinlik, yukseklik + egiklik, +uzunluk);
            GL.Vertex3(-kalinlik, yukseklik, +uzunluk);
            GL.Vertex3(-kalinlik, yukseklik, 0.0);
            GL.Vertex3(kalinlik, yukseklik + egiklik, 0.0);
            GL.End();

        }
    }
}
