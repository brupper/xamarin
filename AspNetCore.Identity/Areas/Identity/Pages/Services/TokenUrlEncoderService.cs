using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace Brupper.AspNetCore.Identity.Areas.Identity;

public class TokenUrlEncoderService
{

    public virtual string EncodeToken(string token)
        => WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

    public virtual string DecodeToken(string urlToken)
        => Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(urlToken));
}
