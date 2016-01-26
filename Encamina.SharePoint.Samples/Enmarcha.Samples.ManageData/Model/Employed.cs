using System;
using System.Collections.Generic;
using Enmarcha.SharePoint.Abstract.Enum;
using Enmarcha.SharePoint.Attribute;
using Enmarcha.SharePoint.Helpers.Data;

namespace Enmarcha.Samples.ManageData.Model
{
    public class Employed
    {
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

    }
}
