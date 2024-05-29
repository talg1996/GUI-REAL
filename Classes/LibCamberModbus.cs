using System;
using System.Diagnostics;
using System.Net.Sockets;
using NModbus;

namespace GUI_REAL.Classes
{
    internal struct LibCamberModbus
    {
        /// <summary>
        /// Reads the current temperature from the Modbus device.
        /// </summary>
        /// <param name="ipAddress">IP address of the Modbus device.</param>
        /// <param name="port">Port number of the Modbus device.</param>
        /// <param name="slaveAddress">Slave address of the Modbus device.</param>
        /// <returns>The current temperature in Celsius.</returns>

        


        public static float ReadTemperature(string ipAddress, int port, byte slaveAddress)
        {
            try
            {
                using (var client = new TcpClient(ipAddress, port))
                {
                    var factory = new ModbusFactory();
                    IModbusMaster master = factory.CreateMaster(client);

                    ushort startAddress = 7991; // Address to read temperature
                    ushort numRegisters = 1; // Number of registers to read

                    ushort[] registers = master.ReadInputRegisters(slaveAddress, startAddress, numRegisters);

                    short temperatureRaw = (short)registers[0]; // Raw temperature value

                    float temperature = temperatureRaw / 10.0f; // Converted temperature value
                    return temperature;
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Trace.WriteLine($"Error reading temperature: {ex.Message}");
                return float.NaN; // Return NaN (Not a Number) indicating an error
            }
        }

        public static void WriteTemperature(string ipAddress, int port, byte slaveAddress, float temperature)
        {
            using (var client = new TcpClient(ipAddress, port))
            {
                var factory = new ModbusFactory();
                IModbusMaster master = factory.CreateMaster(client);

                // The temperature value needs to be multiplied by 10 before writing
                ushort value = (ushort)(temperature * 10);

                // Write the temperature value to address 8100 (0x1FA4)
                ushort startAddress = 8100;
                master.WriteSingleRegister(slaveAddress, startAddress, value);
            }
        }

        public static void SendRunCommand(string ipAddress, int port, byte slaveAddress)
        {
            using (var client = new TcpClient(ipAddress, port))
            {
                var factory = new ModbusFactory();
                IModbusMaster master = factory.CreateMaster(client);

                // Write a 1 to the coil to send the run command (address 0x0000 is used as an example)
                ushort coilAddress = 8000;
                master.WriteSingleCoil(slaveAddress, coilAddress, true);
            }
        }

        public static void SendStopCommand(string ipAddress, int port, byte slaveAddress)
        {
            using (var client = new TcpClient(ipAddress, port))
            {
                var factory = new ModbusFactory();
                IModbusMaster master = factory.CreateMaster(client);

                // Write a 0 to the coil to send the stop command (address 0x0000 is used as an example)
                ushort coilAddress = 8001;
                master.WriteSingleCoil(slaveAddress, coilAddress, true);
            }
        }
    }
}
