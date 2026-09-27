using Microsoft.AspNetCore.Authorization;
using VisoERP.Infra.Ioc;
using VisoERP.Web.Services.Cadastros;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.AutoValidateAntiforgeryTokenAttribute()));
var acessoAnonimoDesenvolvimento = builder.Environment.IsDevelopment() &&
    builder.Configuration.GetValue<bool>("Authentication:AllowAnonymousDevelopment");
var autorizacao = builder.Services.AddAuthorizationBuilder()
    .AddPolicy("GerenciarSuprimentos", policy => policy.RequireAssertion(context =>
        acessoAnonimoDesenvolvimento || context.User.IsInRole("Administrador") || context.User.IsInRole("Estoque")))
    .AddPolicy("GerenciarClientes", policy => policy.RequireAssertion(context =>
        acessoAnonimoDesenvolvimento || context.User.IsInRole("Administrador") || context.User.IsInRole("Vendas")))
    .AddPolicy("GerenciarFornecedores", policy => policy.RequireAssertion(context =>
        acessoAnonimoDesenvolvimento || context.User.IsInRole("Administrador") || context.User.IsInRole("Estoque")))
    .AddPolicy("GerenciarCategorias", policy => policy.RequireAssertion(context =>
        acessoAnonimoDesenvolvimento || context.User.IsInRole("Administrador") || context.User.IsInRole("Estoque")));
if (!acessoAnonimoDesenvolvimento)
    autorizacao.SetFallbackPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());
Modulo.RegistrarServicos(builder.Services, builder.Configuration);
builder.Services.AddScoped<ImagemSuprimentoService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

await Modulo.InicializarIdentidadeAsync(app.Services, app.Configuration);

app.Run();
