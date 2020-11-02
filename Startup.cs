using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StudentManager.CustomerMiddlewares;
using StudentManager.Data;
using StudentManager.DataRepository;
using StudentManager.Models;

namespace StudentManager
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // 
            services.AddDbContextPool<AppDbContext>(options =>
                 options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            //services.AddMvc().AddXmlSerializerFormatters();
            services.AddControllersWithViews(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                                                .RequireAuthenticatedUser()
                                                .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            }).AddXmlSerializerFormatters();

            services.AddScoped<IStudentRepository, SQLStudentRepository>();

            //ʹ��IServiceCollection�ӿڵ�Configure()������ʵ���޸��������������
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 3;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
            });

            ///��AddIdentity()������ʹ��AddErrorDescriber()��������Ĭ�ϵĴ�����ʾ��Ϣ
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddErrorDescriber<CustomIdentityErrorDescriber>()
                .AddEntityFrameworkStores<AppDbContext>();
            //services.AddIdentity<IdentityUser, IdentityRole>()
            //       .AddEntityFrameworkStores<AppDbContext>();          
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                /// app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();

                //�����������Exception,����ExceptionHandler
                app.UseExceptionHandler("/Error");
                //������ֵ���״̬����쳣,�򷵻�UseStatusCodePagesWithReExecute
                app.UseStatusCodePagesWithReExecute("/Error/{0}");

            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            //�����֤�м��
            ///����ϣ���ܹ������󵽴�MVC�м��֮ǰ���û����������֤
            ///���,��������ܵ���UseRouting()�м��֮ǰ�����֤�м����
            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
