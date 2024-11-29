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
using OxyPlot.Axes;
using Aerospace_GroundStation.veriler;
using System.IO;
using System.IO.Ports;

using DevExpress.XtraCharts;
using DevExpress.Charts.Native;

namespace Aerospace_GroundStation
{
    public partial class Form1 : Form
    {

        float x = 0, y = 0, z = 0; // pitch roll ve yaw değerleri
        SerialPort serialport;

        double Latitude = 39.9208;
        double Longitude = 32.8541;
        double sicaklik = 35.3;
        double yukseklik = 250;
        private double time = 0; // Zamanı takip etmek için

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)//seri porttan gelecek verileri alma
        {
            var Veriler = serialPort1.ReadLine();
            AlinacakVeriler SonVeriler = VeriDonusumu(Veriler);


            this.BeginInvoke(new Action(() =>
            {
                string dateTime = DateTime.Now.ToString("HH:mm:ss");
                try
                {
                    
                    chartcontrolSİCAKLİK.Series["Sicaklik"].Points.Add(new SeriesPoint(dateTime,SonVeriler.Sicaklik));//sicaklik yazan parametre telemetri ile alınacak
                    if (chartcontrolSİCAKLİK.Series["Sicaklik"].Points.Count > 4)
                    {
                        chartcontrolSİCAKLİK.Series["Sicaklik"].Points.RemoveAt(0);
                    }
                }
                catch (Exception)
                {

                    throw;//hata oluşursa
                }
                try
                {

                    chartControlYUKSEKLİK.Series["Yukseklik"].Points.Add(new SeriesPoint(dateTime, SonVeriler.Yukseklik1));//sicaklik yazan parametre telemetri ile alınacak
                    if (chartControlYUKSEKLİK.Series["Yukseklik"].Points.Count > 4)
                    {
                        chartControlYUKSEKLİK.Series["Yukseklik"].Points.RemoveAt(0);
                    }
                }
                catch (Exception)
                {

                    throw;//hata oluşursa
                }
                try
                {

                    chartControlYUKSEKLİK.Series["Gpsaltitude"].Points.Add(new SeriesPoint(dateTime, SonVeriler.GPS1Altitude));//sicaklik yazan parametre telemetri ile alınacak
                    if (chartControlYUKSEKLİK.Series["Gpsaltitude"].Points.Count > 4)
                    {
                        chartControlYUKSEKLİK.Series["Gpsaltitude"].Points.RemoveAt(0);
                    }
                }
                catch (Exception)
                {

                    throw;//hata oluşursa
                }
                

            }
            ));

        }
        private AlinacakVeriler VeriDonusumu(string Veriler) //Seri porttan gelen verilerin bir class yardımı ile bir arada toplanması
        {

            string[] VeriDizisi = Veriler.Split(';');
            return new AlinacakVeriler
            {
                PaketNumarasi = int.Parse(VeriDizisi[0]),
                UyduStatusu = int.Parse(VeriDizisi[1]),
                HataKodu = int.Parse(VeriDizisi[2]),
                GondermeSaati = long.Parse(VeriDizisi[3]),
                Basinc1 = float.Parse(VeriDizisi[4]),
                Basinc2 = float.Parse(VeriDizisi[5]),
                Yukseklik1 = float.Parse(VeriDizisi[6]),
                Yukseklik2 = float.Parse(VeriDizisi[7]),
                İrtifaFarki = float.Parse(VeriDizisi[8]),
                İnisHizi = double.Parse(VeriDizisi[9]),
                Sicaklik = double.Parse(VeriDizisi[10]),
                PilGerilimi = int.Parse(VeriDizisi[11]),
                GPS1Latitude = float.Parse(VeriDizisi[12]),
                GPS1Longitude = float.Parse(VeriDizisi[13]),
                GPS1Altitude = float.Parse(VeriDizisi[14]),
                Pitch = int.Parse(VeriDizisi[15]),
                Roll = int.Parse(VeriDizisi[16]),
                Yaw = int.Parse(VeriDizisi[17]),
            };

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

            try
            {
                ChartZamanAyari(chartcontrolSİCAKLİK);
            }
            catch (Exception)
            {

                throw;
            }
            try
            {
                ChartZamanAyari(chartControlYUKSEKLİK);
            }
            catch (Exception)
            {

                throw;
            }
            try
            {
                ChartZamanAyari(chartControlGPSALTITUDE);
            }
            catch (Exception)
            {

                throw;
            }
            while (true)//  DENEME DENEME DENEME
            {
                sicaklik++;
                yukseklik += 10 ;
                this.BeginInvoke(new Action(() =>
                {
                    string dateTime = DateTime.Now.ToString("HH:mm:ss");
                    try
                    {

                        chartcontrolSİCAKLİK.Series["Sicaklik"].Points.Add(new SeriesPoint(dateTime, sicaklik));//sicaklik yazan parametre telemetri ile alınacak
                        if (chartcontrolSİCAKLİK.Series["Sicaklik"].Points.Count > 4)
                        {
                            chartcontrolSİCAKLİK.Series["Sicaklik"].Points.RemoveAt(0);
                        }
                    }
                    catch (Exception)
                    {

                        throw;//hata oluşursa
                    }
                    try
                    {

                        chartControlYUKSEKLİK.Series["Yukseklik"].Points.Add(new SeriesPoint(dateTime,yukseklik));//sicaklik yazan parametre telemetri ile alınacak
                        if (chartControlYUKSEKLİK.Series["Yukseklik"].Points.Count > 4)
                        {
                            chartControlYUKSEKLİK.Series["Yukseklik"].Points.RemoveAt(0);
                        }
                    }
                    catch (Exception)
                    {

                        throw;//hata oluşursa
                    }

                }
           ));

                await Task.Delay(1000);
                
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
            gmapKONUM.MapProvider = GMapProviders.OpenStreetMap;
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
            DialogResult result = MessageBox.Show("Taşıyıcıyı Görev Yükünden Ayırmak İstiyor Musunuz?", "Evet", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                // Taşıyıcıyı görev yükünden ayırma kodlarını buraya yaz
            }
            else
            {
                return;
            }
        }

        private void button1_Click(object sender, EventArgs e) //mekanil filtreleme modülü
        {
            string Filter = textBoxMEKANİKFİLTRELEME.Text;
            char[] charArray = Filter.ToCharArray();

            //burda mekanik filtreleme modülüne girilenlerin sözcük sözcük kontrolü yapılıyor gerek varsa açın
            /* if (char.IsLetter(charArray[0]) && char.IsLetter(charArray[1]) && char.IsLetter(charArray[2]) && char.IsLetter(charArray[3])) 
             * 
             {

             }*/


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

            GL.Color3(Color.FromArgb(250, 0, 0));
            GL.Vertex3(-30.0, 0.0, 0.0);
            GL.Vertex3(30.0, 0.0, 0.0);


            GL.Color3(Color.FromArgb(0, 0, 0));
            GL.Vertex3(0.0, 30.0, 0.0);
            GL.Vertex3(0.0, -30.0, 0.0);

            GL.Color3(Color.FromArgb(0, 0, 250));
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
                    GL.Color3(Color.FromArgb(255, 0, 0));
                else if (step < 90)
                    GL.Color3(Color.FromArgb(255, 255, 255));
                else if (step < 135)
                    GL.Color3(Color.FromArgb(255, 0, 0));
                else if (step < 180)
                    GL.Color3(Color.FromArgb(255, 255, 255));
                else if (step < 225)
                    GL.Color3(Color.FromArgb(255, 0, 0));
                else if (step < 270)
                    GL.Color3(Color.FromArgb(255, 255, 255));
                else if (step < 315)
                    GL.Color3(Color.FromArgb(255, 0, 0));
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
                    GL.Color3(Color.FromArgb(255, 1, 1));
                else if (step < 90)
                    GL.Color3(Color.FromArgb(250, 250, 200));
                else if (step < 135)
                    GL.Color3(Color.FromArgb(255, 1, 1));
                else if (step < 180)
                    GL.Color3(Color.FromArgb(250, 250, 200));
                else if (step < 225)
                    GL.Color3(Color.FromArgb(255, 1, 1));
                else if (step < 270)
                    GL.Color3(Color.FromArgb(250, 250, 200));
                else if (step < 315)
                    GL.Color3(Color.FromArgb(255, 1, 1));
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
                    GL.Color3(Color.FromArgb(255, 1, 1));
                else if (step < 90)
                    GL.Color3(Color.FromArgb(250, 250, 200));
                else if (step < 135)
                    GL.Color3(Color.FromArgb(255, 1, 1));
                else if (step < 180)
                    GL.Color3(Color.FromArgb(250, 250, 200));
                else if (step < 225)
                    GL.Color3(Color.FromArgb(255, 1, 1));
                else if (step < 270)
                    GL.Color3(Color.FromArgb(250, 250, 200));
                else if (step < 315)
                    GL.Color3(Color.FromArgb(255, 1, 1));
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



        private void koni(float step, float topla, float radius1, float radius2, float dikey1, float dikey2)
        {
            float eski_step = 0.1f;
            GL.Begin(BeginMode.Lines);//Y EKSEN CIZIM DAİRENİN
            while (step <= 360)
            {
                if (step < 45)
                    GL.Color3(1.0, 1.0, 1.0);
                else if (step < 90)
                    GL.Color3(1.0, 0.0, 0.0);
                else if (step < 135)
                    GL.Color3(1.0, 1.0, 1.0);
                else if (step < 180)
                    GL.Color3(1.0, 0.0, 0.0);
                else if (step < 225)
                    GL.Color3(1.0, 1.0, 1.0);
                else if (step < 270)
                    GL.Color3(1.0, 0.0, 0.0);
                else if (step < 315)
                    GL.Color3(1.0, 1.0, 1.0);
                else if (step < 360)
                    GL.Color3(1.0, 0.0, 0.0);


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
                    GL.Color3(Color.FromArgb(255, 1, 1));
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
