﻿using System;
using Bashi.Tests.Framework.Data;
using Mandarin.ViewModels.Home.Carousel;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Mandarin.ViewModels.Tests.Home.Carousel
{
    [TestFixture]
    public class CarouselViewModelTests
    {
        [Test]
        public void Images_ShouldGetUrlAndDescriptionFromModel()
        {
            var data = new
            {
                Home = new
                {
                    Carousel = new[]
                    {
                        new
                        {
                            Url = "/static/images/about/ShopFront.jpg",
                            Description = "The Little Mandarin - Shop Front",
                        },
                    },
                },
            };

            var pageContentModel = new PageContentModel(null, JToken.FromObject(data));
            var subject = new CarouselViewModel(pageContentModel);
            Assert.That(subject.Images[0].SourceUrl.ToString(), Is.EqualTo(data.Home.Carousel[0].Url));
            Assert.That(subject.Images[0].Description, Is.EqualTo(data.Home.Carousel[0].Description));
        }

        [Test]
        public void Images_ShouldGetGetCorrectCount()
        {
            var data = new
            {
                Home = new
                {
                    Carousel = new[]
                    {
                        new
                        {
                            Url = TestData.Create<Uri>().ToString(),
                            Description = TestData.Create<string>(),
                        },
                        new
                        {
                            Url = TestData.Create<Uri>().ToString(),
                            Description = TestData.Create<string>(),
                        },
                        new
                        {
                            Url = TestData.Create<Uri>().ToString(),
                            Description = TestData.Create<string>(),
                        },
                    },
                },
            };

            var pageContentModel = new PageContentModel(null, JToken.FromObject(data));
            var subject = new CarouselViewModel(pageContentModel);
            Assert.That(subject.Images, Has.Count.EqualTo(3));
        }
    }
}