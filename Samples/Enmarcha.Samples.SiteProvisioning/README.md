# Aprovisionamiento de sitios #

### Resumen ###
Este ejemplo muestra como  crear Sitios y SubSitios dentro de nuestro Sitio de SharePoint.

### Funciona con ###
-  SharePoint 2013 on-premises

### Prerequisitos ###
Visual Studio 2013 o superior 

### Solucion ###
Solución | Autor(s)
---------|----------
Enmarcha.Samples.ManageData | Juan Carlos Martínez (**ENCAMINA**)

### Version history ###
Version  | Fecha | Comentarios
---------| -----| --------
1.0  | Enero 29 2016 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# ESCENARIO:  #
En este escenario vamos crear un sitio y un subsitio de SharePoint.

### Visual Studio ###

1.- Abrimos la solución Enmarcha.Samples.sln con Visual Studio 2013/ Visual Studio 2015

2.- Restauramos los paquetes Nuget de la Solución

3.- Abrimos el fichero Program.cs e introducimos la url de nuestro sitio de SharePoint que esta asignada en la constante urlSharePointOnpremise:
```C#
 const string urlSharePointOnpremise = "urlsiteSharePoint";
```
4.- Crearemos un sitio utilizando el método extensor [CreateSite](https://github.com/Encamina/Enmarcha-SharePoint/blob/master/Enmarcha.SharePoint/Extensors/Site.cs)
```C#
var siteCreationSuccess = site.CreateSite("sample", "Sample Site", "This is a sample site", "STS");
```
5.- Crearemos un sub sitio utilizando el método extensor [CreateSite](https://github.com/Encamina/Enmarcha-SharePoint/blob/master/Enmarcha.SharePoint/Extensors/Site.cs)
```C#
var subSiteCreationSuccess = web.CreateSubSite("samplesubsite", "Sample Subsite", "This is a sample subsite", "STS");
```
