using M17E_Caderneta.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace M17E_Caderneta.Models
{
    public class AppRoleProvider : RoleProvider
    {
        private M17E_CadernetaContext db = new M17E_CadernetaContext();
        public override string ApplicationName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string username)
        {
            try
            {
                var user = db.Users.Where(u => u.Id.ToString() == username).First();

                if (user == null) throw new Exception("");

                if (user.Perfil == -1)
                    return new string[] { "Indefinido" };
                else if(user.Perfil == 0)
                    return new string[] { "Administrador" };
                else if (user.Perfil == 1)
                    return new string[] { "Professor" };
                else
                    return new string[] { "Aluno" };
            }
            catch (Exception)
            {

                return new string[] { "" };
            }
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            try
            {
                var user = db.Users.Where(u => u.Id.ToString() == username).First();

                if (user == null) throw new Exception("");

                if (user.Perfil != -1 && roleName == "Indefinido")
                    throw new Exception("");
                if (user.Perfil != 0 && roleName == "Administrador")
                    throw new Exception("");
                if (user.Perfil != 1 && roleName == "Professor")
                    throw new Exception("");
                if (user.Perfil != 2 && roleName == "Aluno")
                    throw new Exception("");

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            return roleName == "Administrador" || roleName == "Professor" || roleName == "Aluno" || roleName == "Indefinido";
        }
    }
}