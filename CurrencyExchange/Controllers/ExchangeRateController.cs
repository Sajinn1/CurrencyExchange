﻿using CurrencyExchange.Data;
using CurrencyExchange.Entities;
using CurrencyExchange.Models;
using CurrencyExchange.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyExchange.Controllers
{
    public class ExchangeRateController : Controller
    {
        private readonly CurrencyExchangeDbContext _context;
        private readonly ICurrencyConverterService converterService;

        public ExchangeRateController(ICurrencyConverterService converterService, CurrencyExchangeDbContext context)
        {
            this.converterService = converterService;
            _context = context;
        }

        [HttpGet]
        public IActionResult Converter()
        {
            // Fetch Currencies for dropdowm
            ViewBag.Currencies = GetCurrencySelectListItems();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Converter(decimal amount, Guid fromCurrencyID, Guid toCurrencyID)
        {
            // Validate currency IDs
            var fromCurrency = _context.Currencies.FirstOrDefault(c => c.ID == fromCurrencyID);
            var toCurrency = _context.Currencies.FirstOrDefault(c => c.ID == toCurrencyID);

            if (fromCurrency == null || toCurrency == null)
            {
                ModelState.AddModelError("", "Invalid currency IDs provided.");
                ViewBag.Currencies = GetCurrencySelectListItems();
                return View();
            }

            try
            {
                // Convert amount using the currency converter service
                var convertedAmount = converterService.Convert(amount, fromCurrencyID, toCurrencyID);

                // Set ViewBag values for display in the view
                ViewBag.ConvertedAmount = convertedAmount;
                ViewBag.ToCurrency = toCurrency.Name;

                ViewBag.Currencies = GetCurrencySelectListItems();

                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred during currency conversion.");
                // Log the exception or handle it appropriately
                return View();
            }
        }

        // Helper method to retrieve currency select list items
        private List<SelectListItem> GetCurrencySelectListItems()
        {
            return _context.Currencies
                            .Select(c => new SelectListItem
                            {
                                Value = c.ID.ToString(),
                                Text = c.Name
                            })
                            .ToList();
        }



        // GET: ExchangeRate/Add
        public IActionResult Add()
        {
            var viewModel = new AddExchangeRateViewModel
            {
                Currencies = _context.Currencies
                                    .Select(c => new SelectListItem
                                    {
                                        Value = c.ID.ToString(),
                                        Text = c.Name
                                    })
                                    .ToList()
            };

            return View(viewModel);
        }

        // POST: ExchangeRate/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddExchangeRateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var exchangeRate = new ExchangeRate
                {
                    Id = Guid.NewGuid(),
                    CurrencyID = viewModel.CurrencyID,
                    BuyRate = viewModel.BuyRate,
                    SalesRate = viewModel.SalesRate,
                    User = viewModel.User,
                    CommiPer = viewModel.CommiPer,
                    CommiRs = viewModel.CommiRs
                };

                _context.ExchangeRates.Add(exchangeRate);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // If ModelState is not valid, reload the dropdown list
            viewModel.Currencies = _context.Currencies
                                            .Select(c => new SelectListItem
                                            {
                                                Value = c.ID.ToString(),
                                                Text = c.Name
                                            })
                                            .ToList();

            return View(viewModel);
        }
        public async Task<IActionResult> Index()
        {
            var exchangeRates = await _context.ExchangeRates
                                              .Include(e => e.Currency)
                                              .ToListAsync();
            return View(exchangeRates);
        }



        // GET: ExchangeRate/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var exchangeRate = await _context.ExchangeRates
                                             .Include(e => e.Currency)
                                             .FirstOrDefaultAsync(e => e.Id == id);

            if (exchangeRate == null)
            {
                return NotFound();
            }

            var viewModel = new AddExchangeRateViewModel
            {
                Id = exchangeRate.Id,
                CurrencyID = exchangeRate.CurrencyID,
                Currency = exchangeRate.Currency,
                BuyRate = exchangeRate.BuyRate,
                SalesRate = exchangeRate.SalesRate,
                User = exchangeRate.User,
                CommiPer = exchangeRate.CommiPer,
                CommiRs = exchangeRate.CommiRs,
                Currencies = _context.Currencies
                                     .Select(c => new SelectListItem
                                     {
                                         Value = c.ID.ToString(),
                                         Text = c.Name
                                     })
                                     .ToList()
            };

            return View(viewModel);
        }

        // POST: ExchangeRate/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AddExchangeRateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var exchangeRate = await _context.ExchangeRates.FindAsync(viewModel.Id);

                if (exchangeRate == null)
                {
                    return NotFound();
                }

                exchangeRate.CurrencyID = viewModel.CurrencyID;
                exchangeRate.BuyRate = viewModel.BuyRate;
                exchangeRate.SalesRate = viewModel.SalesRate;
                exchangeRate.User = viewModel.User;
                exchangeRate.CommiPer = viewModel.CommiPer;
                exchangeRate.CommiRs = viewModel.CommiRs;

                try
                {
                    _context.Update(exchangeRate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExchangeRateExists(viewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            // If ModelState is not valid, reload the dropdown list
            viewModel.Currencies = _context.Currencies
                                           .Select(c => new SelectListItem
                                           {
                                               Value = c.ID.ToString(),
                                               Text = c.Name
                                           })
                                           .ToList();

            return View(viewModel);
        }

        private bool ExchangeRateExists(Guid id)
        {
            return _context.ExchangeRates.Any(e => e.Id == id);
        }

        // GET: ExchangeRate/Delete/{id}
        [HttpGet]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exchangeRate = await _context.ExchangeRates
                                               .Include(e => e.Currency)
                                               .FirstOrDefaultAsync(e => e.Id == id);

            if (exchangeRate == null)
            {
                return NotFound();
            }

            return View(exchangeRate);
        }

        // POST: ExchangeRate/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var exchangeRate = await _context.ExchangeRates.FindAsync(id);
            if (exchangeRate == null)
            {
                return NotFound();
            }

            _context.ExchangeRates.Remove(exchangeRate);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


    }
}

