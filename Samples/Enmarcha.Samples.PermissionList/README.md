# Administrar permisos en Listas #

### Resumen ###
Este ejemplo muestra como dar permisos a los grupos en las Listas de SharePoint.

### Funciona con ###
-  SharePoint 2013 on-premises

### Prerequisitos ###
Visual Studio 2013 o superior 

### Solucion ###
Solución | Autor(s)
---------|----------
Enmarcha.Samples.PermissionList | [Adrian Diaz Cervera](https://github.com/AdrianDiaz81) (**ENCAMINA**)

### Version history ###
Version  | Fecha | Comentarios
---------| -----| --------
1.0  | Enero 29 2016 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

----------

## ESCENARIO ##
En este escenario vamos a mostrar como Enmarcha [extiende la API de SharePoint Server](https://github.com/Encamina/Enmarcha-SharePoint/blob/master/Enmarcha.SharePoint/Extensors/List.cs) para ahorrar funcionalidad.

### Visual Studio ###

1.- Abrimos la solución Enmarcha.Samples.sln con Visual Studio 2013/ Visual Studio 2015

2.- Restauramos los paquetes Nuget de la Solución

3.- Abrimos el fichero Program.cs e introducimos la url de nuestro sitio de SharePoint que esta asignada en la constante urlSharePointOnpremise:
```C#
 const string urlSharePointOnpremise = "urlsiteSharePoint";
```
4.- Para crear la lista utilizaremos el método extensor [CreateList](https://github.com/Encamina/Enmarcha-SharePoint/blob/master/Enmarcha.SharePoint/Extensors/List.cs)
```C#
  if (list == null) web.CreateList(listSample, string.Empty, TypeList.GenericList,true);
```

5.- Creamos un grupo con el método Extensor [CreateGroup](https://github.com/Encamina/Enmarcha-SharePoint/blob/master/Enmarcha.SharePoint/Extensors/RolesManagment.cs)
```C#
web.CreateGroup("Test",SPRoleType.Reader);
```

6.- Consultaremos la lista de SharePoint
```C#
 var list = web.Lists.TryGetList(listSample);
```
7.-Y utilizando el método extensor de la lista AddPermissionLibrary le añadiremos al grupo recien creado el rol de "Contributor".
```C#
var result=    list.AddPermisionLibrary("Test", RoleType.Contributor);
```
