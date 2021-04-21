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
    
    public class TurmasController : Controller
    {
        private M17E_CadernetaContext db = new M17E_CadernetaContext();

        // GET: Turmas
        [Authorize(Roles = "Administrador,Professor")]
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

            var turmas = db.Turmas.Select(e => e);

            if (!String.IsNullOrEmpty(searchString))
            {
                turmas = turmas.Where(e => string.Concat(e.Ano,e.Letra," (", e.AnoLetivo.Year.ToString(), ")").Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    turmas = turmas.OrderByDescending(e => string.Concat(e.Ano, e.Letra, " (", e.AnoLetivo.Year.ToString(), ")"));
                    break;
                case "name_asc":
                    turmas = turmas.OrderBy(e => string.Concat(e.Ano, e.Letra, " (", e.AnoLetivo.Year.ToString(), ")"));
                    break;
                default:
                    turmas = turmas.OrderBy(e => string.Concat(e.Ano, e.Letra, " (", e.AnoLetivo.Year.ToString(), ")"));
                    break;
            }
            int pageSize = 3;
            int pageNumber = (page ?? 1);
            return View(turmas.ToPagedList(pageNumber, pageSize));
        }

        public JsonResult PesquisaNotasAluno(string nome)
        {
            var clientes = db.Notas.Include(e => e.Aluno).Include(e => e.Aluno.Turma).Where(c => c.Aluno.Nome.Contains(nome)).ToList();


            return Json(clientes, JsonRequestBehavior.AllowGet);
        }

        // GET: Turmas/Details/5
        [Authorize(Roles = "Administrador,Professor")]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Turma turma = await db.Turmas.FindAsync(id);
            if (turma == null)
            {
                return HttpNotFound();
            }
            double media = 0;
            List<Nota> notas = await db.Notas
                .Where(e => e.Aluno.TurmaId == id).ToListAsync();
            
            if (notas.Count > 0)
                media = Math.Round(notas.Average(e => e.Valor), 0);
            ViewBag.MediaTurma = media;

            return View(turma);
        }

        // GET: Turmas/Create
        [Authorize(Roles = "Administrador")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Turmas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> Create([Bind(Include = "id,AnoLetivo,Ano,Letra")] Turma turma)
        {
            if (ModelState.IsValid)
            {
                db.Turmas.Add(turma);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(turma);
        }

        // GET: Turmas/Edit/5
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Turma turma = await db.Turmas.FindAsync(id);
            if (turma == null)
            {
                return HttpNotFound();
            }
            return View(turma);
        }

        // POST: Turmas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> Edit([Bind(Include = "id,AnoLetivo,Ano,Letra")] Turma turma)
        {
            if (ModelState.IsValid)
            {
                db.Entry(turma).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(turma);
        }

        // GET: Turmas/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Turma turma = await db.Turmas.FindAsync(id);
            if (turma == null)
            {
                return HttpNotFound();
            }
            return View(turma);
        }

        // POST: Turmas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Turma turma = await db.Turmas.FindAsync(id);
            db.Turmas.Remove(turma);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Administrador,Professor")]
        public async Task<ActionResult> ConsultaNotas(int id)
        {
            List<Nota> notas = await db.Notas.Include(e => e.Disciplina).Include(e => e.Aluno).Include(e => e.Aluno.Turma)
                .Where(e => e.Aluno.TurmaId == id)
                .OrderBy(e => e.Disciplina.Nome).OrderBy(e=> e.Aluno.Nome).ToListAsync();

            var disciplinas = notas.GroupBy(e => e.Disciplina).Select(e => new Disciplina
            {
                Nome = e.Key.Nome,
                Id = e.Key.Id,
                Descricao = e.Key.Descricao
            }).ToList();
            double media = 0;
            if (notas.Count > 0)
                media = Math.Round(notas.Average(e => e.Valor), 0);
            ViewBag.MediaTurma = media;
            ViewBag.Disciplinas = disciplinas;
            ViewBag.Notas = notas;

            return View();
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
