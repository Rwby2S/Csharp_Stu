# Asp.Net Core学习
本文本通过记录从零创建一个学生管理系统来学习ASP.NET Core MVC
## Model
### 创建学生模型
``` C#
 public class Student
{
    public int Id { get; set; }    
    public string Name { get; set; }
    public string ClassName { get; set; }
    public string Email { get; set; }
}
```
### 创建学生类接口
``` C#
 public interface IStudentRepository
{
    Student GetStudent(int id);

    //IEnumerable类型是指数据在内存中
    IEnumerable<Student> GetAllStduents();
}
```
### 创建学生测试数据存储类
``` c#
public class MockStudentRepository : IStudentRepository
{
    private List<Student> _students;

    public MockStudentRepository()
    {
        _students = new List<Student>
        {
            new Student(){ Id = 1, Name = "Ruby", ClassName = "一年级", Email = "Rwby@Ruby.com"},
            new Student(){ Id = 2, Name = "Young", ClassName = "一年级", Email = "Rwby@Young.com"},
            new Student(){ Id = 3, Name = "Black", ClassName = "一年级", Email = "Rwby@Black.com"}
        };
    }

    public IEnumerable<Student> GetAllStduents()
    {
        return _students;
    }

    public Student GetStudent(int id)
    {
       return  _students.FirstOrDefault(a => a.Id == id);
    }
}
```
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
### 在控制器中添加依赖注入
``` C#
private readonly IStudentRepository _studentRepository;

public StudentController(IStudentRepository studentRepository)
{
    _studentRepository = studentRepository;
}
```

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
    - 使用相对路径时，不用带扩展名.cshtml
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
#### ViewBag
将上述示例中ViewData修改为ViewBag
##### StudentContrller
``` C#
public IActionResult Details()
{
    Student model = _studentRepository.GetStudent(1);

    ViewBag.PageTitle = "学生详情";
    ViewBag.Student = model;

    return View();
}
```
##### Details.cshtml
``` html
  <h3>@ViewBag.PageTitle</h3>

  <div>
      姓名: @ViewBag.Student.Name
  </div>
  <div>
      邮箱: @ViewBag.Student.Email
  </div>
  <div>
      班级名称: @ViewBag.Student.ClassName
  </div>
 ```
 ### ViewData和ViewBag对比
 - ViewBag是ViewData的包装器
 - 他们都创建了一个弱类型的视图
 - ViewData使用字符串键名，来存储和查询数据
 - ViewBag使用动态属性来存储和查询数据
 - 均是运行时动态解析
 - 均不提供编译时类型检查，没有智能提示
 - 首选方法使使用强类型模型对象，将数据从控制器传递到视图
 ### 使用强类型视图
 #### 创建强类型视图
 - 在视图中使用@model指令指定模型类型
 ``` html
@model StudentManagement.Models.Student
```
 - 使用@Model访问模型对象属性
 ``` html
@Model.Name
@Model.Email
@Model.ClassName
```
 - 强类型视图提供编译时类型检查和智能提醒
### 视图模型
#### ViewModel
模型对象无法满足视图所需的所有数据时，就需要使用ViewModel了
##### StudentDetailsViewModel
``` C#
namespace StudentManagement.ViewModels
{
    public class StudentDetailsViewModel
    {
        public Student Student { get; set; }
        public string PageTitle { get; set; }
    }
}
```
##### 更改StudentContoller中的方法
``` C#
public IActionResult Details()
{
    //ViewModel的引入
    StudentDetailsViewModel studentDetailsViewModel = new StudentDetailsViewModel
    {
        Student = _studentRepository.GetStudent(1),
        PageTitle = "学生详细信息"
    };         
   return View(studentDetailsViewModel);
}
```
视图显示内容和强类型模型类似，不过此处需要修改@Model.Student.Name，因为该Model的属性为Student

### ASP.NET Core MVC中实现List视图
#### 在IStudentRepository接口中添加获取所有学生的方法
``` C#
public interface IStudentRepository
{
    Student GetStudent(int id);

    //IEnumerable类型是指数据在内存中
    IEnumerable<Student> GetAllStduents();
}
```
#### 接口实现方法中实现该方法
``` C#
//修改类型为List
 private List<Student> _students;
 
public IEnumerable<Student> GetAllStduents()
{
    return _students;
}
```
#### StudentController
``` C#
public IActionResult Index()
{
    //此处实际类型类IEnumerable<Student>
    var students = _studentRepository.GetAllStduents();
    return View(students);
}
```
#### 实现Index()方法对应的View
Index.cshtml
``` html
@model IEnumerable<StudentManagement.Models.Student> 
<!DOCTYPE html>
<html>
<head>
    <title>学生页面详情列表</title>
</head>
<body>
    <table>
        <thead>
            <tr>
                <th>ID</th>
                <th>姓名</th>
                <th>班级</th>
                <th>电子邮件</th>
            </tr>
        </thead>  
        <tbody>
            @foreach (var student in Model)
            {
            <tr>
                <td>
                    @student.Id
                </td>
                <td>
                    @student.Name
                </td>
                <td>
                    @student.ClassName
                </td>
                <td>
                    @student.Email
                </td>
            </tr>
            }
        </tbody>
    </table>
</body>
</html>
 ```
 ### MVC中布局页面的使用
 - 让web应用程序中所有的视图保持外观一致性。
 - 布局视图看起来像ASP.NET Web Form中的母页面
 - 布局视图也具有 **.cshtml** 扩展名
 - 在 **ASP.NET Core MVC** 中，默认情况下布局文件名为 **_Layout.cshtml**
 - 布局视图文件通常放在 **"View/Shared"** 的文件夹中
 - 在一个应用程序中可以包含多个布局视图文件
 其基本的结构为:
 ``` html
<!DOCTYPE html>
<html>
<head>
    <title>ViewData["Title"]</title>
</head>
<body>
    <div>
        @RenderBody()
    </div>
   

    @RenderSection("Script", required : false)
</body>
</html>
```
 #### 布局页面Sections
 在布局视图的渲染节点
 ``` html
@RenderSection("Scripts", required: false)
```
在普通视图中定义节点
``` html
@section Scripts{
    <script>
        document.write("hello");
    </script>
}
```
#### 视图开始_ViewStart.cshtml
- ViewStart中的代码会在单个视图中的代码之前执行
- 移动 **公用代码** 到ViewStart视图中，如给布局视图文件设置属性
- ViewStart文件支持分层，不同子文件下可以同时存在ViewStart文件

#### 视图导入_ViewImports
用来导入命名空间、注册模型等等n多种操作。
- ViewImports文件还支持一下指令
  - @addTagHelper
  - @removeTagHelper
  - @tagHelperPrefix
  - @model
  - @inherits
  - @inject     //自动注入
  
### MVC中的路由
- 路由技术：
  - 常规路由
  - 属性路由
  
#### 常规路由
``` C#
 app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});
```
他们之间的路由关系如下一一对应 <br/>
http://localhots:3290/Student/Details/1  
``` C#
{controller=Home}/{action=Index}/{id?}}
``` 
``` C#
 public class StudentController : Controller
{
    ……
    public IActionResult Details(int id)
    {
        //ViewModel的引入
        StudentDetailsViewModel studentDetailsViewModel = new StudentDetailsViewModel
        {
            Student = _studentRepository.GetStudent(id),
           PageTitle = "学生详细信息"
        };         
       return View(studentDetailsViewModel);
    }
}
```

#### 属性路由
属性路由可以自己指定对应的路由路径<br/>
比传统路由更加灵活，可以搭配传统路由使用。 <br/>
在控制器方法上添加路由注解，一个方法可以同时映射多个路由。
``` C#
[Route("")]
[Route("Home")]
[Route("Home/Index")]
public IActionResult Index()
{
    //此处实际类型类IEnumerable<Student>
    var students = _studentRepository.GetAllStduents();
    return View(students);
}
```
路由中也可以指定参数
``` C#
[Route("{test/{id?}")]
public IActionResult Details(int id = 1)
{
    var model = _studentRepository.GetById(id);
    var viewModel = new StudentDetailsViewModel
    {
        Student = model,
        PageTitle = "viewmodel里的页面标题"
    };
    return View(viewModel);
}
```

### 使用包管理工具安装Bootstrap
在wwwroot包下添加客户端库，“提供程序”选择unpkg，在库中搜索twitter-bootstrap，安装完毕后生成libman.json文件以及bootstrap的相关文件
#### 修改_Layout.cshtml文件
```html
<!DOCTYPE html>
<html>
<head>
    <title>ViewData["Title"]</title>
    <link href="~/css/site.css" rel="stylesheet" />
    <link href="~/lib/bootstrap/dist/css/bootstrap.css" rel="stylesheet" />
</head>
<body>
    <div>
        @RenderBody()
    </div>
   

    @RenderSection("Script", required : false)
</body>
</html>
```
#### 修改Index.cshtml
``` html
@model IEnumerable<StudentManager.Models.Student>
@{ 
    ViewData["Title"] = "学生列表页面";
}

<div class="card-deck">
    @foreach (var student in Model)
    {
        <div class="card m-3">
            <h3>@student.Name</h3>
            <img class="card-img-top" src="~/images/bao.png"/>
            <div class="card-footer text-center">
                <a href="#" class="btn btn-primary">查看</a>
                <a href="#" class="btn btn-primary">编辑</a>
                <a href="#" class="btn btn-danger">删除</a>
            </div>
        </div>
    }
</div>
 ```
### Taghelper
- 服务器端组件
- 
        
# 需要了解的技术
## 消息队列rabbitmq 
## extJs
 

 

        










       
