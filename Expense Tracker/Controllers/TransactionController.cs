using Expense_Tracker.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Expense_Tracker.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ApplicationDbContext _context;
        public TransactionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Transaction
        public async Task<ActionResult> Index()
        {
            var applicationDbContext = _context.Transactions.Include(t => t.Category);
             return View(await applicationDbContext.ToListAsync());
        }
        
        // GET: TransactionController/AddOrEdit
        public ActionResult AddOrEdit(int id=0)
        {
            PopulateCategories();
            //ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId");
            if (id == 0)
                return View(new Transaction());
            else
                return View(_context.Transactions.Find(id));
        }

        // POST: TransactionController/AddOrEdit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("TransactionId,CategoryId,Amount,Note,Date")]Transaction transaction)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (transaction.TransactionId == 0)
                        _context.Add(transaction);
                    else
                        _context.Update(transaction);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                PopulateCategories();
                return View(transaction);
            }
            catch
            {
                return View();
            }
        }

        // POST: TransactionController/Delete/5
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, IFormCollection collection)
        {
            try
            {
                if (_context.Transactions == null)
                    return Problem("Entity set 'ApplicationDbContext.Transcations' is null");

                var transcation = await _context.Transactions.FindAsync(id);

                if (transcation != null)
                    _context.Transactions.Remove(transcation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [NonAction]
        public void PopulateCategories()
        {
            var CategoryCollection = _context.Categories.ToList();
            Category Defaultcategory = new Category() { CategoryId = 0, Title = "Choose category" };
            CategoryCollection.Insert(0, Defaultcategory);
            ViewBag.Categories = CategoryCollection;
        }
    }
}
