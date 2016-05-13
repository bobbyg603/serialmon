using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Windows.Forms;

using System.Text.RegularExpressions;

namespace SerialMonitor
{
    public partial class Form1 : Form
    {        
        public const int indexMaster = 0;
        public const int indexSlave = 1;
        
        public Form1()
        {
            InitializeComponent();
            comboBoxDevice.SelectedIndex = 1;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            string comPort = comPortTextBox.Text;
            string recievedBytesThreshold = recievedBytesTextBox.Text;

            if (!Regex.IsMatch(comPort, "COM[0-9]{1,2}"))
            {
                AppendTextToForm("Please enter a valid COM port!");
                return;
            }

            try
            {
                serialPort1.PortName = comPort;
                serialPort1.ReceivedBytesThreshold = int.Parse(recievedBytesThreshold);
            }
            catch (Exception ex)
            {
                AppendTextToForm(ex.ToString());
                return;
            }

            if (!serialPort1.IsOpen)
            {
                try
                {
                    serialPort1.Open();
                    txtStatus.Text = "Running";
                    btnStart.Enabled = false;
                    btnStop.Enabled = true;
                }
                catch (Exception ex)
                {
                    AppendTextToForm(ex.ToString());
                }
            }
            else
                AppendTextToForm("Already running!");
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                try
                {
                    serialPort1.Close();
                    txtStatus.Text = "Stopped";
                    btnStop.Enabled = false;
                    btnStart.Enabled = true;
                }
                catch (Exception ex)
                {
                    AppendTextToForm(ex.ToString());
                }
            }
            else
                AppendTextToForm("Already stopped!");
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtMain.Text = String.Empty;
        }

        // Used to update the status textbox on the MainForm from another thread
        private delegate void ControlStringConsumer(TextBox textbox, string text);

        private void UpdateTextBoxOnMainForm(TextBox textbox, string text)
        {
            if (textbox.InvokeRequired)
            {
                textbox.Invoke(new ControlStringConsumer(UpdateTextBoxOnMainForm), new object[] { textbox, text });
            }
            else
            {
                textbox.AppendText(text + Environment.NewLine);
            }
        }
        
        private void AppendTextToForm(string text)
        {
            Debug.WriteLine("info received");
            UpdateTextBoxOnMainForm(txtMain, text + Environment.NewLine);
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            int length = sp.BytesToRead;
            byte[] inData = new byte[length];

            sp.Read(inData, 0, length);

            AppendTextToForm(BitConverter.ToString(inData));
        }

        private void comboBoxBaud_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = comboBoxBaud.SelectedIndex;
            int baud = 9600;

            switch (index)
            {
                case 0: baud = 2400; break;
                case 1: baud = 4800; break;
                case 2: baud = 9600; break;
                case 3: baud = 14400; break;
                case 4: baud = 19200; break;
                case 5: baud = 38400; break;
                case 6: baud = 57600; break;
                case 7: baud = 115200; break;
            }

            try
            {
                serialPort1.BaudRate = baud;
            }
            catch (Exception ex)
            {
                AppendTextToForm(ex.ToString());
            }
        }

        private void comboBoxDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxDevice.SelectedIndex == indexMaster)
            {
                serialPort1.ReceivedBytesThreshold = 3;
                comboBoxBaud.SelectedIndex = 7; // TODO use const
            }

            if (comboBoxDevice.SelectedIndex == indexSlave)
            {
                serialPort1.ReceivedBytesThreshold = 9;
                comboBoxBaud.SelectedIndex = 2; // TODO use const
            }
        }
    }
}
