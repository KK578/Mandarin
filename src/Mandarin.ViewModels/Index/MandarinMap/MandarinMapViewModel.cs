using System;

namespace Mandarin.ViewModels.Index.MandarinMap
{
    internal sealed class MandarinMapViewModel : IMandarinMapViewModel
    {
        public MandarinMapViewModel()
        {
            MapUri = new Uri("https://www.google.com/maps/embed?pb=!1m14!1m8!1m3!1d619.8081400436498!2d-0.013219!3d51.582301!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x0%3A0x541a94670a2a54e2!2sThe%20Little%20Mandarin!5e0!3m2!1sen!2suk!4v1583077967669!5m2!1sen!2suk");
            Width = 400;
            Height = 300;
        }

        public Uri MapUri { get; }
        public int Width { get; }
        public int Height { get; }
    }
}
