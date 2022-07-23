namespace PlatformService.Data
{
  public static class PrepDb
  {
    public static void PrepDatabase(IApplicationBuilder app)
    {
      using (var serviceScope = app.ApplicationServices.CreateScope())
      {
        SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>());
      }
    }

    private static void SeedData(AppDbContext context)
    {
      if (!context.Platforms.Any())
      {
        Console.WriteLine("Seeding database");

        context.Platforms.AddRange(
          new Models.Platform() { Name = "ASP.NET", Publisher = "Microsoft", Cost = "Free" },
          new Models.Platform() { Name = "SQL Server", Publisher = "Microsoft", Cost = "Paid" }
          );

        context.SaveChanges();
      }

      else
      {
        Console.WriteLine("Database already seeded");
      }
    }
  }
}
