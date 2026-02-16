using Newtonsoft.Json;

namespace Brupper.AspNetCore.Identity.B2C;

public class UsersModel
{
    public UserModel[] Users { get; set; }
        = new UserModel[0];

    public static UsersModel? Parse(string json)
        => JsonConvert.DeserializeObject<UsersModel>(json);
}