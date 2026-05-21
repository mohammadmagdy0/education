using Microsoft.EntityFrameworkCore;
using Education.API.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. إضافة خدمات الـ Controllers والـ OpenAPI (Swagger) لفحص الـ API
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// 2. تسجيل الـ AppDbContext وتحديد نص الاتصال (Connection String)
// هنا نستخدم قاعدة بيانات مجهزة، ويمكن تعديل نص الاتصال لاحقاً بناءً على السيرفر الخاص بك
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Server=(localdb)\\mssqllocaldb;Database=EducationDb;Trusted_Connection=True;MultipleActiveResultSets=true";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

// 3. إعدادات الـ HTTP Pipeline لتشغيل الـ API
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();

// 4. ربط الـ Controllers للعمل بشكل تلقائي
app.MapControllers();

app.Run();