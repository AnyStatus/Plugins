using AnyStatus.API;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace AnyStatus
{
    public class JenkinsViewStatus : ICheckHealth<JenkinsView_v1>
    {
        private readonly ILogger _logger;

        public JenkinsViewStatus(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException();
        }

        [DebuggerStepThrough]
        public async Task Handle(HealthCheckRequest<JenkinsView_v1> request, CancellationToken cancellationToken)
        {
            var jenkinsClient = new JenkinsClient(_logger);

            var jenkinsViewResponse = await jenkinsClient.GetViewAsync(request.DataContext).ConfigureAwait(false);

            var prevJobs = request.DataContext.Items.OfType<JenkinsJob_v1>();

            var dispatcher = Application.Current != null ? Application.Current.Dispatcher : Dispatcher.CurrentDispatcher;

            dispatcher.Invoke(() =>
            {
                // Add Jobs
                var newJobs = jenkinsViewResponse.Jobs.Where(x => prevJobs.All(y => y.URL != x.URL)).OrderBy(x => x.Name);

                foreach (var job in newJobs)
                {
                    AddJob(request.DataContext, job);
                }

                // Remove Jobs
                foreach (var job in prevJobs.Where(x => jenkinsViewResponse.Jobs.All(y => y.URL != x.URL)))
                {
                    request.DataContext.Remove(job);
                }
            });
        }

        private void AddJob(JenkinsView_v1 view, JenkinsJob job)
        {
#warning IsParameterized not set
            view.Add(new JenkinsJob_v1
            {
                Name = job.Name.Replace("%2F", "/"),
                URL = job.URL,
                Interval = view.Interval,
                UserName = view.UserName,
                ApiToken = view.ApiToken,
                CSRF = view.CSRF,
                IgnoreSslErrors = view.IgnoreSslErrors
            });
        }
    }
}