namespace Brupper.Identity.B2C.Settings
{
    public class PfkSettings : global::B2C.AB2CSettings
    {
        public override string TenantPrefix => "pfkb2c";

        public override string ClientID => "a5f522b8-b0d4-45d8-9da2-cbebd4590624";

        public override string PolicySignUpSignIn => "B2C_1_test";

        public override string PolicyEditProfile => "b2c_1_edit_profile";

        public override string PolicyResetPassword => "b2c_1_reset";
    }
}
