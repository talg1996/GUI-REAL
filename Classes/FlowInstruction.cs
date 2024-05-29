using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI_REAL.Classes
{
    /// <summary>
    /// Stores the values entered by the user 
    ///who writes the test flow at excel.
    /// </summary>
    internal struct FlowInstruction
    {
        /// <summary>
        /// Gets or sets the label of the instrument.
        /// </summary>
        public string Lable { get; set; }

        /// <summary>
        /// Gets or sets the SCPI command of the instruction.
        /// </summary>
        public string SCPI_Command { get; set; }

        /// <summary>
        /// Gets or sets the index to save the instruction global array result.
        /// </summary>
        public string Index_To_Save { get; set; }

    }
}
