﻿using System;
using FileStorage.Areas.Identity.Data;
using FileStorage.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(FileStorage.Areas.Identity.IdentityHostingStartup))]
namespace FileStorage.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<FileStorageContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("FileStorageContextConnection")));

                services.AddDefaultIdentity<FileStorageUser>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                })
                    .AddRoles<IdentityRole>()
                   .AddEntityFrameworkStores<FileStorageContext>();
                   

            });
        }
    }
}