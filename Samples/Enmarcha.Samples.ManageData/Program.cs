using System;
using System.Linq;
using Enmarcha.Samples.ManageData.Model;
using Enmarcha.SharePoint.Abstract.Enum;
using Enmarcha.SharePoint.Class.Data;
using Enmarcha.SharePoint.Class.Logs;
using Enmarcha.SharePoint.Extensors;
using Microsoft.SharePoint;

namespace Enmarcha.Samples.ManageData
{
    internal class Program
    {
        private static void Main()
        {
            const string urlSharePointOnpremise = "urlsiteSharePoint";
            const string listName = "Employed";
            using (var site = new SPSite(urlSharePointOnpremise))
            {
                var web = site.OpenWeb();
                var list = web.Lists.TryGetList(listName);
                if (list == null)
                {
                    var createList= web.CreateList(listName, "List of Employed of my Company", TypeList.GenericList, false,
                        typeof (Employed));
                    Console.WriteLine(string.Concat("List Employed Created", createList));
               }

                var employed = new Employed
                {
                    Country = "Spain",
                    DateBorn = new DateTime(1981, 5, 10),
                    Job = "Sofware Architect",
                    LastName = "Diaz Cervera",
                    Name = "Adrian"
                };

                var employed2 = new Employed
                {
                    Country = "Spain",
                    DateBorn = new DateTime(1979, 5, 10),
                    Job = "Head of Innovation",
                    LastName = "Diaz Martin",
                    Name = "Alberto"
                };

                var  logger = new LogManager().GetLogger(new System.Diagnostics.StackTrace().GetFrame(0)); ;
                var repository= new SharePointRepository<Employed>(web,logger,listName,10);

                var  resultInsert= repository.Insert(employed);
                Console.WriteLine(string.Concat("Insertado el elemento: ", resultInsert));
                resultInsert = repository.Insert(employed2);
                Console.WriteLine(string.Concat("Insertado el elemento: ", resultInsert));

                var employed3= repository.Get(resultInsert);
                Console.WriteLine(string.Concat("Return employed: ", employed3.Name));
                var employedCollection= repository.GetAll();
                Console.WriteLine(string.Concat("Count Employed: ", employedCollection.Count));                
                var resultBool = repository.Delete(resultInsert);
                Console.WriteLine(string.Concat("Elemento Eliminado ", resultBool));
                employedCollection = repository.GetAll();
                Console.WriteLine(string.Concat("Count Employed: ", employedCollection.Count));
                var queryCaml = @"<Where>
                                      <Eq>
                                         <FieldRef Name='Name' />
                                         <Value Type='Text'>Adrian</Value>
                                      </Eq>
                                   </Where>";
                var queryCollection = repository.Query(queryCaml, 1);
                Console.WriteLine(string.Concat("Count Employed: ", queryCollection.Count));               
                var query = new Query().Where().Field("Name",string.Empty).Operator(TypeOperators.Eq).Value("Text","Adrian");
                queryCollection = repository.Query(query, 1);
                Console.WriteLine(string.Concat("Count Employed: ", queryCollection.Count));
                var firstEmployed = queryCollection.FirstOrDefault();
                firstEmployed.Name = "Alberto Javier";
                var updateOperation= repository.Save(Convert.ToInt32(firstEmployed.ID), firstEmployed);
                Console.WriteLine(string.Concat("Update Employed: ", updateOperation));                
                Console.ReadLine();
            }
        }
    }
}
