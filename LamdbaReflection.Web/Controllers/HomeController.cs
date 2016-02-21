using LamdbaReflection.Web.Logic;
using LamdbaReflection.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LamdbaReflection.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            
            return View();
        }

        [HttpPost]
        public ActionResult Index(string model)
        {

            IEnumerable<WidgetBeta> genericList = null;

            List<WidgetBeta> listBetas = null;
            List<WidgetGamma> listGammas = null;
            List<WidgetPrime> listPrimes = null;

            List<WidgetBeta> filteredBetas = null;
            List<WidgetGamma> filteredGammas = null;
            List<WidgetPrime> filteredPrimes = null;
            List<WidgetPrime> filteredPrimes2 = null;

            WidgetBeta filteredBeta = null;

            listBetas = PopulateBeta();
            listGammas = PopulateGamma();
            listPrimes = PopulatePrime();

            //base class -- not really usable
            genericList = listBetas.Where(d => d.Description == "Description1");
           
            //won't compile
            //filteredBetas = listBetas.Where(d => d.Description == "Description1");
           
            //Basic compile -- either a list or single
            filteredBetas = listBetas.Where(d => d.Description == "Description1").ToList();

            filteredBeta = listBetas.Where(d => d.Description == "Description1").FirstOrDefault();

            //filter a list that meet a calculated condition
            filteredGammas = listGammas.Where(r => (r.id % 2) == 1).ToList();


            //create anything
            var loosyGoosy = (from g in listGammas
                              select new 
                              {
                                  id = g.id
                                  ,
                                  different = g.RandomString
                              }).ToList();


            //filter into a strongly typed list
            filteredPrimes = (from g in listGammas
                              select new WidgetPrime 
                              { 
                                  id = g.id
                                  ,
                                  RandomString = g.RandomString
                              }).ToList<WidgetPrime>();

            //filter into another list, with conditions
            filteredPrimes2 = (from g in listGammas
                              select new WidgetPrime
                              {
                                  id = g.id
                                  ,
                                  RandomString = g.RandomString
                              }).Where(r => (r.id % 2) == 0).ToList<WidgetPrime>();


            string holdingComment = "I don'\t actually do anything.";
            holdingComment += " prevent green underlines ";

            return RedirectToAction("Index");
        }

        public ActionResult Advanced()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Advanced(string model)
        {

            List<WidgetBeta> listBetas = null;
            List<WidgetGamma> listGammas = null;
            List<WidgetPrime> listPrimes = null;

            List<WidgetBeta> filteredBetas = null;
            List<WidgetGamma> filteredGammas = null;
            List<WidgetPrime> filteredPrimes = null;

            List<string> headers = new List<string>();

            string tableHTML = "";

            listBetas = PopulateBeta();
            listGammas = PopulateGamma();
            listPrimes = PopulatePrime();

            //find all items in source that aren't in filter
            filteredBetas = listBetas.Where(source => !listGammas.Any(filter => filter.id == source.id)).ToList();

            //find all items not in other list's description
            var query = from p in listPrimes
                        where !(from g in listBetas
                               select g.Description).Contains(p.Description)
                        select p;
            
            //Use lambdas as parameters
            
            headers.Add("ID");
            headers.Add("Description");
            headers.Add("RandomInt");
            
            tableHTML = GetMyTable(filteredBetas
                    , headers
                    , x => x.id
                    , x => x.Description
                    , x => x.RandomInt
                 );

            tableHTML = GetMyTable(listPrimes
                    , headers
                    , x => x.id
                    , x => x.Description
                    , x => x.RandomInt
                 );

            return RedirectToAction("Advanced");
        }

        public ActionResult Reflection()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public FileContentResult Reflection(string model)
        {
            List<WidgetBeta> listBetas = null;
            List<WidgetGamma> listGammas = null;
            List<WidgetPrime> listPrimes = null;

            WidgetPrime singleSource = null;
            WidgetBeta singleTarget = null;

            listBetas = PopulateBeta();
            listGammas = PopulateGamma();

            listPrimes = PopulatePrime();// new List<WidgetPrime>();

            for (int i = 0; i < listPrimes.Count; i++)
            {

                singleSource = listPrimes[i];
                singleTarget = new WidgetBeta();

                singleTarget = (WidgetBeta)TransferValues(singleSource, singleTarget);

                listBetas.Add(singleTarget);

            }


            CsvExport<WidgetBeta> csv = new CsvExport<WidgetBeta>(listBetas);
            return File(new System.Text.UTF8Encoding().GetBytes(csv.Export()), "text/csv", "test.csv");

           // return RedirectToAction("Reflection");

        }
        public List<WidgetBeta> PopulateBeta()
        {

            List<WidgetBeta> widgets = new List<WidgetBeta>();
            WidgetBeta thisWidget = null;

            int iLoop = 5;

            Random rand = new Random();

            for (int i = 0; i < iLoop; i++)
            {
                thisWidget = new WidgetBeta();
                thisWidget.id = i + 2;
                thisWidget.RandomInt = rand.Next(10);
                thisWidget.Description = String.Format("Description{0}", i + 2); 

                widgets.Add(thisWidget);
            }

            return widgets;

        }

        public List<WidgetGamma> PopulateGamma()
        {

            List<WidgetGamma> widgets = new List<WidgetGamma>();
            WidgetGamma thisWidget = null;

            Random rand = new Random();

            int iLoop = 5;

            for (int i = 0; i < iLoop; i++)
            {
                thisWidget = new WidgetGamma();
                thisWidget.id = i + 1;
                thisWidget.RandomString = String.Format("Random{0}", rand.Next(10));
                thisWidget.Something = i ^ i;

                widgets.Add(thisWidget);
            }

            return widgets;

        }

        public List<WidgetPrime> PopulatePrime()
        {

            List<WidgetPrime> widgets = new List<WidgetPrime>();
            WidgetPrime thisWidget = null;

            Random rand = new Random();

            int iLoop = 5;

            for (int i = 0; i < iLoop; i++)
            {
                thisWidget = new WidgetPrime();
                thisWidget.id = i;
                thisWidget.RandomString = String.Format("Random{0}", rand.Next(10));
                thisWidget.RandomInt = i ^ i;
                thisWidget.Description =  String.Format("Description{0}", i); 

                widgets.Add(thisWidget);
            }

            return widgets;

        }

        public static object TransferValues(object data, object target)
        {
            Type d = data.GetType();
            Type t = target.GetType();

            foreach (var propertyA in d.GetProperties())
            {
                var propertyB = t.GetProperty(propertyA.Name);
                var valueA = propertyA.GetValue(data, null);
                if (null != propertyB)
                {
                    propertyB.SetValue(target, valueA, null);
                }
            }

            return target;

        }


        public static string GetMyTable<T>(IEnumerable<T> list, List<string> headers, params Func<T, object>[] fxns)
        {

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<TABLE style='border: solid 1px black>\n");

            sb.Append("<TR style='background-color: lightgray;text-weight:bold;'>\n");
            foreach (string col in headers)
            {
                sb.Append("<TH>");
                sb.Append(col);
                sb.Append("</TH>");
            }
            sb.Append("</TR>\n");

            foreach (var item in list)
            {
                sb.Append("<TR>\n");
                foreach (var fxn in fxns)
                {

                    sb.Append("<TD>");
                    sb.Append(fxn(item));
                    sb.Append("</TD>");
                }
                sb.Append("</TR>\n");

            }
            sb.Append("</TABLE>");

            return sb.ToString();
        }
        
    }
}
