namespace LegoMinifigureApp.Services.Offers
{
    public class OfferCleanupJob(IServiceScopeFactory scopeFactory, ILogger<OfferCleanupJob> logger) : BackgroundService
    {
        private static readonly TimeSpan Interval = TimeSpan.FromHours(24);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(Interval);

            do
            {
                await RunCleanupAsync(stoppingToken);
            }
            while (await timer.WaitForNextTickAsync(stoppingToken));
        }

        private async Task RunCleanupAsync(CancellationToken stoppingToken)
        {
            try
            {
                //background jobs live for the whole app, the db context only for one unit of work,
                //so a new scope is created for every run
                using var scope = scopeFactory.CreateScope();
                var offerAdminService = scope.ServiceProvider.GetRequiredService<IOfferAdminService>();

                var deletedCount = await offerAdminService.DeleteExpiredAsync(stoppingToken);

                if (deletedCount > 0)
                {
                    logger.LogInformation("Offer clean-up: permanently deleted {Count} expired offers.", deletedCount);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                //app is shutting down; nothing to do
            }
            catch (Exception ex)
            {
                //a failed run must not stop the app; the next run tries again
                logger.LogError(ex, "Offer clean-up failed.");
            }
        }
    }
}
