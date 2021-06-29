using BookListMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookListMVC.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDBContext _Db;
        [BindProperty]
        public Book Book { get; set; }


        public BooksController(ApplicationDBContext applicationDB)
        {
            _Db = applicationDB;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            Book = new Book();
            if (id==null)
            {
                //Creating a book
                return View(Book);
            }

            //Updating BOok
            Book = _Db.Books.FirstOrDefault(z=>z.Id == id);
            if (Book == null)
            {
                return NotFound();
            }
            return View(Book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert()
        {
            if (ModelState.IsValid)
            {
                if (Book.Id == 0)
                {
                    //Create
                    _Db.Books.Add(Book);
                }
                else
                {
                    _Db.Books.Update(Book);
                }
                _Db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(Book);
        }



        #region API Calls


        [HttpGet]

        public async Task<IActionResult> GetAll()
        {
            return Json(new { data = await _Db.Books.ToListAsync() });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {

            var bookDB = await _Db.Books.FirstOrDefaultAsync(x => x.Id == id);
            if (bookDB == null)
            {
                return Json(new { success = false, message = "Error While Deleting" });
            }
            _Db.Books.Remove(bookDB);
            await _Db.SaveChangesAsync();
            return Json(new { success = true, message = "Delete Successfully" });
        }

        #endregion
    }
}
