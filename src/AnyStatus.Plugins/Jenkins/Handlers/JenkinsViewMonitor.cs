using AnyStatus.API;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace AnyStatus
{
    public class JenkinsViewMonitor : IMonitor<JenkinsView_v1>
    {
        private readonly IJenkinsClient _jenkinsClient;

        public JenkinsViewMonitor(IJenkinsClient jenkinsClient)
        {
            _jenkinsClient = Preconditions.CheckNotNull(jenkinsClient, nameof(jenkinsClient));
        }

        [DebuggerStepThrough]
        public void Handle(JenkinsView_v1 jenkinsView)
        {
            var jenkinsViewResponse = _jenkinsClient.GetViewAsync(jenkinsView).GetAwaiter().GetResult();

            var prevJobs = jenkinsView.Items.OfType<JenkinsJob_v1>();

            var dispatcher = Application.Current != null ? Application.Current.Dispatcher : Dispatcher.CurrentDispatcher;

            dispatcher.Invoke(() =>
            {
                // Add Jobs
                var newJobs = jenkinsViewResponse.Jobs.Where(x => prevJobs.All(y => y.URL != x.URL)).OrderBy(x => x.Name);

                foreach (var job in newJobs)
                {
                    AddJob(jenkinsView, job);
                }

                // Remove Jobs
                foreach (var job in prevJobs.Where(x => jenkinsViewResponse.Jobs.All(y => y.URL != x.URL)))
                {
                    jenkinsView.Remove(job);
                }
            });
        }

        private void AddJob(JenkinsView_v1 view, JenkinsJob job)
        {
            view.Add(new JenkinsJob_v1
            {
                Name = job.Name.Replace("%2F", "/"),
                URL = job.URL,
                Interval = view.Interval,
                UserName = view.UserName,
                ApiToken = view.ApiToken,
                CSRF = view.CSRF,
                IgnoreSslErrors = view.IgnoreSslErrors
                
                #warning IsParameterized is not set
            });
        }
    }
}