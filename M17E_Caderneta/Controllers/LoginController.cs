using M17E_Caderneta.Data;
using M17E_Caderneta.Helper;
using M17E_Caderneta.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
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

        public ActionResult ForgotPassword()
        {
            if (Request["id"] == null)
                return View();

            string guid = Server.UrlDecode(Request["id"].ToString());
            var user = db.Users.Where(e => e.lnkRecuperar == guid).First();

            if (user == null)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(string email)
        {
            var user = db.Users.Where(e => e.Email == email).FirstOrDefault();
            
            if(user == null)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            Guid g = Guid.NewGuid();
            user.lnkRecuperar = g.ToString();

            db.Entry(user).State = EntityState.Modified;
            await db.SaveChangesAsync();

            string mensagem = "Clique no link para recuperar a sua passowrd. \n";
            mensagem += "<a href='http://" + Request.Url.Authority + "/Login/ForgotPassword?";
            mensagem += "id=" + Server.UrlEncode(g.ToString()) + "'>Clique aqui</a>";

            string ppassword = ConfigurationManager.AppSettings["pwdEmail"].ToString();
            string meuEmail = ConfigurationManager.AppSettings["Email"].ToString();

            Utils.enviarMail(meuEmail, ppassword, email, "Recuperar Password", mensagem);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RecoverPassword(string password, string idrecuperar)
        {
            string guid = Server.UrlDecode(idrecuperar);
            var user = db.Users.Where(e => e.lnkRecuperar == guid).First();

            if (user == null)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            password = password.Trim();

            if (password.Length <= 3)
                return View();

            HMACSHA512 hMACSHA512 = new HMACSHA512(new byte[] { 1 });
            var passwordbytes = hMACSHA512.ComputeHash(Encoding.UTF8.GetBytes(password));
            user.Password = Convert.ToBase64String(passwordbytes);
            user.lnkRecuperar = null;

            db.Entry(user).State = EntityState.Modified;
            await db.SaveChangesAsync();

            return RedirectToAction("Index");
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