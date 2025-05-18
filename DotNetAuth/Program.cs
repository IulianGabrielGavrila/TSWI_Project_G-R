using DotNetAuth.Data;
using DotNetAuth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}



using (var scope = app.Services.CreateScope())
{
    var svc = scope.ServiceProvider;
    var db = svc.GetRequiredService<ApplicationDbContext>();
    var um = svc.GetRequiredService<UserManager<IdentityUser>>();
    var rm = svc.GetRequiredService<RoleManager<IdentityRole>>();
    db.Database.Migrate();

    // 1) Ensure roles
    foreach (var role in new[] { "Admin", "User" })
        if (!await rm.RoleExistsAsync(role))
            await rm.CreateAsync(new IdentityRole(role));

    // 2) Create users
    async Task<IdentityUser> CreateUser(string name, string pwd, string role)
    {
        var email = $"{name}@example.com";
        var user = await um.FindByEmailAsync(email);
        if (user == null)
        {
            user = new IdentityUser
            {
                UserName = email,   // <— use the email as the user name
                Email = email,
                EmailConfirmed = true
            };
            await um.CreateAsync(user, pwd);
            await um.AddToRoleAsync(user, role);
        }
        return user;
    }

    var alice = await CreateUser("alice", "P@ssw0rd!", "Admin");
    var bob = await CreateUser("bob", "P@ssw0rd!", "User");

    // 3) Seed documents
    if (!db.Documents.Any())
    {
        db.Documents.AddRange(
            new Document { Title = "Admin Secret", Content = "Top-secret for Admin only.", OwnerId = alice.Id },
            new Document { Title = "Bob’s Notes", Content = "Bob’s personal document.", OwnerId = bob.Id }
        );
        await db.SaveChangesAsync();
    }
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
