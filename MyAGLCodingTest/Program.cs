using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
    
namespace MyAGLCodingTest
{
    public class JsonCats
    {
        public static void Main(string[] args)
        {
            DisplayCats();
            Console.ReadLine();
        }
        public static void DisplayCats()
        {
            var azureJsonResult = GetJSONData().Result;

            var myPets = JArray.Parse(azureJsonResult);

            var myCats = myPets.GroupBy(
                person => person["gender"].Value<string>(), // gender of pets
                person => person["pets"].Where(pet => pet["type"].Value<string>() == "Cat").Select(pet => pet["name"].Value<string>()), // get the name of cats
                (gender, cats) => new { Gender = gender, Cats = cats.SelectMany(cat => cat).OrderBy(name => name) }); // get cat list and sort names in segment

            foreach (var cat in myCats)
            {
                Console.WriteLine(cat.Gender);
                Console.WriteLine("................" + System.Environment.NewLine);

                foreach (var catname in cat.Cats)
                {
                    Console.WriteLine(catname);

                }
                Console.WriteLine(System.Environment.NewLine);
            }

        }
        private static async Task<string> GetJSONData()
        {
            var data = string.Empty;

            using (var myClient = new HttpClient())
            {
                myClient.BaseAddress = new Uri("http://agl-developer-test.azurewebsites.net/");
                myClient.DefaultRequestHeaders.Accept.Clear();
                myClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await myClient.GetAsync("people.json");
                if (response.IsSuccessStatusCode)
                {
                    data = await response.Content.ReadAsStringAsync();
                }
            }
            return data;
        }
    }
}
