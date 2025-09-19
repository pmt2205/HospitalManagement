using QLBV.DAL.Repositories;
using QLBV.BLL;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// --- Connection string ---
var connectionString = builder.Configuration.GetConnectionString("ClinicDb");

// --- Đăng ký DAL repositories ---
builder.Services.AddScoped<UserRepository>(_ => new UserRepository(connectionString));
builder.Services.AddScoped<DoctorRepository>(_ => new DoctorRepository(connectionString));
builder.Services.AddScoped<DepartmentRepository>(_ => new DepartmentRepository(connectionString));
builder.Services.AddScoped<ScheduleRepository>(_ => new ScheduleRepository(connectionString));
builder.Services.AddScoped<AppointmentRepository>(_ => new AppointmentRepository(connectionString));
builder.Services.AddScoped<DiseaseCategoryRepository>(_ => new DiseaseCategoryRepository(connectionString));
builder.Services.AddScoped<DiseaseRepository>(_ => new DiseaseRepository(connectionString));
builder.Services.AddScoped<PatientRepository>(_ => new PatientRepository(connectionString));


// --- Đăng ký BLL services ---
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<DoctorService>();
builder.Services.AddScoped<DepartmentService>();
builder.Services.AddScoped<ScheduleService>();
builder.Services.AddScoped<AppointmentService>();
builder.Services.AddScoped<DiseaseCategoryService>();
builder.Services.AddScoped<DiseaseService>();
builder.Services.AddScoped<MomoService>();


// --- MVC ---
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

// --- Cookie Authentication ---
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.Name = "ClinicAuth";
    });

var app = builder.Build();

// --- Pipeline ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
