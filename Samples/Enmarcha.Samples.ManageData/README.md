# Administrar datos en Listas #

### Resumen ###
Este ejemplo muestra como  hacer operaciones CRUD utilizando el Repositorio de Enmarcha de datos.

### Funciona con ###
-  SharePoint 2013 on-premises

### Prerequisitos ###
Visual Studio 2013 o superior 

### Solucion ###
Solucin | Autor(s)
---------|----------
Enmarcha.Samples.ManageData | Adrian Diaz Cervera (**ENCAMINA**)

### Version history ###
Version  | Fecha | Comentarios
---------| -----| --------
1.0  | Enero 29 2016 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# ESCENARIO:  #
En este escaniro vamos a mostrar como trabajar con una lista utilizando la clase [SharePointRepository](https://github.com/Encamina/Enmarcha-SharePoint/blob/master/Enmarcha.SharePoint/Entities/Data/SharePointRepository.cs).

### Visual Studio ###

1.- Abrimos la solución Enmarcha.Samples.sln con Visual Studio 2013/ Visual Studio 2015
2.- Restauramos los paquetes Nuget de la Solución
3.- Abrimos el fichero Program.cs e introducimos la url de nuestro sitio de SharePoint que esta asignada en la constante urlSharePointOnpremise:
```C#
 const string urlSharePointOnpremise = "urlsiteSharePoint";
```
4.- Para crear la lista utilizaremos el método extensor [CreateList](https://github.com/Encamina/Enmarcha-SharePoint/blob/master/Enmarcha.SharePoint/Extensors/List.cs)
```C#
  var createList= web.CreateList(listName, "List of Employed of my Company", TypeList.GenericList, false,
                        typeof (Employed));
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
