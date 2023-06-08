using Expense_Tracker.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Expense_Tracker.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: Category
        public async Task<IActionResult> Index()
        {
            return _context.Categories != null ?
                        View(await _context.Categories.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.Categories'  is null.");
        }

        // GET: CategoryController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CategoryController/AddOrEdit
        public ActionResult AddOrEdit(int id=0)
        {
            if (id == 0)
                return View(new Category());
            else
                return View(_context.Categories.Find(id));
        }

        // POST: CategoryController/AddOrEdit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(Category category)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (category.CategoryId == 0)
                        _context.Add(category);
                    else
                        _context.Update(category);
                await _context.SaveChangesAsync();


                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        
        // GET: CategoryController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CategoryController/Delete/5
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                if (_context.Categories == null)
                    return Problem("Entrity set 'ApplicationDbContext.Categories' is null");

                var category = await _context.Categories.FindAsync(id);
                if (category != null)
                    _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
