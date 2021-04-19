using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace M17E_Caderneta.Helper
{
    public static class Utils
    {
        public static string UserName(this HtmlHelper htmlHelper,
                System.Security.Principal.IPrincipal utilizador)
        {
            string UserName = "";

            using (var context = new Data.M17E_CadernetaContext())
            {
                var user = context.Users.Where(u => u.Id.ToString() == utilizador.Identity.Name).ToList()[0];

                UserName = user.Nome;
            }

            return UserName;
        }
    }
}