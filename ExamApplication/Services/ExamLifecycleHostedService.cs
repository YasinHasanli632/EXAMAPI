using ExamApplication.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Services
{
    public class ExamLifecycleHostedService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public ExamLifecycleHostedService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();

                var accessCodeService = scope.ServiceProvider.GetRequiredService<IExamAccessCodeService>();

                // YENI
                await accessCodeService.GenerateCodesForUpcomingExamsAsync(stoppingToken);

                // YENI
                await accessCodeService.AutoCloseExpiredStudentExamsAsync(stoppingToken);

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
