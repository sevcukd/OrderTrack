﻿using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderTrack
{
    public static class Global
    {
        public static IConfigurationRoot Configuration { get; }

        static Global()
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }

        public static string SQLiteDatabasePath => Configuration["DatabaseSettings:SQLiteDatabasePath"];
        public static string MsSqlConnectionString => Configuration["DatabaseSettings:MsSqlConnectionString"];
    }
}
