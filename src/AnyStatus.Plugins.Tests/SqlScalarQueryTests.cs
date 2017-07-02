using AnyStatus.API;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace AnyStatus.Plugins.Tests
{
    [TestClass]
    public class SqlScalarQueryTests
    {
        [TestMethod]
        public void SqlScalarQueryTest()
        {
            var logger = Substitute.For<ILogger>();

            var sqlScalarQuery = new SqlScalarQuery
            {
                ConnectionString = "Server=tcp:{your-database}.database.windows.net,1433;Initial Catalog=AnyStatus;Persist Security Info=False;User ID={your-user};Password={your-password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;",
                SqlQuery = "SELECT Count(1) FROM [dbo].[Table]"
            };

            var sqlScalarQueryHandler = new SqlScalarQueryMonitor(logger);

            sqlScalarQueryHandler.Handle(sqlScalarQuery);

            Assert.AreNotEqual(State.None, sqlScalarQuery.State);
        }
    }
}
