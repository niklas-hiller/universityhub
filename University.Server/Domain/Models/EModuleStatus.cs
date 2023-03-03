using System.ComponentModel;

namespace University.Server.Domain.Models
{
    public enum EModuleStatus : byte
    {
        // EDUCATES | Unterrichtet Modul
        [Description("Educates")]
        Educates = 0,

        // ENROLLED | Angemeldet zur Prüfung
        [Description("Enrolled")]
        Enrolled = 1,

        // ENROLMENT | Angemeldet
        [Description("Enrolment")]
        Enrolment = 2,

        // PASSED | Bestanden
        [Description("Passed")]
        Passed = 3,

        // FAILED | Nicht Bestanden
        [Description("Failed")]
        Failed = 4,

        // EXCLUDED | Prüfungszulassung entzogen
        [Description("Excluded")]
        Excluded = 5,

        // TRANSFERRED | Angerechnet / Anerkannt
        [Description("Transferred")]
        Transferred = 6,

        // MISSING | Ergebnis noch ausstehend
        [Description("Missing")]
        Missing = 7,
    }
}
