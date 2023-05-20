using BlobStoragerFileRtrieveUploadDeleteCore6MVC_Demo.BlobStorageServices;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();

builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = int.MaxValue; // if don't set default value is: 30 MB
});

builder.Services.Configure<FormOptions>(x =>
{
    x.MultipartHeadersLengthLimit = Int32.MaxValue;
    x.MultipartBoundaryLengthLimit = Int32.MaxValue;
    x.MultipartBodyLengthLimit = Int64.MaxValue;
    x.ValueLengthLimit = Int32.MaxValue;
    x.BufferBodyLengthLimit = Int64.MaxValue;
    x.MemoryBufferThreshold = Int32.MaxValue;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
