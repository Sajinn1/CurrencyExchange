using CurrencyExchange.Data;
using CurrencyExchange.Entities;
using CurrencyExchange.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CurrencyExchange.Controllers
{
    public class CurrencyController : Controller
    {
        private readonly CurrencyExchangeDbContext _context;

        public CurrencyController(CurrencyExchangeDbContext context)
        {
            _context = context;
        }

        // GET for Index 
        [HttpGet]
        public IActionResult Index()
        {
            var currencies = _context.Currencies.ToList();
            return View(currencies);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }


        // POST For Add
        [HttpPost]
        public async Task<IActionResult> Add(AddCurrencyViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var currency = new Currency
                {
                    ID = Guid.NewGuid(),
                    Name = viewModel.Name,
                    Decimalname = viewModel.Decimalname,
                    Nondecimal = viewModel.Nondecimal,
                    Millionlakh = viewModel.Millionlakh,
                    Symbol = viewModel.Symbol
                };

                _context.Currencies.Add(currency);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            return View(viewModel);
        }

        // GET for Edit 
        [HttpGet]
        public IActionResult Edit(Guid id)
        {
            var currency = _context.Currencies.Find(id);
            if (currency == null)
            {
                return NotFound();
            }

            var viewModel = new AddCurrencyViewModel
            {
                ID = currency.ID,
                Name = currency.Name,
                Decimalname = currency.Decimalname,
                Nondecimal = currency.Nondecimal,
                Millionlakh = currency.Millionlakh,
                Symbol = currency.Symbol
            };

            return View(viewModel);
        }


        // POST for Edit 
        [HttpPost]
        public async Task<IActionResult> Edit(AddCurrencyViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var currency = await _context.Currencies.FindAsync(viewModel.ID);
                if (currency == null)
                {
                    return NotFound();
                }

                currency.Name = viewModel.Name;
                currency.Decimalname = viewModel.Decimalname;
                currency.Nondecimal = viewModel.Nondecimal;
                currency.Millionlakh = viewModel.Millionlakh;
                currency.Symbol = viewModel.Symbol;

                _context.Currencies.Update(currency);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            return View(viewModel);
        }
        // GET: Currency/Delete
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currency = await _context.Currencies
                .FirstOrDefaultAsync(m => m.ID == id);
            if (currency == null)
            {
                return NotFound();
            }

            return View(currency);
        }

        // POST: Currency/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var currency = await _context.Currencies.FindAsync(id);
            if (currency != null)
            {
                _context.Currencies.Remove(currency);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));

        }
    }
}

