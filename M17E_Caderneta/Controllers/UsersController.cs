using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using M17E_Caderneta.Data;
using M17E_Caderneta.Models;
using System.Text;
using System.Security.Cryptography;
using PagedList;
using System.IO;

namespace M17E_Caderneta.Controllers
{
    
    public class UsersController : Controller
    {
        private M17E_CadernetaContext db = new M17E_CadernetaContext();

        // GET: Users
        [Authorize(Roles = "Administrador")]
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = sortOrder;

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var users = db.Users.Include(e => e.Turma).Select(e => e);

            if (!String.IsNullOrEmpty(searchString))
            {
                users = users.Where(e => e.Nome.Contains(searchString) || e.NumInterno.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    users = users.OrderByDescending(s => s.Nome);
                    break;
                case "name_asc":
                    users = users.OrderBy(s => s.Nome);
                    break;
                case "num_desc":
                    users = users.OrderByDescending(s => s.NumInterno);
                    break;
                case "num_asc":
                    users = users.OrderBy(s => s.NumInterno);
                    break;
                default:
                    users = users.OrderBy(s => s.Nome);
                    break;
            }

            int pageSize = 3;
            int pageNumber = (page ?? 1);
            return View(users.ToPagedList(pageNumber, pageSize));
        }

        // GET: Users/Details/5
        [Authorize]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = await db.Users.Include(e =>e.Turma).Where(e => e.Id == id).FirstAsync();
            
            if (user == null)
            {
                return HttpNotFound();
            }
            if(User.Identity.Name != id.ToString() && !User.IsInRole("Administrador"))
                return HttpNotFound();

            List<Nota> notas = await db.Notas.Include(e => e.Disciplina)
                .Where(e => e.IdAluno == user.Id)
                .OrderBy(e => e.Disciplina.Nome).ToListAsync();
            var disciplinas = notas.GroupBy(e => e.Disciplina).Select(e => new Disciplina{
                Nome = e.Key.Nome,
                Id = e.Key.Id,
                Descricao = e.Key.Descricao
            }).ToList();

            ViewBag.Notas = notas;
            ViewBag.Disciplinas = disciplinas;

            return View(user);
        }

        // GET: Users/Create
        [AllowAnonymous]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> Create([Bind(Include = "Email,Nome,DataNascimento,NumInterno,Password")] User user, HttpPostedFileBase foto)
        {
            if (ModelState.IsValid)
            {
                if (foto != null && foto.ContentLength > 0)
                {
                    byte[] imageData = null;
                    using (var binaryReader = new BinaryReader(foto.InputStream))
                    {
                        imageData = binaryReader.ReadBytes(foto.ContentLength);
                        user.foto = imageData;
                    }
                    
                   /* user.foto = new byte[foto.ContentLength];
                    foto.InputStream.Read(user.foto, 0, foto.ContentLength);*/
                }
                else
                {
                    ModelState.AddModelError("","Foto de perfil inválida");
                    return View(user);
                }

                HMACSHA512 hMACSHA512 = new HMACSHA512(new byte[] { 1 });
                var password = hMACSHA512.ComputeHash(Encoding.UTF8.GetBytes(user.Password));
                user.Password = Convert.ToBase64String(password);
                
                
                if(User.IsInRole("Administrador"))
                {
                    user.estado = true;
                    if (user.NumInterno[0] == 'p')
                        user.Perfil = 1;
                    else if(user.NumInterno[0] == 'a')
                        user.Perfil = 2;
                }
                else
                {
                    user.estado = false;
                    user.Perfil = -1;
                }
                    
                db.Users.Add(user);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(user);
        }

        // GET: Users/Edit/5
        [Authorize]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            if (User.IsInRole("Administrador") && User.Identity.Name != user.Id.ToString())
            {
                user.perfis = new[]
                {
                    new SelectListItem{Value = "-1",Text = "Indefinido" },
                    new SelectListItem{Value = "0",Text = "Administrador" },
                    new SelectListItem{Value = "1",Text = "Professor" },
                    new SelectListItem{Value = "2",Text = "Aluno" },
                };
            }
            else
            {
                var temp = db.Users.Where(u => u.Id.ToString() == User.Identity.Name && id == u.Id).FirstOrDefault();
                if (temp == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                AppRoleProvider app = new AppRoleProvider();
                var role = app.GetRolesForUser(User.Identity.Name)[0];

                user.perfis = new[]
                {
                    new SelectListItem{Value = user.Perfil.ToString(),Text = role }
                };
            }
            ViewBag.TurmaId = new SelectList(db.Turmas, "id", "Nome");

            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Email,Password,Perfil,estado,Nome,NumInterno,DataNascimento,TurmaId,NumTurma")] User user, HttpPostedFileBase foto)
        {
            byte[] imageData = null;
            if (foto != null && foto.ContentLength > 0)
            {
                
                using (var binaryReader = new BinaryReader(foto.InputStream))
                {
                    imageData = binaryReader.ReadBytes(foto.ContentLength);
                }

            }

            
            if (User.IsInRole("Administrador") && User.Identity.Name != user.Id.ToString())
            {
                if (ModelState.IsValid)
                {
                    var u = await db.Users.FindAsync(user.Id);
                    u.Nome = user.Nome;
                    u.Email = user.Email;
                    u.Perfil = user.Perfil;
                    u.DataNascimento = user.DataNascimento;
                    if(imageData != null)
                        u.foto = imageData;

                    if (User.IsInRole("Administrador"))
                    {
                        u.NumTurma = null;
                        if (user.Perfil == 2)
                        {
                            u.TurmaId = user.TurmaId;
                            u.NumTurma = user.NumTurma;
                        }

                        u.estado = user.estado;
                        u.NumInterno = user.NumInterno;
                    }

                    db.Entry(u).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");

                }
            }
            else
            {
                

                if(ModelState.IsValidField("Nome") && ModelState.IsValidField("Email") 
                    && ModelState.IsValidField("Perfil")
                    && ModelState.IsValidField("DataNascimento"))
                {

                    var u = await db.Users.FindAsync(user.Id);
                    u.Nome = user.Nome;
                    u.Email = user.Email;
                    u.Perfil = user.Perfil;
                    u.DataNascimento = user.DataNascimento;
                    if (imageData != null)
                    {
                        u.foto = imageData;
                        user.foto = u.foto;
                    }



                    if (user.Password != null && user.Password != "")
                    {
                        HMACSHA512 hMACSHA512 = new HMACSHA512(new byte[] { 1 });
                        var password = hMACSHA512.ComputeHash(Encoding.UTF8.GetBytes(user.Password));
                        user.Password = Convert.ToBase64String(password);

                        u.Password = user.Password;
                    }

                    db.Entry(u).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
            }

            
            if (User.IsInRole("Administrador") && User.Identity.Name != user.Id.ToString())
            {
                user.perfis = new[]
                {
                    new SelectListItem{Value = "-1",Text = "Indefinido" },
                    new SelectListItem{Value = "0",Text = "Administrador" },
                    new SelectListItem{Value = "1",Text = "Professor" },
                    new SelectListItem{Value = "2",Text = "Aluno" },
                };
            }
            else
            {
                var temp = db.Users.Where(u => u.Id.ToString() == User.Identity.Name && user.Id == u.Id).FirstOrDefault();
                if (temp == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                AppRoleProvider app = new AppRoleProvider();
                var role = app.GetRolesForUser(User.Identity.Name)[0];

                user.perfis = new[]
                {
                    new SelectListItem{Value = user.Perfil.ToString(),Text = role }
                };
            }
            ViewBag.TurmaId = new SelectList(db.Turmas, "id", "Nome");

            return View(user);
        }

        // GET: Users/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            User user = await db.Users.FindAsync(id);
            try
            {
                db.Users.Remove(user);
                await db.SaveChangesAsync();
            }
            catch
            {

            }
            
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
