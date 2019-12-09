using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace CFF.Crawler
{
    class Program
    {
        private const string filePathTravel = @"/Users/reborn/SourceCode/tripbffcrawler/Data/TravelData";
        private const string filePathRestaurant = @"/Users/reborn/SourceCode/tripbffcrawler/Data/RestaurantData";
        private const string filePathEdu = @"/Users/reborn/SourceCode/tripbffcrawler/Data/EducationData";
        private const string filePathEntertain = @"/Users/reborn/SourceCode/tripbffcrawler/Data/EntertainData";

        static void Main(string[] args)
        {
            //CrawlerTravelData();
            CrawlerRestaurantData();
            //CrawlerEntertainData();
            //CrawlerEducationData();
        }

        //private static object GetDealRestaurantsFromFile()
        //{
        //    var restaurants = ReadFromJsonFile<List<RestaurantModel>>(filePath);
        //    return restaurants;
        //}

        private static bool CrawlerTravelData()
        {
            //Load html content 
            using (IWebDriver driver = new ChromeDriver("/home/reborn/Downloads/chromedriver_linux64"))
            {
                var rootUrl = "https://www.foody.vn/";
                driver.Navigate().GoToUrl(rootUrl);
                WaitForReady(driver, TimeSpan.FromSeconds(30));

                var citiesDropdownElement = driver.FindElement(By.ClassName("rn-nav-name"));
                citiesDropdownElement.Click();

                IWebElement flpCountriesElement = null;

                while (true)
                {
                    try
                    {
                        flpCountriesElement = driver.FindElement(By.XPath("//*[@id=\"popupLocation\"]/ul/li/ul"));

                        if (flpCountriesElement != null)
                            break;
                    }  
                    catch(Exception)
                    {
                    }
                }


                var citiesParentElements = flpCountriesElement.FindElements(By.TagName("li"));
                var cityUrls = new List<string>();
                
                foreach (var citiesParentElement in citiesParentElements)
                {
                    var cityParentElements = citiesParentElement.FindElements(By.TagName("li"));

                    foreach(var cityParentElement in cityParentElements)
                    {
                        var linkElement = cityParentElement.FindElement(By.TagName("a"));
                        var cityUrl = linkElement.GetAttribute("href") + "/travel";
                        cityUrls.Add(cityUrl);
                    }
                }

                var locations = new List<LocationModel>();

                foreach(var cityUrl in cityUrls)
                {
                    driver.Navigate().GoToUrl(cityUrl);
                    var locationContainerElement = driver.FindElement(By.ClassName("content-container"));

                    // handle load more
                    while (true)
                    {
                        try
                        {
                            var loadMoreContainerElement = locationContainerElement.FindElement(By.ClassName("pn-loadmore"));

                            if (loadMoreContainerElement != null)
                            {
                                var indexLoadMore = 0;

                                while(true)
                                {
                                    try 
                                    {
                                        var loadMoreElement = loadMoreContainerElement.FindElement(By.ClassName("fd-btn-more"));

                                        if (loadMoreElement != null)
                                        {
                                            loadMoreElement.Click();
                                            break;
                                        }
                                    }
                                    catch(Exception)
                                    {
                                    }

                                    indexLoadMore++;

                                    if (indexLoadMore == 100)
                                        break;
                                }
                            }
                            else
                                break;
                        }
                        catch(Exception) 
                        {
                            break;
                        }
                    }

                    WaitForReady(driver, TimeSpan.FromSeconds(30));

                    locationContainerElement = driver.FindElement(By.ClassName("content-container"));
                    var locationElements = locationContainerElement.FindElements(By.ClassName("content-item"));

                    foreach(var locationElement in locationElements)
                    {
                        var locationItemContent = locationElement.FindElement(By.ClassName("items-content"));
                        var titleElement = locationItemContent.FindElement(By.ClassName("title"));
                        var addressElement = locationItemContent.FindElement(By.ClassName("desc"));

                        var title = titleElement.FindElement(By.TagName("a")).Text;
                        var address = addressElement.Text;
                        var location = new LocationModel
                        {
                            Title = title,
                            Address = address
                        };
                        locations.Add(location);
                    }

                    Thread.Sleep(500);
                }

                var result = new LocationResult
                {
                    Locations = locations
                };

                WriteToJsonFile(filePathTravel, result);

                driver.Quit();
            }

            return true;
        }

        private static bool CrawlerRestaurantData()
        {
            //Load html content 
            using (IWebDriver driver = new ChromeDriver("/usr/local/bin"))
            {
                var rootUrl = "https://www.foody.vn/";
                driver.Navigate().GoToUrl(rootUrl);
                WaitForReady(driver, TimeSpan.FromSeconds(30));

                var citiesDropdownElement = driver.FindElement(By.ClassName("rn-nav-name"));
                citiesDropdownElement.Click();

                IWebElement flpCountriesElement = null;

                while (true)
                {
                    try
                    {
                        flpCountriesElement = driver.FindElement(By.XPath("//*[@id=\"popupLocation\"]/ul/li/ul"));

                        if (flpCountriesElement != null)
                            break;
                    }
                    catch (Exception)
                    {
                    }
                }

                var citiesParentElements = flpCountriesElement.FindElements(By.TagName("li"));
                var cityUrls = new List<string>();

                foreach (var citiesParentElement in citiesParentElements)
                {
                    var cityParentElements = citiesParentElement.FindElements(By.TagName("li"));

                    foreach (var cityParentElement in cityParentElements)
                    {
                        var linkElement = cityParentElement.FindElement(By.TagName("a"));
                        var cityUrl = linkElement.GetAttribute("href");
                        cityUrls.Add(cityUrl);
                    }
                }

                var locations = new List<LocationModel>();

                foreach (var cityUrl in cityUrls)
                {
                    driver.Navigate().GoToUrl(cityUrl);
                    var locationContainerElement = driver.FindElement(By.ClassName("content-container"));

                    // handle load more
                    while (true)
                    {
                        try
                        {
                            var loadMoreContainerElement = locationContainerElement.FindElement(By.ClassName("pn-loadmore"));

                            if (loadMoreContainerElement != null)
                            {
                                var indexLoadMore = 0;

                                while (true)
                                {
                                    try
                                    {
                                        var loadMoreElement = loadMoreContainerElement.FindElement(By.ClassName("fd-btn-more"));

                                        if (loadMoreElement != null)
                                        {
                                            loadMoreElement.Click();
                                            break;
                                        }
                                    }
                                    catch (Exception)
                                    {
                                    }

                                    indexLoadMore++;

                                    if (indexLoadMore == 100)
                                        break;
                                }
                            }
                            else
                                break;
                        }
                        catch (Exception)
                        {
                            break;
                        }
                    }

                    WaitForReady(driver, TimeSpan.FromSeconds(30));

                    locationContainerElement = driver.FindElement(By.ClassName("content-container"));
                    var locationElements = locationContainerElement.FindElements(By.ClassName("content-item"));

                    foreach (var locationElement in locationElements)
                    {
                        var locationItemContent = locationElement.FindElement(By.ClassName("items-content"));
                        var titleElement = locationItemContent.FindElement(By.ClassName("title"));
                        var urlElement = titleElement.FindElement(By.TagName("a"));
                        var addressElement = locationItemContent.FindElement(By.ClassName("desc"));

                        var title = urlElement.Text;
                        var locationUrl = urlElement.GetAttribute("href");
                        var address = addressElement.Text;
                        var location = new LocationModel
                        {
                            Title = title,
                            Address = address,
                            Url = locationUrl
                        };
                        locations.Add(location);
                    }

                    Thread.Sleep(500);
                }
                
                // get long, lat of locations
                foreach (var location in locations)
                {
                    try
                    {
                        driver.Navigate().GoToUrl(location.Url);
                        var linkMapElements = driver.FindElements(By.ClassName("linkmap"));
                        var linkMapElement = linkMapElements[2];

                        var imgElement = linkMapElement.FindElement(By.TagName("img"));
                        var src = imgElement.GetAttribute("src");
                        var srcItems = src.Split('_');

                        var longValue = srcItems[2].Split('.')[0].Replace("-", ".");
                        var latValue = srcItems[1].Replace("-", ".");

                        location.Long = longValue;
                        location.Lat = latValue;
                    }
                    catch(Exception)
                    {
                    }

                    Thread.Sleep(500);
                }

                var result = new LocationResult
                {
                    Locations = locations
                };

                WriteToJsonFile(filePathRestaurant, result);
                driver.Quit();
            }

            return true;
        }

        private static bool CrawlerEntertainData()
        {
            //Load html content 
            using (IWebDriver driver = new ChromeDriver("/usr/local/bin"))
            {
                var rootUrl = "https://www.foody.vn/";
                driver.Navigate().GoToUrl(rootUrl);
                WaitForReady(driver, TimeSpan.FromSeconds(30));

                var citiesDropdownElement = driver.FindElement(By.ClassName("rn-nav-name"));
                citiesDropdownElement.Click();

                IWebElement flpCountriesElement = null;

                while (true)
                {
                    try
                    {
                        flpCountriesElement = driver.FindElement(By.XPath("//*[@id=\"popupLocation\"]/ul/li/ul"));

                        if (flpCountriesElement != null)
                            break;
                    }
                    catch (Exception)
                    {
                    }
                }

                var citiesParentElements = flpCountriesElement.FindElements(By.TagName("li"));
                var cityUrls = new List<string>();

                foreach (var citiesParentElement in citiesParentElements)
                {
                    var cityParentElements = citiesParentElement.FindElements(By.TagName("li"));

                    foreach (var cityParentElement in cityParentElements)
                    {
                        var linkElement = cityParentElement.FindElement(By.TagName("a"));
                        var cityUrl = linkElement.GetAttribute("href") + "/entertain";
                        cityUrls.Add(cityUrl);
                    }
                }

                var locations = new List<LocationModel>();

                foreach (var cityUrl in cityUrls)
                {
                    driver.Navigate().GoToUrl(cityUrl);
                    var locationContainerElement = driver.FindElement(By.ClassName("content-container"));

                    // handle load more
                    while (true)
                    {
                        try
                        {
                            var loadMoreContainerElement = locationContainerElement.FindElement(By.ClassName("pn-loadmore"));

                            if (loadMoreContainerElement != null)
                            {
                                var indexLoadMore = 0;

                                while (true)
                                {
                                    try
                                    {
                                        var loadMoreElement = loadMoreContainerElement.FindElement(By.ClassName("fd-btn-more"));

                                        if (loadMoreElement != null)
                                        {
                                            loadMoreElement.Click();
                                            break;
                                        }
                                    }
                                    catch (Exception)
                                    {
                                    }

                                    indexLoadMore++;

                                    if (indexLoadMore == 100)
                                        break;
                                }
                            }
                            else
                                break;
                        }
                        catch (Exception)
                        {
                            break;
                        }
                    }

                    WaitForReady(driver, TimeSpan.FromSeconds(30));

                    locationContainerElement = driver.FindElement(By.ClassName("content-container"));
                    var locationElements = locationContainerElement.FindElements(By.ClassName("content-item"));

                    foreach (var locationElement in locationElements)
                    {
                        var locationItemContent = locationElement.FindElement(By.ClassName("items-content"));
                        var titleElement = locationItemContent.FindElement(By.ClassName("title"));
                        var urlElement = titleElement.FindElement(By.TagName("a"));
                        var addressElement = locationItemContent.FindElement(By.ClassName("desc"));

                        var title = urlElement.Text;
                        var locationUrl = urlElement.GetAttribute("href");
                        var address = addressElement.Text;
                        var location = new LocationModel
                        {
                            Title = title,
                            Address = address,
                            Url = locationUrl
                        };
                        locations.Add(location);
                    }

                    Thread.Sleep(500);
                }

                // get long, lat of locations
                foreach (var location in locations)
                {
                    try
                    {
                        driver.Navigate().GoToUrl(location.Url);
                        var linkMapElements = driver.FindElements(By.ClassName("linkmap"));
                        var linkMapElement = linkMapElements[2];

                        var imgElement = linkMapElement.FindElement(By.TagName("img"));
                        var src = imgElement.GetAttribute("src");
                        var srcItems = src.Split('_');

                        var longValue = srcItems[2].Split('.')[0].Replace("-", ".");
                        var latValue = srcItems[1].Replace("-", ".");

                        location.Long = longValue;
                        location.Lat = latValue;
                    }
                    catch (Exception)
                    {
                    }

                    Thread.Sleep(500);
                }

                var result = new LocationResult
                {
                    Locations = locations
                };

                WriteToJsonFile(filePathEntertain, result);
                driver.Quit();
            }

            return true;
        }

        private static bool CrawlerEducationData()
        {
            //Load html content 
            using (IWebDriver driver = new ChromeDriver("/usr/local/bin"))
            {
                var rootUrl = "https://www.foody.vn/";
                driver.Navigate().GoToUrl(rootUrl);
                WaitForReady(driver, TimeSpan.FromSeconds(30));

                var citiesDropdownElement = driver.FindElement(By.ClassName("rn-nav-name"));
                citiesDropdownElement.Click();

                IWebElement flpCountriesElement = null;

                while (true)
                {
                    try
                    {
                        flpCountriesElement = driver.FindElement(By.XPath("//*[@id=\"popupLocation\"]/ul/li/ul"));

                        if (flpCountriesElement != null)
                            break;
                    }
                    catch (Exception)
                    {
                    }
                }

                var citiesParentElements = flpCountriesElement.FindElements(By.TagName("li"));
                var cityUrls = new List<string>();

                foreach (var citiesParentElement in citiesParentElements)
                {
                    var cityParentElements = citiesParentElement.FindElements(By.TagName("li"));

                    foreach (var cityParentElement in cityParentElements)
                    {
                        var linkElement = cityParentElement.FindElement(By.TagName("a"));
                        var cityUrl = linkElement.GetAttribute("href") + "/edu";
                        cityUrls.Add(cityUrl);
                    }
                }

                var locations = new List<LocationModel>();

                foreach (var cityUrl in cityUrls)
                {
                    driver.Navigate().GoToUrl(cityUrl);
                    var locationContainerElement = driver.FindElement(By.ClassName("content-container"));

                    // handle load more
                    while (true)
                    {
                        try
                        {
                            var loadMoreContainerElement = locationContainerElement.FindElement(By.ClassName("pn-loadmore"));

                            if (loadMoreContainerElement != null)
                            {
                                var indexLoadMore = 0;

                                while (true)
                                {
                                    try
                                    {
                                        var loadMoreElement = loadMoreContainerElement.FindElement(By.ClassName("fd-btn-more"));

                                        if (loadMoreElement != null)
                                        {
                                            loadMoreElement.Click();
                                            break;
                                        }
                                    }
                                    catch (Exception)
                                    {
                                    }

                                    indexLoadMore++;

                                    if (indexLoadMore == 100)
                                        break;
                                }
                            }
                            else
                                break;
                        }
                        catch (Exception)
                        {
                            break;
                        }
                    }

                    WaitForReady(driver, TimeSpan.FromSeconds(30));

                    locationContainerElement = driver.FindElement(By.ClassName("content-container"));
                    var locationElements = locationContainerElement.FindElements(By.ClassName("content-item"));

                    foreach (var locationElement in locationElements)
                    {
                        var locationItemContent = locationElement.FindElement(By.ClassName("items-content"));
                        var titleElement = locationItemContent.FindElement(By.ClassName("title"));
                        var urlElement = titleElement.FindElement(By.TagName("a"));
                        var addressElement = locationItemContent.FindElement(By.ClassName("desc"));

                        var title = urlElement.Text;
                        var locationUrl = urlElement.GetAttribute("href");
                        var address = addressElement.Text;
                        var location = new LocationModel
                        {
                            Title = title,
                            Address = address,
                            Url = locationUrl
                        };
                        locations.Add(location);
                    }

                    Thread.Sleep(500);
                }

                // get long, lat of locations
                foreach (var location in locations)
                {
                    try
                    {
                        driver.Navigate().GoToUrl(location.Url);
                        var linkMapElements = driver.FindElements(By.ClassName("linkmap"));
                        var linkMapElement = linkMapElements[2];

                        var imgElement = linkMapElement.FindElement(By.TagName("img"));
                        var src = imgElement.GetAttribute("src");
                        var srcItems = src.Split('_');

                        var longValue = srcItems[2].Split('.')[0].Replace("-", ".");
                        var latValue = srcItems[1].Replace("-", ".");

                        location.Long = longValue;
                        location.Lat = latValue;
                    }
                    catch (Exception)
                    {
                    }

                    Thread.Sleep(500);
                }

                var result = new LocationResult
                {
                    Locations = locations
                };

                WriteToJsonFile(filePathEdu, result);
                driver.Quit();
            }

            return true;
        }


        private static void WaitForReady(IWebDriver webDriver, TimeSpan timeout)
        {
            new WebDriverWait(webDriver, timeout).Until(
                d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));

            //WebDriverWait wait = new WebDriverWait(webDriver, timeout);
            //wait.Until(driver =>
            //{
            //    bool isAjaxFinished = (bool)((IJavaScriptExecutor)driver).
            //        ExecuteScript("return jQuery.active == 0");
            //    bool isLoaderHidden = (bool)((IJavaScriptExecutor)driver).
            //        ExecuteScript("return $('.loading-center').length <= 0");
            //    return isAjaxFinished & isLoaderHidden;
            //});
        }

        public static void WaitForLoad(IWebDriver driver, TimeSpan timeout)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            WebDriverWait wait = new WebDriverWait(driver, timeout);
            wait.Until(wd => js.ExecuteScript("return document.readyState").ToString() == "complete");
        }

        public static void WriteToJsonFile(string filePath, object objectToWrite, bool append = false)
        {
            TextWriter writer = null;
            try
            {
                //if (!Directory.Exists(filePath.Substring(0, filePath.LastIndexOf('\\'))))
                //{
                //    Directory.CreateDirectory(filePath.Substring(0, filePath.LastIndexOf('\\')));
                //}
                var contentsToWriteToFile = JsonConvert.SerializeObject(objectToWrite);
                writer = new StreamWriter(filePath, append);
                writer.Write(contentsToWriteToFile);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
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
    }
}
