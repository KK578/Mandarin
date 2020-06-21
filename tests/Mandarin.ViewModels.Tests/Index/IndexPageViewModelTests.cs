using System;
using System.Collections.Generic;
using Bashi.Tests.Framework.Data;
using Mandarin.ViewModels.Index;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Mandarin.ViewModels.Tests.Index
{
    [TestFixture]
    public class IndexPageViewModelTests
    {
        [Test]
        public void MainContent_ShouldBeHtmlString()
        {
            // TODO: Remove unrelated data to test value.
            //       There is a lot of unrelated data object due to the dependency tree...
            var data = new
            {
                About = new
                {
                    Carousel = TestData.Create<List<ImageUrlModel>>(),
                    MainText = "This *is* markdown!.",
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

            var pageContentModel = new PageContentModel(null, JToken.FromObject(data));
            var subject = new IndexPageViewModel(pageContentModel);
            Assert.That(subject.MainContent.Value, Is.EqualTo("<p>This <em>is</em> markdown!.</p>\n"));
        }
    }
}
