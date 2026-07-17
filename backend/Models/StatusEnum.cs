namespace Backend.Models
{
    public enum StatusEnum
    {
        Added,      // creat de user, asteapta validarea adminului
        Validated,  // aprobat de admin, dar licitatia nu a inceput inca
        ActiveBid,  // licitatie live
        NoWinner,   // s-a terminat fara nicio oferta
        Sold,       // s-a terminat cu castigator
        Rejected    // respins de admin
    }
}
