using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using VisoERP.Web.ModelBinders;
using VisoERP.Infra.Ioc;
using VisoERP.Web.Services.Cadastros;
using VisoERP.Web.Health;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.AutoValidateAntiforgeryTokenAttribute());
    options.ModelBinderProviders.Insert(0, new DecimalPtBrModelBinderProvider());
});
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
        acessoAnonimoDesenvolvimento || context.User.IsInRole("Administrador") || context.User.IsInRole("Estoque")))
    .AddPolicy("GerenciarEstoque", policy => policy.RequireAssertion(context =>
        acessoAnonimoDesenvolvimento || context.User.IsInRole("Administrador") || context.User.IsInRole("Estoque")))
    .AddPolicy("GerenciarVendas", policy => policy.RequireAssertion(context =>
        acessoAnonimoDesenvolvimento || context.User.IsInRole("Administrador") || context.User.IsInRole("Vendas")))
    .AddPolicy("GerenciarFinanceiro", policy => policy.RequireAssertion(context =>
        acessoAnonimoDesenvolvimento || context.User.IsInRole("Administrador") || context.User.IsInRole("Financeiro")));
if (!acessoAnonimoDesenvolvimento)
    autorizacao.SetFallbackPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());
Modulo.RegistrarServicos(builder.Services, builder.Configuration);
builder.Services.AddScoped<ImagemSuprimentoService>();
builder.Services.AddScoped<IDatabaseConnectionProbe, DatabaseConnectionProbe>();

if (!builder.Environment.IsDevelopment())
{
    builder.Services.Configure<ForwardedHeadersOptions>(options =>
        ForwardedProxyConfiguration.Configure(options, builder.Configuration["FORWARDED_PROXY_IP"], required: true));
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseForwardedHeaders();
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapDatabaseHealthCheck();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

await Modulo.InicializarIdentidadeAsync(app.Services, app.Configuration);

app.Run();
