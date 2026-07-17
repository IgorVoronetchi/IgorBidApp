using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services
{
    /// <summary>
    /// Ruleaza periodic tranzitiile din ciclul de viata al licitatiilor:
    ///   Validated -> ActiveBid   (cand StartDate a trecut)
    ///   ActiveBid -> Sold        (EndDate a trecut si exista oferte -> se seteaza Winner)
    ///   ActiveBid -> NoWinner    (EndDate a trecut fara oferte)
    /// </summary>
    public class AuctionLifecycleService(IServiceScopeFactory scopeFactory) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = scopeFactory.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    await RunTransitionsAsync(db);
                }
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

        public static async Task RunTransitionsAsync(ApplicationDbContext db)
        {
            var now = DateTime.UtcNow;

            var toActivate = await db.AuctionItems
                .Where(i => i.Status == StatusEnum.Validated && i.StartDate <= now)
                .ToListAsync();
            foreach (var item in toActivate)
                item.Status = StatusEnum.ActiveBid;

            var toFinish = await db.AuctionItems
                .Include(i => i.Bids)
                .Where(i => i.Status == StatusEnum.ActiveBid && i.EndDate <= now)
                .ToListAsync();

            foreach (var item in toFinish)
            {
                var topBid = item.Bids.OrderByDescending(b => b.Amount).FirstOrDefault();
                if (topBid != null)
                {
                    item.Status = StatusEnum.Sold;
                    item.WinnerId = topBid.BidderId;
                    item.CurrentPrice = topBid.Amount;
                }
                else
                {
                    item.Status = StatusEnum.NoWinner;
                }
            }

            if (toActivate.Count > 0 || toFinish.Count > 0)
                await db.SaveChangesAsync();
        }
    }
}
