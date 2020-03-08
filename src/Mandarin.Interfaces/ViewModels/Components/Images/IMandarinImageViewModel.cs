using System;

namespace Mandarin.ViewModels.Components.Images
{
    public interface IMandarinImageViewModel
    {
        Uri SourceUrl { get; }
        string Description { get; }
    }
}
