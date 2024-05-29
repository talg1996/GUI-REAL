using System.Diagnostics;

namespace GUI_REAL.Classes
{ /// <summary>
  /// Represents an instrument with communication details.
  /// </summary>
    public struct Instrument
    {
        // Properties

        /// <summary>
        /// Gets or sets the model of the instrument.
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// Gets or sets the name of the instrument.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the COM port of the instrument.
        /// </summary>
        public string Com { get; set; }

        /// <summary>
        /// Gets or sets the LAN address of the instrument.
        /// </summary>
        public string Lan { get; set; }

        /// <summary>
        /// Gets or sets the VISA USB address of the instrument.
        /// </summary>
        public string Visa_USB { get; set; }

        /// <summary>
        /// Gets or sets the VISA LAN address of the instrument.
        /// </summary>
        public string Visa_Lan { get; set; }

        /// <summary>
        /// Gets or sets the IP address of the instrument.
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// Gets or sets the ModbusIP address of the instrument.
        /// </summary>
        public string ModbusIP { get; set; } 

        //  Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Instrument"/> struct with default values.
        /// </summary>
        public Instrument()
        {
            Model = "none";
            Name = "none";
            Com = "none";
            Lan = "none";
            Visa_USB = "none";
            Visa_Lan = "none";
            IP = "none";
            ModbusIP = "none";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Instrument"/> struct with specified values.
        /// </summary>
        /// <param name="model">The model of the instrument.</param>
        /// <param name="name">The name of the instrument.</param>
        /// <param name="com">The COM port of the instrument.</param>
        /// <param name="lan">The LAN address of the instrument.</param>
        /// <param name="visa_usb">The VISA USB address of the instrument.</param>
        /// <param name="visa_lan">The VISA LAN address of the instrument.</param>
        /// <param name="ip">The IP address of the instrument.</param>
        /// <param name="modbusIp" >The ModbusIP address of the instrument.</param>
        public Instrument(string model, string name, string com, string lan, string visa_usb, string visa_lan, string ip ,string modbusIp)
        {
            Model = model;
            Name = name;
            Com = com;
            Lan = lan;
            Visa_USB = visa_usb;
            Visa_Lan = visa_lan;
            IP = ip;
            ModbusIP=modbusIp;

        }

        // Methods

        /// <summary>
        /// Gets the details of the instrument.
        /// </summary>
        /// <returns>A string containing the details of the instrument.</returns>
        public string Details()
        {
            string details = $"Model:{Model}\nName:{Name}\nCOM:{Com}\nLAN:{Lan}\nVISA USB:{Visa_USB}\nVISA LAN:{Visa_Lan}\nIP:{IP}\n{ModbusIP}\n";
            return details;
        }


        /// <summary>
        /// Determines the communication type of the instrument.
        /// </summary>
        /// <returns>The communication type.</returns>
        public string How_Communicate()
        {
            if (Com.ToLower() != "none") { return "Com"; }
            if (Lan.ToLower() != "none") { return "Lan"; }
            if (Visa_USB.ToLower() != "none") { return "Visa_USB"; }
            if (Visa_Lan.ToLower() != "none") { return "Visa_Lan"; }
            if (IP.ToLower() != "none") { return "IP"; }
            if(ModbusIP.ToLower() != "none") { return "ModbusIP"; }
            return "none";
        }


        /// <summary>
        /// Determines where the instrument communicates based on the specified communication type.
        /// </summary>
        /// <param name="WhereToSend">The communication type.</param>
        /// <returns>The communication address.</returns>
        public string where_Communicate(string WhereToSend)
        {
            if (WhereToSend == "Com") { return Com; }
            if (WhereToSend == "Lan") { return Lan; }
            if (WhereToSend == "Visa_USB") { return Visa_USB; }
            if (WhereToSend == "Visa_Lan") { return Visa_Lan; }
            if (WhereToSend == "IP") { return IP; }
            if((WhereToSend == "ModbusIP")) { return ModbusIP; }
            return "none";
        }

    }
}