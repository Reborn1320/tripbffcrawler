using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CFF.Api.Server.Controllers
{
    public class HomeController : Controller
    {
        private const string filePath = @"~/CFF/Data/CrawlerData";

        public ActionResult Index()
        {
            string rePath = Server.MapPath(filePath);
            var restaurants = ReadFromJsonFile<List<RestaurantModel>>(filePath);
            ViewBag.Title = "Home Page";

            return View();
        }

        public static T ReadFromJsonFile<T>(string filePath) where T : new()
        {
            TextReader reader = null;
            try
            {
                reader = new StreamReader(filePath);
                var fileContents = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<T>(fileContents);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

        public class RestaurantModel
        {
            public RestaurantModel()
            {
                Foods = new List<Food>();
            }

            public string Name { get; set; }
            public string Address { get; set; }
            public string FoodyUrl { get; set; }
            public bool IsClosed { get; set; }
            public int BasicDeliveryFee { get; set; }
            public Promotion Promotion { get; set; }
            public IList<Food> Foods { get; set; }
        }

        public class Promotion
        {
            public Promotion()
            {
                Codes = new List<DiscountCode>();
            }

            public IList<DiscountCode> Codes { get; set; }
            public DiscountFeeDelivery DiscountFeeDelivery { get; set; }
            public DiscountAirpay DiscountAirpay { get; set; }
        }

        public class DiscountBase
        {
            public DateTime ExpiredDate { get; set; }
            public int MinOrderTotal { get; set; }
            public int MaxOrderTotal { get; set; }
        }

        public class DiscountCode : DiscountBase
        {
            public int PercentDiscount { get; set; }
            public string CodeDiscount { get; set; }
        }

        public class DiscountFeeDelivery : DiscountBase
        {
        }

        public class DiscountAirpay : DiscountBase
        {
        }

        public class Food
        {
            public string Name { get; set; }
            public int Price { get; set; }
            public int NumberOfOrders { get; set; }
            public string ImgSrc { get; set; }
        }
    }
}
