using System.Net;
using Mandarin.Tests.Data;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Mandarin.Tests.Mocks
{
    internal static class FruityMock
    {
        private static WireMockServer server;

        public static void EnsureStarted()
        {
            FruityMock.server ??= FruityMock.CreateServer();
        }

        private static WireMockServer CreateServer()
        {
            var wireMock = WireMockServer.Start("http://localhost:9090");
            wireMock.Given(Request.Create().WithPath("/api/stockist"))
                    .RespondWith(Response.Create()
                                         .WithStatusCode(HttpStatusCode.OK)
                                         .WithBodyFromFile(WellKnownTestData.Fruity.Stockist.TheLittleMandarin));

            return wireMock;
        }
    }
}
