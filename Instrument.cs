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
        public string Visa_Usb { get; set; }
        public string Visa_Lan { get; set; }

        // Default Constructor
        public Instrument()
        {
            Model = "none";
            Name = "none";
            Com = "none";
            Lan = "none";
            Visa_Usb = "none";
            Visa_Lan = "none";
        }

        // Method
        public string Details()
        {
            string details = $"{Model}\n{Name}\n{Com}\n{Lan}\n{Visa_Usb}\n{Visa_Lan}\n";
            Trace.WriteLine(details);
            return details;
        }
    }
}