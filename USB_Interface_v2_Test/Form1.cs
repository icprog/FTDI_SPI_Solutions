﻿using System;
using System.Text;
using System.Windows.Forms;

namespace USB_Interface_v2_Test
{
    public partial class Form1 : Form
    {

        public USB_Control usb = new USB_Control();

        private bool signalSwap;

        public Form1()
        {
            InitializeComponent();
            ExtLog.bx = textBox1;
            timer1.Interval = GlobalProperties.USB_Refresh_Period;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ExtLog.AddLine("Loading Devices:");
            var dl = usb.GetDevicesList();
            if (dl.Count > 0)
            {
                comboBox1.Items.Clear();
                foreach (var l in dl)
                {
                    ExtLog.AddLine(l);
                    comboBox1.Items.Add(l);
                }
                if (dl.Count == 1)
                {
                    comboBox1.SelectedIndex = 0;
                }
            }
        }



        private void timer1_Tick(object sender, EventArgs e)
        {
            if (usb.IsOpen)
            {

                if (!radioButton1.Checked)
                {
                    var b = radioButton2.Checked ? (signalSwap ? (byte)0xAA : (byte)0x55):(signalSwap? (byte)0x00 : (byte)0xFF);

                    binarySelector1.ChangeValue(b);
                    binarySelector2.ChangeValue(b);
                    binarySelector3.ChangeValue(b);
                    signalSwap = !signalSwap;
                }
                
                SignalGenerator.OutputByte0 = binarySelector1.Result;
                SignalGenerator.OutputByte1 = binarySelector2.Result;
                SignalGenerator.OutputByte2 = binarySelector3.Result;

                usb.Transfer();
                Update(usb.InputBuffer);

                binarySelector4.ChangeValue(SignalGenerator.InputByte0);
                binarySelector5.ChangeValue(SignalGenerator.InputByte1);
            }
        }

        private StringBuilder sb = new StringBuilder();

        public void Update(byte[] usbData)
        {
            if (usbData.Length == 64)
            {
                textBox2.Clear();
                sb.Clear();
                for (var i = 0; i < usb.dataSize; i++)
                {
                    sb.Append(i.ToString("D2"));
                    sb.Append(" ");
                    sb.AppendLine(Convert.ToString(outDataCheckbox.Checked ? usb.OutputBuffer[i]:usbData[i], 2).PadLeft(8, '0'));
                }
                textBox2.Text = sb.ToString();


            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var sel = comboBox1.SelectedItem.ToString(); 
            comboBox1.Text = sel;
            usb.OpenDeviceByLocation(uint.Parse(sel.Substring(0, 4)));
        }

    }



    public static class ExtLog
    {
        public static TextBox bx;
        public static void AddLine(string s)
        {
            bx?.AppendText(s + "\r\n");
        }
    }
}
