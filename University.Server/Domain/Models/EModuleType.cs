using System.ComponentModel;

namespace University.Server.Domain.Models
{
    public enum EModuleType : byte
    {
        // COMPULSORY | Pflichtmodul
        [Description("Compulsory")]
        Compulsory = 1,

        // OPTIONAL | Optionales Modul
        [Description("Optional")]
        Optional = 2,
    }
}
