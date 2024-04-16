using System.Diagnostics;

namespace GUI_REAL
{
    public struct Instrument
    {
        // Properties
        public string Model { get; set; }
        public string Name { get; set; }
        public string Com { get; set; }
        public string Lan { get; set; }
        public string Visa_USB { get; set; }
        public string Visa_Lan { get; set; }
        public string IP { get; set; }

        // Default Constructor
        public Instrument()
        {
            Model = "none";
            Name = "none";
            Com = "none";
            Lan = "none";
            Visa_USB = "none";
            Visa_Lan = "none";
            IP = "none";
        }

        public Instrument(string model, string name, string com, string lan, string visa_usb, string visa_lan, string ip)
        {
            Model = model;
            Name = name;
            Com = com;
            Lan = lan;
            Visa_USB = visa_usb;
            Visa_Lan = visa_lan;
            IP = ip;

        }

        // Method
        public string Details()
        {
            string details = $"Model:{Model}\nName:{Name}\nCOM:{Com}\nLAN:{Lan}\nVISA USB:{Visa_USB}\nVISA LAN:{Visa_Lan}\nIP:{IP}\n";
            Console.WriteLine(details);
            return details;
        }


        /// <summary>
        /// This function show how the instrument communicate
        /// </summary>
        /// <returns>Communicate type</returns>

        public string How_Communicate()
        {
            if (Com != "none") { return "Com"; }
            if (Lan != "none") { return "Lan"; }
            if (Visa_USB != "none") { return "Visa_USB"; }
            if (Visa_Lan != "none") { return "Visa_Lan"; }
            if (IP != "none") { return "IP"; }
            return "none";
        }

        public string where_Communicate(string WheeToSend)
        {
            if (WheeToSend == "Com") { return this.Com; }
            if (WheeToSend == "Lan") { return this.Lan; }
            if (WheeToSend == "Visa_USB") { return this.Visa_USB; }
            if (WheeToSend == "Visa_Lan") { return this.Visa_Lan; }
            if (WheeToSend == "IP") { return this.IP; }
            return "none";
        }

    }
}