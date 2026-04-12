using Investissement_WebClient.Core.InterfacesServices;
using Investissement_WebClient.Core.Modeles;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Investissement_WebClient.Data.Services
{
    public class InvestissementService : IInvestissementService
    {
        private readonly IDbContextFactory<InvestissementDbContext> _dbFactory;

        public InvestissementService(IDbContextFactory<InvestissementDbContext> dbContext)
        {
            _dbFactory = dbContext;
        }

        public async Task<IEnumerable<TransactionVM>> GetTransactions()
        {
            //await using var context = await _dbFactory.CreateDbContextAsync();
            //return await context.Transactions.Select(t => new TransactionVM
            //{
            //    Date = t.Date ,
            //    Actif = t.Actif,
            //    Prix = t.Prix,
            //    Quantite = t.Quantite,
            //}).ToListAsync();
            return [];
        }

        public async Task AddTransactionsRange(IEnumerable<Transaction> transactions)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            foreach (var transaction in transactions)
            {
                if (await context.Transactions.AnyAsync(t => t.Id != transaction.Id))
                    await context.Transactions.AddAsync(transaction);
            }

            await context.SaveChangesAsync();
        }

        public async Task AddFluxBancairesRange(IEnumerable<FluxBancaire> fluxBancaires)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            foreach (var transaction in fluxBancaires)
            {
                if (await context.FluxBancaires.AnyAsync(t => t.Id != transaction.Id))
                    await context.FluxBancaires.AddAsync(transaction);
            }

            await context.SaveChangesAsync();
        }
    }
}
