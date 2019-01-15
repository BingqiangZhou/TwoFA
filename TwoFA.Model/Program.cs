using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwoFA.Model.Context;
using TwoFA.Model.Entity;

namespace TwoFA.Model
{
    class Program
    {
        static void Main(string[] args)
        {
            using(var db = new TwoFaDbContext())
            {
                Custom_User custom_User = new Custom_User
                {
                    Id = 1,
                    Name = "hello",
                    OpenId = "world"
                };
                db.CustomUsers.Add(custom_User);
                CustomWithManufacturer customWithManufacturer = new CustomWithManufacturer
                {
                    CustomeUserId = 1,
                    ManufacturerUserId = 1
                };
                db.CustomWithManufacturers.Add(customWithManufacturer);
                Manufacturer_User manufacturerUser = new Manufacturer_User
                {
                    Id = 1,
                    Name = "hello"
                };
                db.ManufacturerUsers.Add(manufacturerUser);
                try
                {
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}
