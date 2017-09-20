using AnyStatus.API;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;

namespace AnyStatus
{
    public class JenkinsViewMonitor : IMonitor<JenkinsView_v1>
    {
        [DebuggerStepThrough]
        public void Handle(JenkinsView_v1 item)
        {
            if (item.Parent == null) return;

            var jenkinsClient = new JenkinsClient();

            var build = jenkinsClient.GetViewAsync(item).Result;

            item.State = State.Ok;

            // HACK: This adds items, but requires a restart before "schedulers" are created by AnyStatus.
            var prevJobs = item.Parent.Items.OfType<JenkinsJob_v1>();

            Application.Current.Dispatcher.Invoke(() =>
            {
                // Add new jobs
                var newJobs = build.Jobs.Where(x => prevJobs.All(y => y.URL != x.URL)).OrderBy(x => x.Name);

                foreach (var job in newJobs)
                {
                    this.AddJob(item, job);
                }

                // Update existing jobs
                foreach (var job in build.Jobs.Except(newJobs))
                {
                    this.UpdateJob(prevJobs.Single(x => x.URL == job.URL), job);
                }

                // Remove any jobs that no longer exist
                foreach (var job in prevJobs.Where(x => build.Jobs.All(y => y.URL != x.URL)))
                {
                    this.RemoveJob(item, job);
                }
            });
        }

        private void AddJob(JenkinsView_v1 item, JenkinsJob job)
        {
            item.Add(new JenkinsJob_v1
            {
                Name = job.Name.Replace("%2F", "/"),
                URL = job.URL,
                Interval = item.Interval,
                UserName = item.UserName,
                ApiToken = item.ApiToken,
                IgnoreSslErrors = item.IgnoreSslErrors
            });
        }

        private void UpdateJob(JenkinsJob_v1 item, JenkinsJob job)
        {
            // TODO: not sure what to update... => Update Item.State
        }

        private void RemoveJob(JenkinsView_v1 item, JenkinsJob_v1 job)
        {
            item.Remove(job);
        }

        
    }
    
}