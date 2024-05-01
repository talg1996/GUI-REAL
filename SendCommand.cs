using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VisaComLib;
using System.IO.Ports;
using System.Windows.Controls;

namespace GUI_REAL
{
    internal class SendCommand
    {
         
        Command command;
        Instrument instrument;
        string result = "No output";

        public SendCommand(Command command, Instrument instrument)
        {
            this.command = command;
            this.instrument = instrument;
        }

        public SendCommand(string command, Instrument instrument)
        {
            this.command.SCPI_Command = command;
            this.instrument = instrument;
        }

        public string SendCommandToInstrument()
        {
            
            switch (instrument.How_Communicate())
            {
                case "Com":
                    result = sendScipiViaCOM();
                    break;

                case "Lan":
                    // Handle command for LAN connection
                    break;

                case "Visa_USB":
                    result = sendScipiViaVisaUSB();
                    break;

                case "Visa_Lan":
                    result =sendScipiViaVisaLAN();
                    break;

                default:
                    // Handle default case
                    break;
            }

            return result;
        }

        private string sendScipiViaVisaUSB()
        {
            // Create ResourceManager and FormattedIO488 instances
            ResourceManager rm = new ResourceManager();
            FormattedIO488 inst = new FormattedIO488();

            // Define ID and commandd variables 
            string ID = instrument.where_Communicate(instrument.How_Communicate());
            string commandd = command.SCPI_Command;
            try
            {
                inst.IO = (IMessage)rm.Open(ID);
                inst.WriteString(commandd);

                if (IsQuery(commandd))
                    result = inst.ReadString();
                else result = "No output";


            }
            catch (Exception ex)
            {
                result = ex.Message;
                MessageBox.Show(ex.Message);
            }
            return result;
        }

        private string sendScipiViaCOM()
        {
            string result = "no output";

            string[] portNames = SerialPort.GetPortNames();
            if (!portNames.Contains(instrument.Com))
            {
                return "Specified port is not available.";
            }

            try
            {
                using (SerialPort port = new SerialPort(instrument.Com, 115200, Parity.None, 8, StopBits.One))
                {
                    port.ReadTimeout = 3000; // Set a reasonable read timeout (adjust as needed)
                    port.Open();

                    port.WriteLine(command.SCPI_Command); // Send command

                    if (command.SCPI_Command.Contains("?"))
                    {
                        string response = port.ReadLine();
                        if (!string.IsNullOrEmpty(response))
                        {
                            result = response.Trim();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., port not available, permission denied)
                result = $"Error: {ex.Message}";
            }

            return result;
        }

        private bool IsQuery(string command)
        {
            return command.Contains("?");
        }


        private string sendScipiViaVisaLAN()
        {
            VisaComLib.ResourceManager rm = new VisaComLib.ResourceManager();
            VisaComLib.FormattedIO488 inst = new FormattedIO488();
            string ID = instrument.where_Communicate(instrument.How_Communicate());
            string commandd = command.SCPI_Command;
            try
            {
                inst.IO = (IMessage)rm.Open(ID);
                inst.WriteString(commandd);
               
                    if(IsQuery(commandd))
                    result = inst.ReadString();
                    else result = "No output";
                
                
            }
            catch (Exception ex)
            {
                result = ex.Message;
               MessageBox.Show(ex.Message);
            }
            return result;
        }

        
    }
}
