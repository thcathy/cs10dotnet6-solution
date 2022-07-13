using My.Shared;

Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(builder =>
    {
        builder.UseStartup<Startup>();
    })
    .Build().Run();

Console.WriteLine("Web server is stopped");

