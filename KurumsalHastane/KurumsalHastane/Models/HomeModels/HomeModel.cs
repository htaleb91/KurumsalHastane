namespace KurumsalHastane.Models.HomeModels
{
    public class HomeModel
    {
        public HomeModel()
        {
            SignInModel = new SignInModel();
            ChangePasswordModel = new ChangePasswordModel();
        }
        public ChangePasswordModel ChangePasswordModel { get; set; }

        public SignInModel SignInModel { get; set; }
    }
}
