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

### 自定义视图发现
- View()或View(object model):查找与操作方法同名的视图文件
- View(string viewName)
  - 可查询自定义名称的视图文件
  - 您可以指定视图名称或视图文件路径
    - 绝对路径必须指定.cshtml扩展名
    - 使用相对庐江时，不用带扩展名.cshtml
### 从控制器传递数据到视图
- 使用ViewData
- 使用ViewBag
- 强类型视图

#### ViewData
- 是弱类型的字典对象
- 使用string类型的键值，存储和查询ViewData字典中的数据，
- 运行时动态解析
- 没有智能提示，编译时也没有类型检查
示例:需要在视图上显示第一个学生的信息
##### StudentContrller
``` C#
 public IActionResult Details()
  {
      Student model = _studentRepository.GetStudent(1);

      ViewData["PageTitle"] = "Student Details";
      ViewData["Student"] = model;

      return View();
  }
  ```
  ##### Details.cshtml
``` html
@using StudentManagement.Models

<!DOCTYPE html>
<html>
<head>
    <title></title>
</head>
<body>
    <h3>@ViewData["PageTitle"]</h3>

    @{
        var student = ViewData["Student"] as Student;
    }

    <div>
        姓名: @student.Name
    </div>
    <div>
        邮箱: @student.Email
    </div>
    <div>
        班级名称: @student.ClassName
    </div>
</body>
</html>
```
  
        










       
