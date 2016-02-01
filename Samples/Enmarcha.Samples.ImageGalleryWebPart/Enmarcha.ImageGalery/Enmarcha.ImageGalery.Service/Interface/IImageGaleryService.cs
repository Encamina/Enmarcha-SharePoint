using System.Collections.Generic;
using Enmarcha.ImageGalery.Model;

namespace Enmarcha.ImageGalery.Service.Interface
{
    public interface IImageGaleryService
    {
         IList<ImageGallery> GetNews();
    }
}
