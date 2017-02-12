using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Cors;

namespace OWINRestService //In development
{
    public class Website
    {
        public int Id { get; set; }
        public string Municipality { get; set; }
        public string Date { get; set; }
    }

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class WebsitesController : ApiController
    {
        Website[] websites = new Website[]
        {
            new Website { Id = 1, Municipality = "Vilnius", Date =  DateTime.Now.ToString("dd/MM/yyyy")},
            new Website { Id = 2, Municipality = "Vilnius", Date = DateTime.Now.ToString("dd/MM/yyyy")}
        };

        // GET api/Websites 
        public IEnumerable Get()
        {
            return websites;
        }

        // GET api/Websites/5 
        public Website Get(int id)
        {
            try
            {
                return websites[id];
            }
            catch (Exception e)
            {
                return new Website();
            }
        }

   

        // POST api/values 
        public void Post([FromBody]string value)
        {
            Console.WriteLine("Post method called with value = " + value);
        }

        // PUT api/values/5 
        public void Put(int id, [FromBody]string value)
        {
            Console.WriteLine("Put method called with value = " + value);
        }

        // DELETE api/values/5 
        public void Delete(int id)
        {
            Console.WriteLine("Delete method called with id = " + id);
        }

    }
}