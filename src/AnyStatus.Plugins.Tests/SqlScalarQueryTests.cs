using AnyStatus.API;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus.Plugins.Tests
{
    [TestClass]
    public class SqlScalarQueryTests
    {
        [TestMethod]
        public async Task SqlScalarQueryTest()
        {
            var logger = Substitute.For<ILogger>();

            var sqlScalarQuery = new SqlScalarQuery
            {
                SqlQuery = "SELECT Count(1) FROM [dbo].[Table]",
                ConnectionString = "Server=tcp:{your-database}.database.windows.net,1433;Initial Catalog=AnyStatus;Persist Security Info=False;User ID={your-user};Password={your-password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=1;",
            };

            var request = MetricQueryRequest.Create(sqlScalarQuery);

            var handler = new SqlScalarQueryHandler(logger);

            await handler.Handle(request, CancellationToken.None);

            Assert.AreNotEqual(State.None, sqlScalarQuery.State);
        }
    }
}