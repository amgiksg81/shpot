using System.Data.Entity;

namespace SHPOT_DAL
{
    public class DBContext : DbContext
    {
        public DBContext() : base("DBConnection")
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new UserDBInitializer());
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserLogin> UserLogins { get; set; }
        public DbSet<UserPremium> UserPremiums { get; set; }
        public DbSet<Business> Businesses { get; set; }
        public DbSet<BusinessImage> BusinessImages { get; set; }
        public DbSet<BusinessRating> BusinessRatings { get; set; }
        public DbSet<FavouritePlace> FavouritePlaces { get; set; }
        public DbSet<SupportQuery> SupportQueries { get; set; }
    }
}
