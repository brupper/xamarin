using Newtonsoft.Json;

namespace Brupper.Identity
{
    public class UsersModel
    {
        public UserModel[] Users { get; set; }

        public static UsersModel Parse(string json) => JsonConvert.DeserializeObject<UsersModel>(json);
    }
}