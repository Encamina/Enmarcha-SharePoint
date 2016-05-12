# WebPart Galeria de Imagenes #

### Resumen ###
Este ejemplo muestra crear un WebPart Visual realizando consultas sobre listas de SharePoint. Y como aprovisionar los artefactos necesarios para el correcto funcionamiento del WebPart.

### Funciona con ###
-  SharePoint 2013 on-premises

### Prerequisitos ###
Visual Studio 2013 o superior 

### Solucion ###
Solución | Autor(s)
---------|----------
Enmarcha.ImageGalery | [Adrian Diaz Cervera](https://github.com/AdrianDiaz81) (**ENCAMINA**)

### Version history ###
Version  | Fecha | Comentarios
---------| -----| --------
1.0  | Febrero 01 2016 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

----------

## ESCENARIO ##
En este escenario vamos a crear un WebPart Visual. En primer lugar nos crearemos una Feature en la que aprovisonaremos los artefactos que nos hace falta para el correcto funcionaiento del WebPart. En segundo lugar, utilizaremos una arquitectura de N-Capas para separar la lógica de la aplicación de la interfaz de usuario. Y para finalizar, realizaremos test unitarios utilizando la herramienta [JustMock de Telerik](http://www.telerik.com/products/mocking.aspx). (para poder ejecutarlo es necesario tener licencia sobre este sofware) 

### Visual Studio ###

1.- Abrimos la solución Enmarcha.ImageGalery.sln con Visual Studio 2013/ Visual Studio 2015

2.- Restauramos los paquetes Nuget de la Solución

### Feature Main ###
1.- Abrimos el Event Receiver asociado a la Feature Main (Main.eventReceiver.cs). Dentro de este código esta la creación de las Columnas de sitio, tipos de contenido y lista de ImageGalery.

```C#
var site = properties.Feature.Parent as SPSite;
var web = site.RootWeb;
ILog log = new LogManager().GetLogger(new StackTrace().GetFrame(0)); ;
var columnSiteCollection = web.CreateColumnSite("Image Galery", typeof(ImageGallery));
web.CreateContentType(Constants.ContentType.ImageGallery, "Enmarcha ContentType", "Elemento", columnSiteCollection);
web.CreateList(Constants.List.ImageGallery, "Lista de la galeria de imagenes", TypeList.GenericList, true);
var list = web.Lists.TryGetList(Constants.List.ImageGallery);
if (list != null)
{
 list.AddContentTypeLibrary("Image Galery");
}
```
2.- En la propia feature, se despliega los ficheros javascript y css que utiliza el WebPArt, para ello en la carpeta Module hay un modulo Style Library donde se despliegua estos elementos. 

3.- En el CodeBehind del WebPart vamos a realizar la llamada a nuestro servicio de [ImageGalery](https://github.com/Encamina/Enmarcha-SharePoint/tree/master/Samples/Enmarcha.Samples.ImageGalleryWebPart/Enmarcha.ImageGalery.Service)
```C#
 private void LoadData()
 {
 try
  {
  var listSharePoint = SPContext.Current.Web.Lists.TryGetList(Constants.List.ImageGallery);
  var imageGaleryService = new ImageGaleryService(listSharePoint, 5);
  var imageGaleryCollection = imageGaleryService.GetNews();
  listViewImageGalery.DataSource = imageGaleryCollection;
  listViewImageGalery.DataBind();
 }
 catch (Exception exception)
   {
     Logger.Error(string.Concat("Error Concat LoadData",exception.Message));
   }
 }
```
### Test ###
1.- La Api de SharePoint esta cerrada por lo que para realizar test unitarios es necesario bien hacer un Mock sobre SharePoint o bien utilizar una herramienta de terceros en nuestros caso hemos utilzado JustMock de Telerik
```C#
 [TestMethod]
 public void GetNews()
 {
  var fakeSiteUrl = "http://www.telerik.com";
  var fakeSharepointSite = Mock.Create<SPSite>();
  var fakeSharePointList = Mock.Create<SPList>();
 
  Mock.Arrange(() => SPContext.Current.Site).Returns(fakeSharepointSite);
  Mock.Arrange(() => fakeSharepointSite.RootWeb.Lists.TryGetList("demo")).Returns(fakeSharePointList);

  var service = new ImageGaleryService(fakeSharePointList, 10);            
  Mock.Arrange(() => service.GetNews()).Returns(new List<ImageGallery>
  { new ImageGallery
  {
    Title = "Imagen",
    Description = "Image",
    UrlNew = new UrlField {Description = string.Empty,Url = "http://google.es"},
    Image = new UrlField {Description = string.Empty,Url = "http://google.es"},
    Visible = true,
    ID = "1",
     OpenWindows = true
            } });
        }
```
