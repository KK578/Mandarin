using System;
using Bashi.Tests.Framework.Data;
using Mandarin.ViewModels.Home.MandarinMap;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Mandarin.ViewModels.Tests.Home.MandarinMap
{
    [TestFixture]
    public class MandarinMapViewModelTests
    {
        [Test]
        public void MapUri_ShouldGetDataFromModel()
        {
            var data = new
            {
                Home = new
                {
                    Map = new
                    {
                        Url = TestData.Create<Uri>().ToString(),
                        Width = TestData.Create<int>(),
                        Height = TestData.Create<int>(),
                    },
                },
            };

            var pageContentModel = new PageContentModel(null, JToken.FromObject(data));
            var subject = new MandarinMapViewModel(pageContentModel);

            Assert.That(subject.MapUri.ToString(), Is.EqualTo(data.Home.Map.Url));
            Assert.That(subject.Width, Is.EqualTo(data.Home.Map.Width));
            Assert.That(subject.Height, Is.EqualTo(data.Home.Map.Height));
        }
    }
}
