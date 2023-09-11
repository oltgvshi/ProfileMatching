using ASP.NETCoreIdentityCustom.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProfileMatching.Models;

namespace ProfileMatching.Controllers
{
    public class DataContext : IdentityDbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
                : base(options)
        { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<EmploymentType> EmploymentTypes { get; set; }
        public DbSet<JobPost> JobPosts { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<FreelancerDetails> FreelancerDetails { get; set; }
        public DbSet<FreelancerExperience> FreelancerExperiences { get; set; }
        public DbSet<FreelancerEducation> FreelancerEducations { get; set; }
        public DbSet<ApplicantsPerJob> ApplicantsPerJobs { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<ClientDetail> ClientDetails { get; set; }
        public DbSet<SavedJob> SavedJobs { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<RecentLogin> RecentLogins { get; set; }
        public DbSet<UserCredentials> UserCredentials { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            builder.ApplyConfiguration(new ApplicationUserEntityConfiguration());
        }
    }

    public class ApplicationUserEntityConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(u => u.FirstName).HasMaxLength(255);
            builder.Property(u => u.LastName).HasMaxLength(255);
        }
    }
}

