using System;

namespace Mandarin.ViewModels.Index.MandarinMap
{
    public interface IMandarinMapViewModel
    {
        Uri MapUri { get; }
        int Width { get; }
        int Height { get; }
    }
}
