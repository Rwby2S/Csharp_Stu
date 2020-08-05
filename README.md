# Csharp_Stu
Csharp x学习
# 事件
## 事件模型的五个组成部分
1.事件的拥有者<br/>
2.事件的成员<br/>
3.事件的响应者<br/>
4.事件处理器<br/>
5.事件订阅——把事件处理器与事件关联在一起，本质上是一种以委托类型为基础的“约定”
## 注意
事件处理器是方法成员
# 了解如何加载相关数据
原文链接如下:https://blog.csdn.net/long870294701/article/details/87882615
## EF AsNoTracking()
### DBSet.AsNoTracking()获取非跟踪数据
   AsNoTracking称之为获取不带变动跟踪的实体查询。<br/>
   在一些情况下，我们只需要查询返回一个制度的数据记录，而不会对数据记录进行任何的修改。这种时候不希望Entity Framework进行不必要的状态变动跟踪，可以使用Entity Framework的AsNoTracking方法来查询返回不带变动跟踪的查询结果。
### EF实体对象变动跟踪
链接地址：https://blog.csdn.net/u011127019/article/details/53941235
  Entity Framework 通过DbContext.ChangeTracker对实体对象的变动进行跟踪，实现跟踪的方式有两种：变动跟踪快照和变动跟踪代理。
- 变动追踪快照
- 变动追踪代理

# [DbConnection Methods](https://docs.microsoft.com/zh-cn/dotnet/api/system.data.common.dbconnection.openasync?view=netcore-3.1)
# EF Core迁移功能
EF Core迁移功能可以通过使EF更新数据库架构而不是创建新数据库来解决如下问题：
   当数据模型更改时，模型都无法与数据库保持同步。每当更改数据模型之后你都可以删除数据库，EF将创建匹配该模型的新数据库并用测试数据为其设定种子。
<br/><br/>
   但是在实际生产环境中，通常需要存储保留的数据，以便不会再每次更改（如添加新列）时丢失所有数据。而EF Core的数据迁移功能可以解决盖尔问题。
   
## 使用迁移功能
### 工具的安装
在Package Manager console中运行以下命令，安装包管理器控制台工具：
``
Install-Package Microsoft.EntityFrameworkCore.Tools
``
在Package Manager Console中运行以下命令，更新这些工具。
``
Update-Package Microsoft.EntityFrameworkCore.Tools
``
### 验证安装
通过运行以下命令验证是否已安装这些工具：
``
Get-Help about_EntityFrameworkCore
``
### 脚本迁移
以下示例使用迁移名称创建用于 InitialCreate 迁移的脚本。
``
Script-Migration -To InitialCreate
``
以下示例使用迁移 ID，为 InitialCreate 迁移后的所有迁移创建一个脚本。
``
Script-Migration -From 20180904195021_InitialCreate
``
#### Update-Database
将数据库更新到上次迁移或指定迁移。
下面的示例将还原所有迁移。
``
Update-Database -Migration 0
``
下面的示例将数据库更新为指定的迁移。 第一个使用迁移名称，第二个使用迁移 ID 和指定的连接：
``
Update-Database -Migration InitialCreate
Update-Database -Migration 20180904195021_InitialCreate -Connection your_connection_string
``

## 自定义数据模型
### DataType 特性
### DisplayFormat 特性用于显式指定日期格式：
``
[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
``
### StringLength 特性
使用特性指定数据验证规则和验证错误消息。 StringLength 特性设置数据库中的最大长度，并为 ASP.NET Core MVC 提供客户端和服务器端验证。 还可在此属性中指定最小字符串长度，但最小值对数据库架构没有影响。
```
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ContosoUniversity.Models
{
    public class Student
    {
        public int ID { get; set; }
        [StringLength(50)]
        public string LastName { get; set; }
        [StringLength(50)]
        public string FirstMidName { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EnrollmentDate { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; }
    }
}
```
   
