using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FileStorage.Models;

namespace FileStorage.Data
{
    public class FileStorageContextData : DbContext
    {
        public FileStorageContextData (DbContextOptions<FileStorageContextData> options)
            : base(options)
        {
        }

        public DbSet<Movie> Movie { get; set; }
    }
}
