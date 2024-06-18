
using CurrencyExchange.Data;
using CurrencyExchange.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CurrencyExchange.Services
{
    public interface ICurrencyConverterService
    {
        decimal Convert(decimal amount, Guid fromCurrencyID, Guid toCurrencyID);
    }

    public class CurrencyConverterService : ICurrencyConverterService
    {
        private readonly CurrencyExchangeDbContext _context;

        public CurrencyConverterService(CurrencyExchangeDbContext context)
        {
            _context = context;
        }

        public decimal Convert(decimal amount, Guid fromCurrencyID, Guid toCurrencyID)
        {
            // Fetch exchange rates from the database
            var exchangeRates = _context.ExchangeRates
                                        .Include(e => e.Currency)
                                        .ToList();

            // Find the exchange rate for the specified currencies
            var fromCurrencyRate = exchangeRates.FirstOrDefault(e => e.CurrencyID == fromCurrencyID);
            var toCurrencyRate = exchangeRates.FirstOrDefault(e => e.CurrencyID == toCurrencyID);

            if (fromCurrencyRate == null || toCurrencyRate == null)
            {
                throw new ArgumentException("Invalid currency IDs provided.");
            }

            // Perform conversion based on the fetched exchange rates
            decimal convertedAmount = (amount / fromCurrencyRate.BuyRate) * toCurrencyRate.BuyRate;

            return convertedAmount;
        }
    }
}