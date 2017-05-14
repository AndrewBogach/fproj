using FProj.Api;
using System.Web.Mvc;
using System.Web.Security;
using FProj.Repository;

namespace FProj.Web.Controllers
{
    public class UserController : Controller
    {
        public ActionResult SignUp() => View();

        [HttpPost]
        public ActionResult SignUp(UserApi model, string password) => Json(UnitOfWork.Instance.UserRepository.Add(model, password));

        public ActionResult SignIn() => View();

        [HttpPost]
        public ActionResult SignIn(string email, string password)
        {
            var user = UnitOfWork.Instance.UserRepository.UserVerification(email, password);

            if (user == null)
                return View();

            FormsAuthentication.SetAuthCookie(user.Email, true);

            return RedirectToAction("Index", "Film");
        }

        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}