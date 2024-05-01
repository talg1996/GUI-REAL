using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI_REAL
{
    internal struct FlowInstruction
    {
        public string Lable { get; set; }
        public string SCPI_Command { get; set; }
        public string Index_To_Save { get; set; }

    }
}
