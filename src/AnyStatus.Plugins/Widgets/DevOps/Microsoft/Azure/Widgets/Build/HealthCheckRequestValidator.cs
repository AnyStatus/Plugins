using AnyStatus.API;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AnyStatus.Plugins.Widgets.DevOps.Microsoft.Azure.Widgets
{
    public class AzureDevOpsBuildHealthCheckRequestValidator : IValidator<HealthCheckRequest<AzureDevOpsBuildWidget>>
    {
        private readonly ILogger _logger;

        public AzureDevOpsBuildHealthCheckRequestValidator(ILogger logger)
        {
            _logger = logger;
        }

        public IEnumerable<ValidationResult> Validate(HealthCheckRequest<AzureDevOpsBuildWidget> request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.DataContext == null)
            {
                throw new InvalidOperationException("DataContext must be specified.");
            }

            if (request.DataContext.IsInitialized)
            {
                return null;
            }

            var msg = $"{request.DataContext.Name} was not initialized.";

            _logger.Info(msg);

            return new[] { new ValidationResult(msg) };
        }
    }
}
