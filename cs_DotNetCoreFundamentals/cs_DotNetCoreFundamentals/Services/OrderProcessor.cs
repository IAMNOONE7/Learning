using cs_DotNetCoreFundamentals.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_DotNetCoreFundamentals.Services
{
    // This is the class that shows DI, logging and configuration together.
    public class OrderProcessor : IOrderProcessor
    {
        private readonly ILogger<OrderProcessor> _logger;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly AppSettings _settings;

        public OrderProcessor(
            ILogger<OrderProcessor> logger,
            IDateTimeProvider dateTimeProvider,
            IOptions<AppSettings> settings)
        {
            _logger = logger;
            _dateTimeProvider = dateTimeProvider;
            _settings = settings.Value;
        }

        public Task ProcessAsync()
        {
            _logger.LogInformation("=== Order processing started at {time} ===", _dateTimeProvider.Now);
            _logger.LogInformation("RunMode: {mode} | ProcessedBy: {by}", _settings.RunMode, _settings.ProcessedBy);

            // Simulate processing 3 orders
            for (int i = 1; i <= 3; i++)
            {
                _logger.LogInformation("Processing order with Id={id}", i);
            }

            _logger.LogInformation("=== Order processing finished at {time} ===", _dateTimeProvider.Now);

            return Task.CompletedTask;
        }
    }
}
