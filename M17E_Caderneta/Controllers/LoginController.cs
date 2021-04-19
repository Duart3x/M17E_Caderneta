using M17E_Caderneta.Data;
using M17E_Caderneta.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace M17E_Caderneta.Controllers
{
    public class LoginController : Controller
    {
        private M17E_CadernetaContext db = new M17E_CadernetaContext();

        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(User user)
        {
            if(user.Email != null && user.Password != null)
            {
                //Hash password
                HMACSHA512 hMACSHA512 = new HMACSHA512(new byte[] { 1 });
                var password = hMACSHA512.ComputeHash(Encoding.UTF8.GetBytes(user.Password));
                user.Password = Convert.ToBase64String(password);

                foreach (var utilizador in db.Users.ToList())
                {
                    if (user.Email == utilizador.Email && user.Password == utilizador.Password)
                    {
                        if(utilizador.Perfil != -1)
                        {
                            FormsAuthentication.SetAuthCookie(utilizador.Id.ToString(), false);

                            if (Request.QueryString["ReturnUrl"] == null)
                                return RedirectToAction("Index", "Home");
                            else
                                return Redirect("~/"+Request.QueryString["ReturnUrl"].ToString());
                        }
                        else
                        {
                            ModelState.AddModelError("", "A tua conta ainda não foi validade.");

                            return View(user);
                        }
                        

                    }
                }
            }
            ModelState.AddModelError("", "Login falhou. Tente novamente.");
            return View(user);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}