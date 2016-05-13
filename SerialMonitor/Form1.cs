using System;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SerialMonitor
{
    public partial class Form1 : Form
    {        
        public const int indexMaster = 0;
        public const int indexSlave = 1;
        
        public Form1()
        {
            InitializeComponent();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            try
            {
                string comPort = PortInput.Text;
                string packetLength = PacketLengthInput.Text;

                if (!Regex.IsMatch(comPort, "COM[0-9]{1,2}"))
                {
                    AppendTextToForm("Please enter a valid COM port!");
                    return;
                }

                serialPort1.PortName = comPort;
                serialPort1.ReceivedBytesThreshold = int.Parse(packetLength);

                if (!serialPort1.IsOpen)
                {
                    serialPort1.Open();
                    txtStatus.Text = "Open";
                    btnStart.Enabled = false;
                    btnStop.Enabled = true;
                }
                else
                    AppendTextToForm("Not Open!");
            }
            catch (Exception ex)
            {
                AppendTextToForm(ex.ToString());
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                try
                {
                    serialPort1.Close();
                    txtStatus.Text = "Closed";
                    btnStop.Enabled = false;
                    btnStart.Enabled = true;
                }
                catch (Exception ex)
                {
                    AppendTextToForm(ex.ToString());
                }
            }
            else
                AppendTextToForm("Already closed!");
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
    }
}
