

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using TrainList.Model.Models;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;


namespace TrainList.Data;
public class ApplicationContext : DbContext
{
    public DbSet<TrainSheet> TrainSheets { get; set; }
    public DbSet<TrainSheetItem> TrainSheetItems { get; set; }

    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
    {
        Database.EnsureCreated();   // создаем базу данных при первом обращении
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TrainSheet>().HasData(
            new TrainSheet
            {
                Id = 1, 
                TrainNumber = "0101",
                SostavNumber = "0010",
                FromStationName = "station1",
                ToStationName = "stat3",
                LastStationName = "st3",
                WhenLastOperation = DateTime.Parse("30.06.2019 14:49:00").ToUniversalTime(),
                LastOperationName = "op1"
            }
        );
    }

}