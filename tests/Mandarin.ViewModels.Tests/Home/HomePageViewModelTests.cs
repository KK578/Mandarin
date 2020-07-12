using System;
using System.Collections.Generic;
using Bashi.Tests.Framework.Data;
using Mandarin.ViewModels.Home;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Mandarin.ViewModels.Tests.Home
{
    [TestFixture]
    public class HomePageViewModelTests
    {
        [Test]
        public void AllPropertiesShouldReadDataFromModel()
        {
            var pageContentModel = HomePageViewModelTests.CreatePageContentModel();
            var subject = new HomePageViewModel(pageContentModel);

            Assert.That(subject.CarouselViewModel, Is.Not.Null);
            Assert.That(subject.MapViewModel, Is.Not.Null);
            Assert.That(subject.OpeningTimesViewModel, Is.Not.Null);
        }

        private static PageContentModel CreatePageContentModel()
        {
            var data = new
            {
                Home = new
                {
                    Carousel = TestData.Create<List<ImageUrlModel>>(),
                    GiftCards = new
                    {
                        Text = TestData.NextString(),
                        AnimationImage = TestData.Create<ImageUrlModel>(),
                    },
                    Map = new
                    {
                        Url = TestData.Create<Uri>().ToString(),
                        Width = TestData.NextInt(),
                        Height = TestData.NextInt(),
                    },
                    OpeningTimes = TestData.Create<List<OpeningTimeModel>>(),
                },
            };

            return new PageContentModel(null, JToken.FromObject(data));
        }
    }
}
