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

        public string SendCommandToInstrument()
        {
            
            switch (instrument.How_Communicate())
            {
                case "Com":
                    result = sendScipiViaVisaCOM();
                    break;

                case "Lan":
                    // Handle command for LAN connection
                    break;

                case "Visa_USB":
                    
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

        private string sendScipiViaVisaCOM( )

        {
            string result;
            string commandd = command.SCPI_Command;
            int time_out = 3000;
            int delay = 400;

           // if (!command.Contains("?")) { time_out = 300; delay = 0; } //if it is a quary dont wait for buffer tu update and dont take long runtime
            
            
                SerialPort port = new SerialPort(instrument.Com, 115200, Parity.None, 8, StopBits.One);
                port.ReadTimeout = time_out; //3 second Set a reasonable read timeout (adjust as needed)

                port.Open();


                // Assign the brush to the Fill property

                port.WriteLine(command.SCPI_Command); //Add delay so the instrument have time to write the correct value

                int milliseconds = delay; // 0.7 seconds
                Thread.Sleep(milliseconds);
            if (IsQuery(commandd)){
                double read = Convert.ToDouble(port.ReadLine());
                result = read.ToString();
            }
            else
            {
                result = "no output";
            }



            port.Close();

            return result;
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

        public bool IsQuery(string command)
        {
            return command.Contains("?");
        }
    }
}
