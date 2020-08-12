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
 
### 修改Details.cshtml
``` html
@model StudentManager.ViewModels.StudentDetailsViewModel
@{ 
    ViewData["Title"] = "学生详情页";
}

<div class="row justify-content-center m-3">
    <div class="col-sm-8">
        <h1>@Model.Student.Name</h1>
    </div>

    <div class="card-body text-center">
        <img class="card-img-top" src="~/images/bao.png" />

        <h4>学生ID: @Model.Student.Id</h4>
        <h4>邮箱: @Model.Student.Email</h4>
        <h4>班级名称: @Model.Student.ClassName</h4>
    </div>
    <div class="card-footer text-center">
        <a href="@Url.Action("index","student")" class="btn btn-primary">返回</a>
        <a href="#" class="btn btn-primary">编辑</a>
        <a href="#" class="btn btn-danger">删除</a>
    </div>
</div>
```

### Taghelper(标记助手)
优点：根据参数自动生成，不需要手写超链接<br/>
在ViewImport中添加TagHelper
``` html
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
```
比如，链接TagHelper使用
``` html
 <a asp-controller="student" asp-action="details" asp-route-id="@student.Id"
                   class="btn btn-primary">查看</a>
```
手动编写的连接内容如下
``` html
<a href="/Student/Details/2">查看</a>
```

#### 为什么要使用TagHelper
当路由模板发生变化时，手动编写连接因为路由错误导致无法正常跳转<br />
而使用TagHelper则不需要担心这种情况

#### 图片的TagHelper
在img标签中使用 **asp-append-version="true"** 对该标签进行增强
- Image TagHelper增强了 <img>标签，为静态图像文件提供 **缓存破坏行为**
- 生成唯一的散列值讲其附加到图片的URL。此唯一字符串会提示浏览器从服务器重新加载图片，而不是从浏览器缓存重新加载

#### TagHelper开发环境
##### 环境标签编辑器
- Include和exclude属性
在开发环境中使用本地css文件，在非开发环境下使用的是CDN的css文件。<br/>
注：```integrity```是用来做完整性检查的，保证CDN提供文件的完整和安全。<br/>
integrity （大部分情况）是给 CDN 的静态文件使用的 <br/>
CDN虽然好但 CDN 有可能被劫持，导致下载的文件是被篡改过的（比如通过 DNS 劫持），有了 integrity 就可以检查文件是否是原版。<br/>
**总之**: 只有当你的网页域名和要载入的静态文件存放的站点域名不一样的时候，使用这两个属性才有意义（并且因浏览器的规定 crossorigin 属性只有这个时候才能正常使用）。
``` html
 <environment include="Development">
     <link href="~/lib/bootstrap/dist/css/bootstrap.css" rel="stylesheet" />
 </environment>

 <environment exclude="UnDevelopment">
     <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" 
           integrity="sha384-JcKb8q3iqJ61gNV9KGb8thSsNjpSL0n8PARn9HuZOnIxN0hoP+VmmDGMN5t9UJ0Z" crossorigin="anonymous">
 </environment>
``` 
为了防止CDN加载失败页面无法显示，可以加上fallback相关属性，第一个是失败时加载的文件，第二个是不检查这个文件的完整性
``` html
asp-fallback-href="~/lib/twitter-bootstrap/dist/css/bootstrap.css"
asp-suppress-fallback-integrity="true"
```
### 为学生管理系统添加导航栏
修改_Layout.cshtml文件
``` html
 <div class="container">
        <nav class="navbar navbar-expand-sm bg-dark navbar-dark">

            <a class="navbar-brand" asp-controller="student" asp-action="index">
                <img src="~/images/banner.jpg" width="40" height="30" alt="Alternate Text" />
            </a>
            <button class="navbar-toggler" type="button" data-toggle="collapse" data-target="#collapsibleNavbar">
                <span class="navbar-toggler-icon"></span>
            </button>
            
            <div id="collapsibleNavbar" class="collapse navbar-collapse">
                <ul class="navbar-nav">
                    <li class="nav-item">
                        <a class="nav-link" asp-controller="student" asp-action="Index">学生列表</a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link" asp-controller="student" asp-action="create">添加学生</a>
                    </li>
                </ul>
            </div>
        </nav>

        @RenderBody()
    </div>

    <script src="~/lib/jquery/dist/jquery.js"></script>
    <script src="~/lib/bootstrap/dist/js/bootstrap.js"></script>
```

### Core中模型绑定
- 要将HTTP的请求数据绑定到控制器操作方法上对应的参数上，模型绑定将按照以下指定的顺序在以下位置查找来自HTTP请求的数据。
 - 表单中的值
 - 路由中的值
 - 查询字符串
 
### IStudentRepository定义Add方法
``` C#
 public Student Add(Student student);
```
#### 向MockStudentRepository中添加Add方法
``` C#
public Student Add(Student student)
{
    student.Id = _students.Max(s => s.Id) + 1;
    _students.Add(student);
    return student;
}
```
#### StudentContorller
``` C#
 [HttpGet]
 public IActionResult Create()
 {
     return View();
 }

 //重定向实体，然后在创建视图页面中创建实体，通过按钮提交，存储到存储体中
 [HttpPost]
 public RedirectToActionResult Create(Student student)
 {
     Student newStudent = _studentRepository.Add(student);
     return RedirectToAction("details", new { id = newStudent.Id });
 }
``` 
### 模型验证
- Required ： 指定该字段是必填的
- Range : 指定允许的最小值和最大值
- MinLength : 使用MinLength指定字符串的最小长度
- MaxLength : 使用MaxLength指定字符串的最大长度
- Compare : 比较模型的 **2**个属性。例如，比较Emial和ConfirmEmail属性
- RegularExpression : 正则表达式 验证提供的值是否与正则表达式指定的模式匹配
        
#### 第一步:在属性上变价验证属性
``` C#
 public class Student
    {
        public int Id { get; set; }    
        [Display(Name = "姓名")]
        //自定义验证信息
        [Required(ErrorMessage = "请输入姓名"), MaxLength(50, ErrorMessage = "姓名长度不能超过50个字符")]
        public string Name { get; set; }

        [Display(Name = "班级信息")]
        public ClassNameEnum ClassName { get; set; }

        [Required]
        [Display(Name = "电子邮箱")]
        [RegularExpression(@"^[a-zA-Z0-9_-]+@[a-zA-Z0-9_-]+(\.[a-zA-Z0-9_-]+)+$", ErrorMessage = "邮箱格式不正确")]
        public string Email { get; set; }

        //public string PhotoPath { get; set; }
    }
    
    public enum ClassNameEnum
    {
        [Display(Name = "未分配")]
        None,
        [Display(Name = "一年级")]
        FirstGrade,
        [Display(Name = "二年级")]
        SecondGrade,
        [Display(Name = "三年级")]
        GradeThree
    }
```
#### 第二步: 使用ModelState.IsValid属性验证属性是成功还是失败
``` C#
[HttpPost]
public IActionResult Create(Student student)
{
    if (ModelState.IsValid)
    {
        Student newStudent = _studentRepository.Add(student);
        return RedirectToAction("Details", new { id = newStudent.Id });
    }

    return View();
}
```
#### 第三步：显示错误信息
使用 ```asp-validation-for```和```asp-validation-summary```标签帮助其来显示错误信息
``` html
 <form asp-controller="student" asp-action="create" method="post" class="mt-3">
     <div asp-validation-summary="All" class="text-danger"></div>
     <div class="form-group row">
         <label asp-for="Name" class="col-sm-2 col-form-label"></label>

         <div class="col-sm-10">
             <input asp-for="Name" class="form-control" placeholder="请输入名字"/>
             <span asp-validation-for="Name" class="text-danger"></span>
         </div>

     </div>

     <div class="form-group row">
         <label asp-for="Email" class="col-sm-2 col-form-label"></label>
         <div class="col-sm-10">
             <input asp-for="Email" class="form-control" placeholder="请输入邮箱" />
             <span asp-validation-for="Email" class="text-danger"></span>
         </div>
     </div>

     <div class="form-group row">
         <label asp-for="ClassName" class="col-sm-2 col-form-label"></label>
         <div class="col-sm-10">
             <select asp-for="ClassName" asp-items="Html.GetEnumSelectList<ClassNameEnum>()" class="custom-select mr-sm-2">
                 <option value="">请选择</option>
             </select>
             <span asp-validation-for="ClassName" class="text-danger"></span>
         </div>
     </div>

     <div class="col-sm-10">
         <button type="submit" class="btn btn-primary">创建</button>
     </div>                
</form>
```

### select标签的验证
设置班级信息有验证属性，讲其设置为可空的类型
``` C#
[Display(Name = "班级信息")]
[Required]
public ClassNameEnum? ClassName { get; set; }
```

## 下一阶段
- EF的基础
- 单层架构和分层架构的区别
- 图片上传
- 404异常页面的拦截
- 全局异常的拦截
- 日志记录
- 身份认证

## AddSingleTon、AddScoped、AddTransient
|服务类型             |   同一个Http请求的范围 |  横跨多个不同Http请求   |
| :------------------| -------------------:| :-------------------: |
| Scoped Service     |     同一个实例        |      新实例           |
| Transient Service  |      新的实例         |      新实例           |
| Singleton Service  |     同一个实例        |      同一个实例        |

# EF Core
## 什么是ORM，为什么要使用ORM
- oRM(对象关系映射器) Object-Relational Mapper
- EF Core是轻量级，可扩展和开源的软件
- EF Core也是跨平台额
- EF Core是微软官方推荐的数据访问平台
## Code First 和DB First
Code First
- 1.创建领域类DbContext 
- 2.EF Core
- 3.Database
DB First
- 1.Database
- 2. EF Core
- 3.领域类&DbContext类

## 多层Web应用程序
|       内容          |         功能     | 
| :------------------| :-------------------:|
| 表现层              |    多页MVC、WebApi      | 
| 应用层              |     针对用户场景、用例设计应用层服务，隔离底层细节       | 
| 领域层              |    专注于维护业务规则    |
| 持久化层            |     负责数据查询和持久化         | 
**注意：** 编写业务代码和其处理流程时，尽量在纯粹的内存环境中进行考虑，更利于引入设计模式，不会被底层存储细节打断思路

## Entity Framework Core包
```
graph TD
     A[Micirosoft.EntityFrameworkCore.SqlServer] -->B(Micirosoft.EntityFrameworkCore.Relational)
     B -->C(Micirosoft.EntityFrameworkCore)
```
## DbContext
### 实现DbContext
``` C#
public class AppDbContext : DbContext
{

    // 将应用程序的配置传递给DbContext
    public AppDbContext(DbContextOptions<AppDbContext> options)
        :base(options)
    {
    }

    // 对要使用到的每个实体都添加 DbSet<TEntity> 属性
    // 通过DbSet属性来进行增删改查操作
    // 对DbSet采用Linq查询的时候，EFCore自动将其转换为SQL语句来对基础数据库做查询操作
    public DbSet<Student> students { get; set; }
}
```
### 使用Sql Server
#### 注册DbContext连接池
``` C#
services.AddDbContextPool<AppDbContext>(
     optionsAction : options => options.UseSqlServer(_configuration.GetConnectionString("StudentDBConnection"))
);
```
 其中，本地SqlServer数据库的配置，在appserttings.json中：           
``` Json
"ConnectionStrings": {
      "StudentDBConnection": "server=(localdb)\\MSSQLLocalDB;database=StudentDB;Trusted_Connection=true"
    }
```
## 仓储模式
- IStudentRepository
 - 添加
 - 删除
 - 修改
 - 查询
使用仓储
- 内存仓储 ：MockStudentRepository
- SQL仓储 : SQLStudentRepository
### 实现仓储
``` C#
public class SQLStudentRepository : IStudentRepository
{
    private readonly AppDbContext _context;

    public SQLStudentRepository(AppDbContext context)
    {
        _context = context;
    }

    public Student Add(Student student)
    {
        _context.Students.Add(student);
        _context.SaveChanges();
        return student;
    }

    public Student Delete(int id)
    {
        Student student = _context.Students.Find(id);
        if(student != null)
        {
            _context.Students.Remove(student);
            _context.SaveChanges();
        }
        return student;
    }

    public IEnumerable<Student> GetAllStduents()
    {
        return _context.Students;
    }

    public Student GetStudent(int id)
    {
        return _context.Students.Find(id);
    }

    public Student Update(Student updatestudent)
    {
        var student = _context.Students.Attach(updatestudent);
        student.State = Microsoft.EntityFrameworkCore.EntityState.Modified;

        _context.SaveChanges();
        return updatestudent;

    }
}
```
## 迁移功能
- 迁移时为了让我们的数据库架构设计与应用程序的模型类保持同步的功能
### 迁移功能常用指令
- get-help about_entityframeworkcore
  - 提供EF Core的帮助信息
- Add-Migration
  - 添加新迁移记录
- Update-Database
  - 讲数据库更新为指定的迁移
### 进行迁移
``` json
"ConnectionStrings": {
   "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=StudentDB;Trusted_Connection=True;MultipleActiveResultSets=true",
}
```
### 数据库种子
添加静态方法类存放种子
``` c#
public static class ModelBuilderExtensions
{
    public static void Seed(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Student>().HasData(
            new Student(){ Id = 1, Name = "Ruby", ClassName = ClassNameEnum.FirstGrade, Email = "Rwby@Ruby.com"},
            new Student(){ Id = 2, Name = "Young", ClassName = ClassNameEnum.SecondGrade, Email = "Rwby@Young.com"},
            new Student(){ Id = 3, Name = "Black", ClassName = ClassNameEnum.GradeThree, Email = "Rwby@Black.com"}
            );
    }
}
```
在AppDbContext中添加方法
``` C#
 protected override void OnModelCreating(ModelBuilder modelbuilder)
 {
     modelbuilder.Seed();
 }
 ```
 再次进行迁移
### 领域模型与数据库架构
-使用 **迁移功能**来同步我们的领域模型和数据库架构设计，使他们保持一致
- 使用 **Add-Migration**命令来创建一个新的迁移记录
- 要更新我们的数据库架构，需要采用 **Update-Databse**命令
- 使用 **Remove-Migration**命令可以删除尚未应用到数据库的迁移记录
- **_EFMigrationHistory**表用于追踪应用于数据库的迁移记录信息
- **ModelSnapshot.cs**文件顾名思义，它是当前模型的快照，用于确定将在下一次迁移时发生了什么变化

## 上传文件
### 定义ViewModel
要上传的字段采用 IFormFile 类型
``` C#
[Display(Name = "图片")]
public IFormFile Photo { get; set; }
```
### 编辑视图
修改cshtml视图文件，修改模型绑定：
``` html
@model StudentCreateViewModel
```
加入上传文件表单项
``` html
<div class="form-group row">
    <label asp-for="Photo" class="col-sm-2 col-form-label"></label>
    <div class="col-sm-10">
        <div class="custom-file">
            <input asp-for="Photo" class="form-control custom-file-input" />
            <label class="custom-file-label">请选择照片....</label>
        </div>               
    </div>
</div>
```
使表单上显示文件路径内容
``` javascript
 @section Script{
     <script type="text/javascript">
         $(document).ready(function () {
             $('.custom-file-input').on("change", function () {
                 var filename = $(this).val().split("\\").pop();

                 $(this).next(".custom-file-label").html(filename);
             });

         });
     </script>
 }
```
修改控制器，使之能够将文件内容复制到wwwroot的images目录下:
``` c#
private readonly IStudentRepository _studentRepository;
private readonly IWebHostEnvironment _hostingEnvironment;

public StudentController(IStudentRepository studentRepository, IWebHostEnvironment hostingEnvironment)
{
    _studentRepository = studentRepository;
    _hostingEnvironment = hostingEnvironment;
}

//创建学生信息
[HttpPost]
 public IActionResult Create(StudentCreateViewModel model)
 {
     if (ModelState.IsValid)
     {
         String uniqueFileName = null;

         if (model.Photo != null)
         {
             string uploadsFloder = Path.Combine(_hostingEnvironment.WebRootPath, "images");
             uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
             string filePath = Path.Combine(uploadsFloder,uniqueFileName);

             model.Photo.CopyTo(new FileStream(filePath, FileMode.Create));
         }
         Student newStudent = new Student
         {
             Name = model.Name,
             Email = model.Email,
             ClassName = model.ClassName,
             PhotoPath = uniqueFileName
         };
         _studentRepository.Add(newStudent);
         return RedirectToAction("Details", new { id = newStudent.Id });
     }

     return View();
 }
```
### 多文件上传
#### ViewModel
修改类型为List
``` C#
[Display(Name = "图片")]
public List<IFormFile> Photos { get; set; }
```
#### 修改视图
``` html
<div class="form-group row">
     <label asp-for="Photos" class="col-sm-2 col-form-label"></label>
     <div class="col-sm-10">
         <div class="custom-file">
             <input asp-for="Photos" multiple class="form-control custom-file-input" />
             <label class="custom-file-label">请选择图片 可以一次选择多张</label>
         </div>               
     </div>
 </div>
```
**js代码:**
``` javascript
@section Script{
   <script type="text/javascript">
       $(document).ready(function () {
           $('.custom-file-input').on("change", function () {
               console.log($(this));
               var fileLable = $(this).next(".custom-file-label");
               var files = $(this)[0].files;
               if (files.length > 1) {
                   fileLable.html('您已选择了' + files.length + '个文件');
               } else if (files.length == 1) {
                   fileLable.html(filename);
               }
               //var filename = $(this).val().split("\\").pop();                        
           });
       });
   </script>
}
```
#### 修改控制器
``` C#
if(model.Photos != null && model.Photos.Count > 0)
{
    foreach (var photo in model.Photos)
    {
        if (photo != null)
        {
            string uploadsFloder = Path.Combine(_hostingEnvironment.WebRootPath, "images");
            uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
            string filePath = Path.Combine(uploadsFloder, uniqueFileName);

            photo.CopyTo(new FileStream(filePath, FileMode.Create));
        }
    }
}
```
### 添加学生信息编辑功能
#### 添加编辑视图
``` html
@model StudentManager.ViewModels.StudentEditViewModel
@{
    ViewData["Title"] = "编辑学生信息";
    var ExistingPhotoPath = "~/images/" + (Model.ExistingPhotoPath ?? "bao.png");
}

<form enctype="multipart/form-data" asp-controller="student" asp-action="edit" method="post" class="mt-3">
    <div asp-validation-summary="All" class="text-danger"></div>

    <input hidden asp-for="Id" />
    <input hidden asp-for="ExistingPhotoPath" />

    <div class="form-group row">
        <label asp-for="Name" class="col-sm-2 col-form-label"></label>

        <div class="col-sm-10">
            <input asp-for="Name" class="form-control" placeholder="请输入名字" />
            <span asp-validation-for="Name" class="text-danger"></span>
        </div>

    </div>

    <div class="form-group row">
        <label asp-for="Email" class="col-sm-2 col-form-label"></label>
        <div class="col-sm-10">
            <input asp-for="Email" class="form-control" placeholder="请输入邮箱" />
            <span asp-validation-for="Email" class="text-danger"></span>
        </div>
    </div>

    <div class="form-group row">
        <label asp-for="ClassName" class="col-sm-2 col-form-label"></label>
        <div class="col-sm-10">
            <select asp-for="ClassName" asp-items="Html.GetEnumSelectList<ClassNameEnum>()" class="custom-select mr-sm-2">
                <option value="">请选择</option>
            </select>
            <span asp-validation-for="ClassName" class="text-danger"></span>
        </div>
    </div>

    <div class="form-group row">
        <label asp-for="Photo" class="col-sm-2 col-form-label"></label>
        <div class="col-sm-10">
            <div class="custom-file">
                <input asp-for="Photo" class="form-control custom-file-input" />
                <label class="custom-file-label">请选择照片....</label>
            </div>
        </div>
    </div>

    <div class="form-group row row col-sm-4 offset-4">
        <img class="imagesThumbnail" src="@ExistingPhotoPath" asp-append-version="true" />
    </div>

    <div class="form-group row">
        <div class="col-sm-10">
            <button type="submit" class="btn btn-primary">更新</button>
            <a asp-controller="student" asp-action="Index" class="btn btn-primary">取消</a> 
        </div>
    </div>  

    @section Script{
        <script type="text/javascript">
                $(document).ready(function () {
                    $('.custom-file-input').on("change", function () {
                        var filename = $(this).val().split("\\").pop();

                        $(this).next(".custom-file-label").html(filename);
                    });

                });
        </script>
    }
</form>
```
#### Controller添加Edit方法

``` c#

```

## 异常处理
### UseStatusCodePagesWithRedirects和UseStatusCodePagesWithReExecute的区别
推荐用 UseStatusCodePagesWithReExecute 而不是 UseStatusCodePagesWithRedirects，前者在管道内执行执行错误跳转url，后者会重定向到该url，导致http错误状态码变成新页面的正常执行的200码了。
#### UseStatusCodePagesWithRedirects
- UseStatusCodePagesWithRedirects会发出重定向请求，而地址栏中url将会发生更改。
- 当发生真实错误的时候，它返回一个sucess status代码(200),它在语义上显示的是不正确的。

#### UseStatusCodePagesWithReExecute
- 重新执行管道请求并返回原始状态代码(例如404)
- 由于它是重新执行管道请求，而不是发出重定向请求；我们也会在地址栏中保留原始url，例如http://localhost/market/food

### 全局异常处理


Job任务
  
# 需要了解的技术
## 消息队列rabbitmq 
## extJs
 

 

        










       
