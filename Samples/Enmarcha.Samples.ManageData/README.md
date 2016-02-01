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
Enmarcha.Samples.ManageData | [Adrian Diaz Cervera](https://github.com/AdrianDiaz81) (**ENCAMINA**)

### Version history ###
Version  | Fecha | Comentarios
---------| -----| --------
1.0  | Enero 29 2016 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

----------

## ESCENARIO ##
En este escenario vamos a mostrar como trabajar con una lista utilizando la clase [SharePointRepository](https://github.com/Encamina/Enmarcha-SharePoint/blob/master/Enmarcha.SharePoint/Entities/Data/SharePointRepository.cs)

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
Los Atributos que se pueden añadir a cada propiedad estan dentro de la Clase [EnmarchaAttribute.cs]https://github.com/Encamina/Enmarcha-SharePoint/blob/master/Enmarcha.SharePoint/Attribute/EnmarchaAttribute.cs)
AddPrefeix-> Le añada un prefijo cuando crea el campo de forma que se evita que coincida con algun campo ya declarado
Create -> Indica si esta propiedad hay que crearla o no.
Type -> Tipo de SharePoint con el que representa esta propiedad

5.-A continuación, inicialicaremos la clase SharePointRepository, los parametros que son necesarios son:

. SPweb

. Log (Enmarcha por defecto trae un Log que graba en los [logs de SharePoint](https://github.com/Encamina/Enmarcha-SharePoint/blob/master/Enmarcha.SharePoint/Entities/Logs/LogManager.cs) pero se puede utilizar cualquier Log siemple que se implemente la interfaz [ILog](https://github.com/Encamina/Enmarcha-SharePoint/blob/master/Enmarcha.SharePoint.Abstract/Interfaces/Artefacts/ILog.cs)

```C#
var  logger = new LogManager().GetLogger(new System.Diagnostics.StackTrace().GetFrame(0)); ;
var repository= new SharePointRepository<Employed>(web,logger,listName,10);
```

6.- Ahora para insertar un elemento sobre la lista de SharePoint Employed, tendremos en primer lugar crear una elemento basado en la clase Employed y a continuación pasarle ese elemento al metodo "Insert" de nuestro repositorio de SharePoint. de 
```C#
  var employed = new Employed
                {
                    Country = "Spain",
                    DateBorn = new DateTime(1981, 5, 10),
                    Job = "Sofware Architect",
                    LastName = "Diaz Cervera",
                    Name = "Adrian"
                };
var  resultInsert= repository.Insert(employed);
```
7.- Para realizar una modificación sobre un elemento hay que pasarle los datos que se quierean modificar y el identificador del elemento que vamos actualizar
```C#
  var firstEmployed= new Employed { Job = "Sofware Architect Lead"};
  var updateOperation= repository.Save(Convert.ToInt32(resultInsert), firstEmployed);
```

8.- Eliminar un elemento
```C#
var resultBool = repository.Delete(resultInsert);
```

9.- Como hacer Hacer consultas sobre las listas, se pueden hacer de dos formas pasando la Caml Query de forma directa:
```C#
 var queryCaml = @"<Where>
                                      <Eq>
                                         <FieldRef Name='Name' />
                                         <Value Type='Text'>Adrian</Value>
                                      </Eq>
                                   </Where>";
 var queryCollection = repository.Query(queryCaml, 1);
```
o bien podemos utilizar un [generador de consultas](https://github.com/Encamina/Enmarcha-SharePoint/blob/master/Enmarcha.SharePoint/Entities/Data/Query.cs) que esta dentro de Enmarcha
```C#
var query = new Query().Where().Field("Name",string.Empty).Operator(TypeOperators.Eq).Value("Text","Adrian");
  queryCollection = repository.Query(query, 1);
```
