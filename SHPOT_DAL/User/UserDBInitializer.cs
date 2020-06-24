using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace SHPOT_DAL
{
    public class UserDBInitializer : CreateDatabaseIfNotExists<DBContext>
    {
        protected override void Seed(DBContext context)
        {
            User newUser = new User { UserName = "admin", Email = "admin@gmail.com", Password = "Test123@", CreatedDate = DateTime.Now, FirstName="Admin", LastName="User", IPAddress="192.168.1.1", HeaderToken= "eaQRFWpErA8:APA91bGuQeAOEAmA2SbjGg9_Q0XEHrPh8TcAfz6EOsvWILyDE9lAQiFaxVDYjDyRXgDUyi5OSEwyXOljlUJ94f_U3A46SaRWTQtsleJOjlRACVwLerFM61YrWXpfhx7tsVEo9295EGbD", IsActive=true, UserType="Business" };
            context.Users.Add(newUser);
            context.SaveChanges();

            base.Seed(context);
        }
    }
}
