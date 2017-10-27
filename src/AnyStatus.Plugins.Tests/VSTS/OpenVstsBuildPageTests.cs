using AnyStatus.API;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace AnyStatus.Plugins.Tests.VSTS
{
    [TestClass]
    public class OpenVstsBuildPageTests
    {
        [TestMethod]
        public void ShouldOpenBuildDefinitionWebPage()
        {
            var processstarter = Substitute.For<IProcessStarter>();

            var vstsBuild = new VSTSBuild_v1
            {
                Account = "account",
                Project = "project",
                DefinitionId = 1
            };

            var openVstsBuildPage = new OpenVstsBuildPage(processstarter);

            openVstsBuildPage.Handle(vstsBuild);

            var expected = "https://account.visualstudio.com/project/_build/index?definitionId=1&_a=completed";

            processstarter.Received().Start(expected);
        }
    }
}

