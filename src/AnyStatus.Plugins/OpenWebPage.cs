using AnyStatus.API;
using System;

namespace AnyStatus
{
    public class OpenWebPage<TWebPage> : RequestHandler<OpenWebPageRequest<TWebPage>>, IOpenWebPage<TWebPage> where TWebPage : IWebPage
    {
        private readonly IProcessStarter _ps;

        public OpenWebPage(IProcessStarter ps)
        {
            _ps = Preconditions.CheckNotNull(ps, nameof(ps));
        }

        protected override void HandleCore(OpenWebPageRequest<TWebPage> request)
        {
            var uri = request.DataContext.URL;

            if (!Uri.IsWellFormedUriString(uri, UriKind.Absolute))
            {
                throw new UriFormatException(uri);
            }

            _ps.Start(uri);
        }
    }
}