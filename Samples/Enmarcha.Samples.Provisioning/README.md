# Administrar datos en Listas #

### Resumen ###
Este ejemplo muestra como aprovisionar Columnas de Sitio, Tipos de Contenido, Listas en un sitio de SharePoint.

### Funciona con ###
-  SharePoint 2013 on-premises

### Prerequisitos ###
Visual Studio 2013 o superior 

### Solución ###
Solucin | Autor(s)
---------|----------
Enmarcha.Samples.Provisioning | Adrian Diaz Cervera (**ENCAMINA**)

### Version history ###
Version  | Fecha | Comentarios
---------| -----| --------
1.0  | Enero 29 2016 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# ESCENARIO:  #
En este escenario vamos a mostrar como trabajar con una lista utilizando la clase [SharePointRepository](https://github.com/Encamina/Enmarcha-SharePoint/blob/master/Enmarcha.SharePoint/Entities/Data/SharePointRepository.cs).

### Visual Studio ###

1.- Abrimos la solución Enmarcha.Samples.sln con Visual Studio 2013/ Visual Studio 2015

2.- Restauramos los paquetes Nuget de la Solución

3.- Abrimos el fichero Program.cs e introducimos la url de nuestro sitio de SharePoint que esta asignada en la constante urlSharePointOnpremise:
```C#
 const string urlSharePointOnpremise = "urlsiteSharePoint";
```
4.-En primer lugar se creará una Columna de Sitio en base la clase [Employed.cs](https://github.com/Encamina/Enmarcha-SharePoint/blob/master/Samples/Enmarcha.Samples.Provisioning/Model/Employed.cs)
```C#
  var columnSite = web.CreateColumnSite("ENMARCHA ContentType", typeof(Employed));
```
Esto crea una lista y le añade los campos, cada una de las propiedades que hay en la clase [Employed.cs](https://github.com/Encamina/Enmarcha-SharePoint/blob/master/Samples/Enmarcha.Samples.ManageData/Model/Employed.cs). 
Para saber que tipo de Columnas de SharePoint son necesarios a cada propiedad le asignamos unos Atributos donde se condigura estos valores:
```C#
[Enmarcha(AddPrefeix = false, Create = false, Type = TypeField.Text)]
public string ID { get; set; }
[Enmarcha(AddPrefeix = false, Create = true, Type = TypeField.Text, DisplayName = "Fist Name")]
public string Name { get; set; }
[Enmarcha(AddPrefeix = false, Create = true, Type = TypeField.Text, DisplayName = "Last Name")]
public string LastName { get; set; }
[Enmarcha(AddPrefeix = false, Create = true, Type = TypeField.DateTime, DisplayName = "Date of Born")]
public DateTime DateBorn { get; set; }
[Enmarcha(AddPrefeix = false, Create = true, Type = TypeField.Choice, DisplayName = "Job",Choice= new []{"Developer","Designer"})]
public string Job { get; set; }
[Enmarcha(AddPrefeix = false, Create = true, Type = TypeField.Text, DisplayName = "Country")]
public string Country { get; set; }
[Enmarcha(AddPrefeix = false, Create = true, Type = TypeField.User, DisplayName = "Boss Primary")]
public IList<UserSP> Boss { get; set; }
```
Los Atributos que se pueden añadir a cada propiedad estan dentro de la Clase [EnmarchaAttribute.cs]https://github.com/Encamina/Enmarcha-SharePoint/blob/master/Enmarcha.SharePoint/Attribute/EnmarchaAttribute.cs)

AddPrefeix-> Le añada un prefijo cuando crea el campo de forma que se evita que coincida con algun campo ya declarado

Create -> Indica si esta propiedad hay que crearla o no.

Type -> Tipo de SharePoint con el que representa esta propiedad

5.-A continuación, crearemos un tipo de contenido, añadiendo las columnas de sitio creadas en el paso anterior

```C#
web.CreateContentType("EmployedBussines", "ENMARCHA","Elemento", columnSite);
```

6.- Ahora creamos una lista y le agregamos el tipo de contenido creado
```C#
   web.CreateList("Empleados", "Lista de Empleados", TypeList.GenericList, true);
   var list = web.Lists.TryGetList("Empleados");                    
   list.AddContentTypeLibrary("EmployedBussines");
```
