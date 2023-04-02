namespace University.Server.Domain.Models
{
    public enum EModuleStatus : byte
    {
        // EDUCATES | Unterrichtet Modul
        Educates = 0,

        // ENROLLED | Angemeldet zur Prüfung
        Enrolled = 1,

        // ENROLMENT | Angemeldet
        // Enrolment = 2,

        // PASSED | Bestanden
        Passed = 2,

        // FAILED | Nicht Bestanden
        Failed = 3,

        // EXCLUDED | Prüfungszulassung entzogen
        // Excluded = 5,

        // TRANSFERRED | Angerechnet / Anerkannt
        // Transferred = 6,

        // MISSING | Ergebnis noch ausstehend
        // Missing = 7,
    }
}