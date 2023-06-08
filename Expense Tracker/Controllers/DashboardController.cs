using Expense_Tracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Common;
using System.Globalization;

namespace Expense_Tracker.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }
    
        public async Task<IActionResult> Index()
        {

            //last 7 dyas
            DateTime StartDate = DateTime.Today.AddDays(-6);
            DateTime EndDate = DateTime.Today;

            List<Transaction> SelectedTranscations = await _context.Transactions.
                Include(x => x.Category)
                .Where(y => y.Date >= StartDate && y.Date <= EndDate).ToListAsync();

            //Total Income

            int TotalIncome = SelectedTranscations.Where(i => i.Category.Type == "Income")
                .Sum(j => j.Amount);

            ViewBag.TotalIncome = TotalIncome.ToString("C0");



            //Total Expense

            int TotalExpense = SelectedTranscations.Where(i => i.Category.Type == "Expense")
                .Sum(j => j.Amount);

            ViewBag.TotalExpense = TotalExpense.ToString("C0");



            //Balance
            int Balance = TotalIncome - TotalExpense;
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
            culture.NumberFormat.CurrencyNegativePattern = 1;
            ViewBag.Balance = String.Format(culture,"{0:C0}",Balance);


            //Doughnut Chart - expense By category

            ViewBag.DoughnutChartData = SelectedTranscations
                .Where(i => i.Category.Type == "Expense")
                .GroupBy(j => j.Category.CategoryId)
                .Select(k => new
                {
                    categoryTitleWithIcon = k.First().Category.Icon + " " + k.First().Category.Title,
                    amount = k.Sum(j => j.Amount),
                    formattedAmount = k.Sum(j => j.Amount).ToString("C0")
                })
                .OrderByDescending(l=>l.amount)
                .ToList();

            //Spline chart - TotalIncome vs expense

            //Income
            List<SplineChartData> IncomeSummary = SelectedTranscations
                .Where(i => i.Category.Type == "Income")
                .GroupBy(j => j.Date)
                .Select(k => new SplineChartData()
                {
                    day = k.First().Date.ToString("dd-MMM"),
                    income = k.Sum(l => l.Amount)
                })
                .ToList();


            //Expense
            List<SplineChartData> ExpenseSummary = SelectedTranscations
                .Where(i => i.Category.Type == "Expense")
                .GroupBy(j => j.Date)
                .Select(k => new SplineChartData()
                {
                    day = k.First().Date.ToString("dd-MMM"),
                    expense = k.Sum(l => l.Amount)
                })
                .ToList();

            //combine income & expense

            string[] last7Days = Enumerable.Range(0, 7)
                .Select(i => StartDate.AddDays(i).ToString("dd-MMM"))
                .ToArray();


            ViewBag.SplineChartData = from day in last7Days
                                      join income in IncomeSummary on day equals income.day into dayIncomeJoined
                                      from income in dayIncomeJoined.DefaultIfEmpty()
                                      join expense in ExpenseSummary on day equals expense.day into dayExpenseJoined
                                      from expense in dayExpenseJoined.DefaultIfEmpty()
                                      select new
                                      {
                                          day = day,
                                          income = income == null ? 0 : income.income,
                                          expense = expense == null ? 0 : expense.expense
                                      };



            //Recent transcations

            ViewBag.RecentTransactions = await _context.Transactions
                .Include(i => i.Category)
                .OrderByDescending(j => j.Date)
                .Take(5)
                .ToListAsync();




            return View();
        }

       
    }

    public class SplineChartData
    {
        public string day{ get; set; }
        public int income{ get; set; }
        public int expense{ get; set; }
    }
}
