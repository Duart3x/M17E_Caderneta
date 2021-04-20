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

namespace M17E_Caderneta.Controllers
{
    
    public class NotasController : Controller
    {
        private M17E_CadernetaContext db = new M17E_CadernetaContext();

        // GET: Notas
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> Index()
        {
            var notas = db.Notas.Include(e => e.Aluno).Include(e => e.Aluno.Turma).Include(e => e.Professor).Include("Disciplina");
            return View(await notas.ToListAsync());
        }

        // GET: Notas/Details/5
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Nota nota = await db.Notas.Include(e => e.Aluno).Include(e => e.Aluno.Turma).Include(e => e.Professor).Include("Disciplina").Where(e => e.Id == id).FirstAsync();
            if (nota == null)
            {
                return HttpNotFound();
            }
            return View(nota);
        }

        // GET: Notas/Create
        [Authorize(Roles = "Professor,Administrador")]
        public ActionResult Create()
        {
            ViewBag.IdAluno = new SelectList(db.Users.Where(e => e.Perfil == 2 && e.NumTurma != null).Include("Turma").ToList(), "Id", "NomeCompleto");
            ViewBag.IdDisciplina = new SelectList(db.Disciplinas, "Id", "Nome");
            if (User.IsInRole("Professor"))
            {
                ViewBag.IdProfessor = new SelectList(db.Users.Where(e => e.Id.ToString() == User.Identity.Name), "Id", "NomeCompleto");
            }
            else
            {
                ViewBag.IdProfessor = new SelectList(db.Users.Where(e => e.Perfil == 1 || e.Perfil == 0), "Id", "NomeCompleto");
            }
            return View();
        }

        // POST: Notas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Professor,Administrador")]
        public async Task<ActionResult> Create([Bind(Include = "Id,IdAluno,IdProfessor,IdDisciplina,Valor")] Nota nota)
        {
            if (ModelState.IsValid)
            {
                db.Notas.Add(nota);
                await db.SaveChangesAsync();
                if (User.IsInRole("Professor"))
                    return RedirectToAction("Index", "Home");
                else
                    return RedirectToAction("Index");
            }

            ViewBag.IdAluno = new SelectList(db.Users.Where(e => e.Perfil == 2 && e.NumTurma != null).Include("Turma").ToList(), "Id", "NomeCompleto");
            ViewBag.IdDisciplina = new SelectList(db.Disciplinas, "Id", "Nome");
            if(User.IsInRole("Professor"))
            {
                ViewBag.IdProfessor = new SelectList(db.Users.Where(e => e.Id.ToString() == User.Identity.Name), "Id", "NomeCompleto");
            }
            else
            {
                ViewBag.IdProfessor = new SelectList(db.Users.Where(e => e.Perfil == 1 || e.Perfil == 0), "Id", "NomeCompleto");
            }
            
            return View(nota);
        }

        // GET: Notas/Edit/5
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Nota nota = await db.Notas.FindAsync(id);
            if (nota == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdAluno = new SelectList(db.Users.Where(e => e.Perfil == 2 && e.NumTurma != null).Include("Turma").ToList(), "Id", "NomeCompleto");
            ViewBag.IdDisciplina = new SelectList(db.Disciplinas, "Id", "Nome");
            ViewBag.IdProfessor = new SelectList(db.Users.Where(e => e.Perfil == 1 || e.Perfil == 0), "Id", "NomeCompleto");
            return View(nota);
        }

        // POST: Notas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> Edit([Bind(Include = "Id,IdAluno,IdProfessor,IdDisciplina,Valor")] Nota nota)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nota).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.IdAluno = new SelectList(db.Users.Where(e => e.Perfil == 2 && e.NumTurma != null).Include("Turma").ToList(), "Id", "NomeCompleto");
            ViewBag.IdDisciplina = new SelectList(db.Disciplinas, "Id", "Nome");
            ViewBag.IdProfessor = new SelectList(db.Users.Where(e => e.Perfil == 1 || e.Perfil == 0), "Id", "NomeCompleto");
            return View(nota);
        }

        // GET: Notas/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Nota nota = await db.Notas.Include(e => e.Aluno).Include(e => e.Aluno.Turma).Include(e => e.Professor).Include("Disciplina").Where(e => e.Id == id).FirstAsync();

            if (nota == null)
            {
                return HttpNotFound();
            }
            return View(nota);
        }

        // POST: Notas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Nota nota = await db.Notas.Include(e => e.Aluno).Include(e => e.Aluno.Turma).Include(e => e.Professor).Include("Disciplina").Where(e => e.Id == id).FirstAsync();

            db.Notas.Remove(nota);
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
