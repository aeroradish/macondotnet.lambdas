using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace LamdbaReflection.Web.Models
{
    public class WidgetPrime
    {
        public WidgetPrime()
        {
        }

        public int id { get; set; }

        [DisplayName("Description")]
        public string Description { get; set; }

        [DisplayName("Random String")]
        public string RandomString { get; set; }

        [DisplayName("Random Int")]
        public int RandomInt { get; set; }

    }
}