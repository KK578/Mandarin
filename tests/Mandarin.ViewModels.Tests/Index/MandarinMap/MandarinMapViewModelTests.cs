using System;
using Bashi.Tests.Framework.Data;
using Mandarin.ViewModels.Index.MandarinMap;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Mandarin.ViewModels.Tests.Index.MandarinMap
{
    [TestFixture]
    public class MandarinMapViewModelTests
    {
        [Test]
        public void MapUri_ShouldGetDataFromModel()
        {
            var data = new
            {
                About = new
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

            Assert.That(subject.MapUri.ToString(), Is.EqualTo(data.About.Map.Url));
            Assert.That(subject.Width, Is.EqualTo(data.About.Map.Width));
            Assert.That(subject.Height, Is.EqualTo(data.About.Map.Height));
        }
    }
}
