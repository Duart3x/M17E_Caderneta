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
using PagedList;

namespace M17E_Caderneta.Controllers
{
    public class DisciplinasController : Controller
    {
        private M17E_CadernetaContext db = new M17E_CadernetaContext();

        // GET: Disciplinas
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

            var disciplinas = db.Disciplinas.Select(e => e);

            if (!String.IsNullOrEmpty(searchString))
            {
                disciplinas = disciplinas.Where(e=> e.Nome.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    disciplinas = disciplinas.OrderByDescending(s => s.Nome);
                    break;
                case "name_asc":
                    disciplinas = disciplinas.OrderBy(s => s.Nome);
                    break;
                default:
                    disciplinas = disciplinas.OrderBy(s => s.Nome);
                    break;
            }
            int pageSize = 3;
            int pageNumber = (page ?? 1);
            return View(disciplinas.ToPagedList(pageNumber, pageSize));
        }

        // GET: Disciplinas/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Disciplina disciplina = await db.Disciplinas.FindAsync(id);
            if (disciplina == null)
            {
                return HttpNotFound();
            }
            return View(disciplina);
        }

        // GET: Disciplinas/Create
        [Authorize(Roles ="Administrador")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Disciplinas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> Create([Bind(Include = "Id,Nome,Descricao")] Disciplina disciplina)
        {
            if (ModelState.IsValid)
            {
                db.Disciplinas.Add(disciplina);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(disciplina);
        }

        // GET: Disciplinas/Edit/5
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Disciplina disciplina = await db.Disciplinas.FindAsync(id);
            if (disciplina == null)
            {
                return HttpNotFound();
            }
            return View(disciplina);
        }

        // POST: Disciplinas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Nome,Descricao")] Disciplina disciplina)
        {
            if (ModelState.IsValid)
            {
                db.Entry(disciplina).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(disciplina);
        }

        // GET: Disciplinas/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Disciplina disciplina = await db.Disciplinas.FindAsync(id);
            if (disciplina == null)
            {
                return HttpNotFound();
            }
            return View(disciplina);
        }

        // POST: Disciplinas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Disciplina disciplina = await db.Disciplinas.FindAsync(id);
            db.Disciplinas.Remove(disciplina);
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
