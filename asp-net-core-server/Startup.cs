using DevExpress.AspNetCore;
using DevExpress.DashboardAspNetCore;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWeb;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Json;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
namespace AspNetCoreDashboardBackend {
    public class Startup {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
       
        public Startup(IConfiguration configuration, IWebHostEnvironment hostingEnvironment) {
            Configuration = configuration;
            FileProvider = hostingEnvironment.ContentRootFileProvider;
            
            Console.WriteLine("TEST");
        }
        

        public IConfiguration Configuration { get; }
        public IFileProvider FileProvider { get; }

        public ItemDataRequestMode ItemDataRequestMode { get; set; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services) {
            // Configures services to use the Web Dashboard Control.
            services
                .AddCors(options => {
                    options.AddPolicy("CorsPolicy", builder => {
                        builder.AllowAnyOrigin();
                        builder.AllowAnyMethod();
                        builder.WithHeaders("Content-Type");
                    });
                })
                .AddDevExpressControls()
                .AddControllers();

            services.AddScoped<DashboardConfigurator>((IServiceProvider serviceProvider) => {
                DashboardConfigurator configurator = new DashboardConfigurator();
                
                configurator.SetDashboardStorage(new DashboardFileStorage(FileProvider.GetFileInfo("App_Data/Dashboards").PhysicalPath));
                configurator.SetDataSourceStorage(CreateDataSourceStorage());
                configurator.SetConnectionStringsProvider(new DashboardConnectionStringsProvider(Configuration));
                configurator.ConfigureDataConnection += Configurator_ConfigureDataConnection;
                configurator.ConfigureDataReloadingTimeout += Default_ConfigureDataReloadingTimeout;
                configurator.SetConnectionStringsProvider(new MyDataSourceWizardConnectionStringsProvider());
                


                return configurator;
            });
        }

        
        private void Default_ConfigureDataReloadingTimeout(object sender, ConfigureDataReloadingTimeoutWebEventArgs e)
        {
            Console.WriteLine("TEST CACHE RELOAD");

           
            e.DataReloadingTimeout = TimeSpan.FromHours(24);
            Console.WriteLine($"Data reloaded for {e.DataSourceName} at {DateTime.Now}");

          
            if (e.DataReloadingTimeout.TotalSeconds < 1)
            {
                Console.WriteLine($"Data for {e.DataSourceName} loaded from cache at {DateTime.Now}");
            }
            else
            {
                Console.WriteLine($"Data for {e.DataSourceName} loaded freshly at {DateTime.Now}");
            }
        }

        
        

        public class MyDataSourceWizardConnectionStringsProvider : IDataSourceWizardConnectionStringsProvider
        {
            public Dictionary<string, string> GetConnectionDescriptions()
            {
                Dictionary<string, string> connections = new Dictionary<string, string>();

             
                connections.Add("jsonUrlConnection", "JSON URL Connection");
                connections.Add("msSqlConnection", "MS SQL Connection");
                return connections;
            }

            public DataConnectionParametersBase GetDataConnectionParameters(string name)
            {
                
                if (name == "jsonUrlConnection")
                {
                    return new JsonSourceConnectionParameters()
                    {
                        JsonSource = new UriJsonSource(
                            new Uri("https://raw.githubusercontent.com/DevExpress-Examples/DataSources/master/JSON/customers.json"))
                    };
                }
                else if (name == "msSqlConnection")
                {
                    return new MsSqlConnectionParameters("localhost", "Northwind", "", "", MsSqlAuthorizationType.Windows);
                }
                throw new System.Exception("The connection string is undefined.");
            }
        }



        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            // Registers the DevExpress middleware.
            app.UseDevExpressControls();
            app.UseRouting();
            app.UseCors("CorsPolicy");
            app.UseEndpoints(endpoints => {
                // Maps the dashboard route.
                endpoints.MapDashboardRoute("api/dashboard", "DefaultDashboard");
                // Requires CORS policies.
                endpoints.MapControllers().RequireCors("CorsPolicy");
            });
        }

        public DataSourceInMemoryStorage CreateDataSourceStorage() {
            DataSourceInMemoryStorage dataSourceStorage = new DataSourceInMemoryStorage();
                        
            DashboardJsonDataSource jsonDataSourceSupport = new DashboardJsonDataSource("Support");
            jsonDataSourceSupport.ConnectionName = "jsonSupport";
            jsonDataSourceSupport.RootElement = "Employee";
            dataSourceStorage.RegisterDataSource("jsonDataSourceSupport", jsonDataSourceSupport.SaveToXml());

            DashboardJsonDataSource jsonDataSourceCategories = new DashboardJsonDataSource("Categories");
            jsonDataSourceSupport.ConnectionName = "jsonCategories";
            jsonDataSourceCategories.RootElement = "Products";
            dataSourceStorage.RegisterDataSource("jsonDataSourceCategories", jsonDataSourceCategories.SaveToXml());

            DashboardJsonDataSource jsonDataSourceLocal = new DashboardJsonDataSource("LocalJSONDataSource");
            jsonDataSourceLocal.ConnectionName = "jsonLocal";
            jsonDataSourceLocal.RootElement = "DATABASE"; // Nahraïte za skutoèný názov koreòového elementu vo vašom súbore
            dataSourceStorage.RegisterDataSource("jsonDataSourceLocal", jsonDataSourceLocal.SaveToXml());



            return dataSourceStorage;


        }
        private void Configurator_ConfigureDataConnection(object sender, ConfigureDataConnectionWebEventArgs e) {
            if (e.ConnectionName == "jsonSupport") {
                Uri fileUri = new Uri(FileProvider.GetFileInfo("App_data/Support.json").PhysicalPath, UriKind.RelativeOrAbsolute);
                JsonSourceConnectionParameters jsonParams = new JsonSourceConnectionParameters();
                jsonParams.JsonSource = new UriJsonSource(fileUri);
                e.ConnectionParameters = jsonParams;
            }
            if (e.ConnectionName == "jsonCategories") {
                Uri fileUri = new Uri(FileProvider.GetFileInfo("App_data/Categories.json").PhysicalPath, UriKind.RelativeOrAbsolute);
                JsonSourceConnectionParameters jsonParams = new JsonSourceConnectionParameters();
                jsonParams.JsonSource = new UriJsonSource(fileUri);
                e.ConnectionParameters = jsonParams;
            }


            if (e.ConnectionName == "jsonLocal")
            {
                // Nastavte cestu k vášmu JSON súboru v rámci adresára App_Data
                Uri fileUri = new Uri(FileProvider.GetFileInfo("App_Data/sk_database.json").PhysicalPath, UriKind.RelativeOrAbsolute);

                JsonSourceConnectionParameters jsonParams = new JsonSourceConnectionParameters();
                jsonParams.JsonSource = new UriJsonSource(fileUri);
                e.ConnectionParameters = jsonParams;
            }



        }






    }
}