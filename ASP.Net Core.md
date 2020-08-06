# Asp.Net Core学习
## 依赖注入
- ASP.NET Core依赖注入容器注册服务
  - AddSingleton() : 全局单例
  - AddTransient()：每次使用都创建新对象
  - AddScoped()：每个http请求中创建和使用同一个对象
``` C#
public void ConfigureServices(IServiceCollection services)
{
    // services.AddControllersWithViews();
    services.AddMvc();
    services.AddSingleton<IStudentRepository,MockStudentRepository>();
}
```
## 控制器
Contoller服务于Http请求到我们的应用程序，作为MVC中的控制器来处理访问的请求，并且能够响应相关的请求。
### 内容格式协商
在控制器方法中使用 ObjectResult 返回类型，支持内容协商，根据请求头参数返回数据，<br/>
``` C#
// 支持内容格式协商
public ObjectResult Details(int id)
{
    return new ObjectResult(_studentRepository.GetById(id));
}
```
例如我们要返回一个xml格式的内容，<br/>
如：

Accept: application/xml<br/>

除了要在控制器中设定上述的返回类型，还需要在StartUp的ConfigureServices中添加Xml序列化器<br/>
``` C#
//将数据序列化为xml格式
services.AddMvc().AddXmlSerializerFormatters();
```

如果要返回一个视图，那么在Controller中将放发的返回类型改为IActionResult
``` C#
public IActionResult Privacy()
{
    return View();
}
```
### 小结
- 当来自浏览器的请求到达我们的应用程序时，作为MVC设计模式中的控制器，它处理传入的http请求并相应用户操作。

- 控制器构建模型(Model)
- 如果我们正在构建API，则将模型数据返回给调用方
- 或者选择"View"视图并将模型数据传递给视图，然后视图生成所需的HTML来显示数据

## 视图入门
了解它是干什么的，了解它在中间的处理流程，以及在MVC设计模式中所扮演的角色。<br/>
服务于Controller和Model，呈现给用户的视图，来显示Model中数据的HTML视图文件。<br/>






       
