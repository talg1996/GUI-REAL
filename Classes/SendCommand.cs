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
using Microsoft.Office.Interop.Excel;


namespace GUI_REAL.Classes
{
    /// <summary>
    /// Handles the sending of commands to instruments.
    /// </summary>
    internal class SendCommand
    {
        Command command;
        Instrument instrument;
        string result = "No output";

        /// <summary>
        /// Initializes a new instance of the <see cref="SendCommand"/> class with a command and an instrument.
        /// </summary>
        /// <param name="command">The command to be sent.</param>
        /// <param name="instrument">The instrument to which the command will be sent.</param>
        public SendCommand(Command command, Instrument instrument)
        {
            this.command = command;
            this.instrument = instrument;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SendCommand"/> class with a command string and an instrument.
        /// </summary>
        /// <param name="command">The command string to be sent.</param>
        /// <param name="instrument">The instrument to which the command will be sent.</param>
        public SendCommand(string command, Instrument instrument)
        {
            this.command.SCPI_Command = command;
            this.instrument = instrument;
        }

        /// <summary>
        /// Sends the command to the instrument based on the communication type.
        /// </summary>
        /// <returns>The result of the command execution.</returns>
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
                    result = sendScipiViaVisaLAN();
                    break;

                case "IP":
                    // Handle command for IP connection
                    break;
                case "ModbusIP":
                    if(instrument.Model== "LIB-Chamber")
                    result = SendLibModbusIPCommand();// Handle command for Modbus IP connection
                    else result="No match chamber";
                    break;

                default:
                    // Handle default case
                    break;
            }

            return result;
        }

        private string SendLibModbusIPCommand()
        {
            try
            {
                string[] parts = command.SCPI_Command.Split(new string[] { "to" }, StringSplitOptions.RemoveEmptyEntries);

                string? commandName = command.SCPI_Command;
                string stringTemperature=null;
                if (parts.Length == 2)
                {
                     commandName = parts[0].Trim();
                     stringTemperature = parts[1].Trim();
                }
                switch (commandName)
                {
                    case string cmdName when cmdName.Contains("Set temperature"):
                        LibCamberModbus.WriteTemperature(instrument.ModbusIP, 8000, (byte)1, float.Parse(stringTemperature));
                        return $"Write temperature to {command.SCPI_Command}";
                    case "Read temperature":
                        string temperature = LibCamberModbus.ReadTemperature(instrument.ModbusIP, 8000, 1).ToString();
                        return $"Read temperature: {temperature}";
                    case "Run":
                        LibCamberModbus.SendRunCommand(instrument.ModbusIP, 8000, 1);
                        return "Run command sent.";
                    case "S":
                        LibCamberModbus.SendStopCommand(instrument.ModbusIP, 8000, 1);
                        return "Stop command sent.";
                    default:
                        return "No supported command";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Issu with LIB chamber communication");
                return "Error was occurred";
            }
            }
        



        private string sendScipiViaVisaUSB()
        {
            // Create ResourceManager and FormattedIO488 instances
            ResourceManager rm = new ResourceManager();
            FormattedIO488 inst = new FormattedIO488();

            // Define ID and command variables 
            string ID = instrument.where_Communicate(instrument.How_Communicate());
            string commandd = command.SCPI_Command;
            try
            {
                inst.IO = (IMessage)rm.Open(ID);
                inst.IO.Timeout = 6000;
                inst.WriteString(commandd);

                if (IsQuery(commandd))
                    result = inst.ReadString();
                else result = "No output";
                inst.IO.Clear();
                inst.IO.Close();

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
            ResourceManager rm = new ResourceManager();
            FormattedIO488 inst = new FormattedIO488();
            string ID = instrument.where_Communicate(instrument.How_Communicate());
            string commandd = command.SCPI_Command;
            try
            {
                inst.IO = (IMessage)rm.Open(ID);
                inst.IO.Timeout = 10000;

                Task t1 = Task.Factory.StartNew(() => inst.WriteString(commandd));
                t1.Wait(3000); // This will wait for the task to complete.

                if (IsQuery(commandd))
                    result = inst.ReadString();
                else result = "No output";


                inst.IO.Clear();
                inst.IO.Close();

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
