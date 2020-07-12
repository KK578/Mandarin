using Bashi.Tests.Framework.Data;
using Mandarin.ViewModels.Home.OpeningTimes;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Mandarin.ViewModels.Tests.Home.OpeningTimes
{
    [TestFixture]
    public class OpeningTimesViewModelTests
    {
        [Test]
        public void OpeningTimes_GivenDateTimesAreSet_ShouldGetMessageFromTimes()
        {
            var data = new
            {
                Home = new
                {
                    OpeningTimes = new[]
                    {
                        new
                        {
                            Name = "Monday",
                            Open = "2020-06-20T09:00:00Z",
                            Close = "2020-06-20T17:00:00Z",
                        },
                    },
                },
            };

            var pageContentModel = new PageContentModel(null, JToken.FromObject(data));
            var subject = new OpeningTimesViewModel(pageContentModel);
            Assert.That(subject.OpeningTimes[0].NameOfDay, Is.EqualTo("Monday"));
            Assert.That(subject.OpeningTimes[0].Message, Is.EqualTo("9:00am - 5:00pm"));
        }

        [Test]
        public void OpeningTimes_GivenMessageIsSet_ShouldGetMessageAsIs()
        {
            var data = new
            {
                Home = new
                {
                    OpeningTimes = new[]
                    {
                        new
                        {
                            Name = "Monday",
                            Message = TestData.WellKnownString,
                            Open = "2020-06-20T09:00:00Z",
                            Close = "2020-06-20T17:00:00Z",
                        },
                    },
                },
            };

            var pageContentModel = new PageContentModel(null, JToken.FromObject(data));
            var subject = new OpeningTimesViewModel(pageContentModel);
            Assert.That(subject.OpeningTimes[0].Message, Is.EqualTo("9:00am - 5:00pm"));
        }

        [Test]
        public void OpeningTimes_GivenMessageAndTimesAreSet_ShouldGetMessageFromTimes()
        {
            var data = new
            {
                Home = new
                {
                    OpeningTimes = new[]
                    {
                        new
                        {
                            Name = "Monday",
                            Message = TestData.WellKnownString,
                        },
                    },
                },
            };

            var pageContentModel = new PageContentModel(null, JToken.FromObject(data));
            var subject = new OpeningTimesViewModel(pageContentModel);
            Assert.That(subject.OpeningTimes[0].Message, Is.EqualTo(TestData.WellKnownString));
        }

        [Test]
        public void OpeningTimes_GivenMultipleOpeningTimes_ShouldRenderAllItems()
        {
            var data = new
            {
                Home = new
                {
                    OpeningTimes = new[]
                    {
                        new
                        {
                            Name = "Monday",
                            Message = TestData.WellKnownString,
                        },
                        new
                        {
                            Name = "Tuesday",
                            Message = TestData.WellKnownString,
                        },
                        new
                        {
                            Name = "Wednesday",
                            Message = TestData.WellKnownString,
                        },
                        new
                        {
                            Name = "Thursday",
                            Message = TestData.WellKnownString,
                        },
                        new
                        {
                            Name = "Friday",
                            Message = TestData.WellKnownString,
                        },
                        new
                        {
                            Name = "Saturday",
                            Message = TestData.WellKnownString,
                        },
                        new
                        {
                            Name = "Sunday",
                            Message = TestData.WellKnownString,
                        },
                    },
                },
            };

            var pageContentModel = new PageContentModel(null, JToken.FromObject(data));
            var subject = new OpeningTimesViewModel(pageContentModel);
            Assert.That(subject.OpeningTimes, Has.Count.EqualTo(7));
        }
    }
}
