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

   
