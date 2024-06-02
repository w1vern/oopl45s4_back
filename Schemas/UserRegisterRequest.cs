using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace MafiaAPI.Schemas
{
    public class UserRegisterRequest
    {
        public string login {  get; set; }
        public string password1 { get; set; }
        public string password2 { get; set; }
    }
}
