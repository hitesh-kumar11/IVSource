using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IVSource.Models
{
    public class IvsUserTerminal
    {
        public IvsUserDetails userDetail { get; set; }
        public List<IvsUserTerminalId> userDetailTerminalLists { get; set; }
    }
}
