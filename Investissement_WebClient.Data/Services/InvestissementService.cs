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
            await using var context = await _dbFactory.CreateDbContextAsync();
            return await context.Transactions.Select(t => new TransactionVM
            {
                Date = t.Date,
                Actif = t.Actif,
                Prix = t.Prix,
                Quantite = t.Quantite,
            }).ToListAsync();
        }
    }
}
