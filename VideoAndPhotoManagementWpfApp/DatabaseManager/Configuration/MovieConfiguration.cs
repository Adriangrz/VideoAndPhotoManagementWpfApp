using DatabaseManager.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManager.Configuration
{
    public class MovieConfiguration : IEntityTypeConfiguration<Movie>
    {
        public void Configure(EntityTypeBuilder<Movie> builder)
        {
            builder.ToTable("Movies");

            builder.HasKey(x => x.MovieId);

            builder.HasOne(m => m.Category)
                .WithMany(ca => ca.Movies)
                .HasForeignKey(m => m.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
