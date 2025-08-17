using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum
{
    public enum eStatusConta
    {
        [Description("Inativa")]
        Inativa = 0,
        [Description("Ativa")]
        Ativa = 1     
    } 
}
