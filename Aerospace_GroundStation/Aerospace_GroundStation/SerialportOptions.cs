using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Aerospace_GroundStation
{
    public static class SerialportConfigure//seri port ayarları yapmaya yarar
    {
        public static void SerialPortOptions(SerialPort serialport,System.Windows.Forms.ComboBox comboBoxSERİALPORT,System.Windows.Forms. ComboBox comboBoxBAUDRATE) // seri port ayarları
        {
            
            serialport = new SerialPort();
            serialport.PortName = comboBoxSERİALPORT.SelectedIndex.ToString();
            serialport.BaudRate = comboBoxBAUDRATE.SelectedIndex;
            serialport.DataBits = 8;
            serialport.Parity = Parity.None;
            serialport.StopBits = StopBits.None;
        }

    }
}
