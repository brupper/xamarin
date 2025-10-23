using Microsoft.AspNetCore.Identity;

namespace Brupper.AspNetCore.Identity.Entities;

public class User : IdentityUser //, IMayHaveTenant
{
    #region Constructors

    public User() { }

    public User(string userName) : base(userName) { }

    #endregion

    /// <summary> Vezeték és keresztnév </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// Az adott felhasználót melyik céghez rendelték hozzá.
    /// <para>
    ///   Ez a bejegyzés a Claims-ek közt is szerepel.
    ///   Azért tároljuk itt is, mert a céges adminoknak
    ///   ez biztosítja az előszűrés, hogy mely Usereket láthatja.
    /// </para>
    /// </summary>
    public string? TenantId { get; set; }
}
