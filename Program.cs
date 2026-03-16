using MatrixYhToolService.MatrixServices;
using MatrixYhToolService.MatrixTool;
using MatrixYhToolService.Model;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//注册Newtonsoft.Json 作为默认序列化器
builder.Services.AddControllers().AddNewtonsoftJson();

// 注册GBK编码
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

// 注册Com组件单例
builder.Services.AddSingleton<YhInterfaceHelper>(sp => YhInterfaceHelper.Instance);

//注册自定义服务
builder.Services.AddScoped<MatrixHandleService>();

// 注册托管服务，负责生命周期管理
builder.Services.AddHostedService<YhInterfaceHostedService>();

// 注册文件路径配置
builder.Services.Configure<FileStorageOptions>(builder.Configuration.GetSection(FileStorageOptions.SectionName));


// 添加 CORS 服务
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policy =>
        {
            //policy.WithOrigins("http://你的前端IP:端口") // 指定允许的来源
            //      .AllowAnyHeader()
            //      .AllowAnyMethod();
            policy.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader(); //允许所有来源
        });
});

// 添加日志服务
builder.Logging.ClearProviders();
builder.Logging.AddConsole(); // 或其他提供程序

// 配置 Serilog日志管理
builder.Host.UseSerilog((context, loggerConfig) =>
{
    var logDir = context.Configuration["FileStorage:LogPath"];
    if (string.IsNullOrEmpty(logDir))
    {
        logDir = "Logs"; // 默认
    }
    var logPath = Path.Combine(context.HostingEnvironment.ContentRootPath, logDir, "log.txt");
    // 确保目录存在
    var directory = Path.GetDirectoryName(logPath);
    if (!string.IsNullOrEmpty(directory))
    {
        Directory.CreateDirectory(directory);
    }

    loggerConfig.ReadFrom.Configuration(context.Configuration)   //从 appsettings.json 读取配置
                .MinimumLevel.Information()
                .WriteTo.File(logPath,
                    rollingInterval: RollingInterval.Day, // 每天滚动一次
                    retainedFileCountLimit: 15, // 保留最近15天的文件
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}");
});

var app = builder.Build();

// 初始化静态日志类
var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
MatrixLogHelper.Initialize(loggerFactory);

app.Urls.Add("http://*:8023"); // 监听HTTP IP端口
//app.Urls.Add("https://*:44372");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();// HTTPS 重定向干扰,开发环境中注释掉,避免潜在干扰

app.UseRouting();

// 使用 CORS(必须放在 UseRouting 之后，UseEndpoints 和 MapControllers 之前)实际上放在 UseAuthorization 之前即可
app.UseCors("AllowSpecificOrigin");

// 临时添加：打印请求方法
app.Use(async (context, next) =>
{
    //Console.WriteLine($"Request Method: {context.Request.Method}, Path: {context.Request.Path}");
    var loggerFactory = context.RequestServices.GetRequiredService<ILoggerFactory>();
    var logger = loggerFactory.CreateLogger("RequestLogging");
    logger.LogInformation("Request Method: {Method}, Path: {Path}", context.Request.Method, context.Request.Path);
    await next();
});

app.UseAuthorization();

app.MapControllers();

app.Run();
