# objnull

> 象空 - 开源Github用户社区 [http://objnull.com/](http://objnull.com/) QQ群：612309214  

## 项目结构

![001](img/001.jpg)
* **MVCWeb.Redis**  
Redis缓存项目，提供Redis的访问。Redis用于存储经常访问的权限控制数据，消息通知，有时效性的临时数据。  
* **MVCWeb.Model**  
EntityFramework数据模型定义项目，包括了模型定义类和DBContext的定义和设置。
* **MVCWeb.DataSvc**  
数据CURD服务层，包括一个基础操作接口和实现和其他模型类的具体实现。
* **MVCWeb**  
具体网站项目。  

## 项目运行注意事项

1. **数据库**  
修改Web.config中连接字符串对应你本地计算机数据库，推荐使用MySQL。  
在**MVCWeb.Model**项目中**DBContext**文件夹修改类`MyDBContext`，切换注释设置数据库初始化器。  
![005](img/005.jpg)  
直接运行就会生成对应数据库，不要手动建立。

2. **Redis**  
安装Redis并启动，推荐安装一个新的系统服务，端口和密码设置如下图则不用修改代码。  
在**MVCWeb.Redis**项目中**Base**文件夹下`MyRedisDBFactory`类修改Redis连接配置。  
![006](img/006.jpg)  

3. **文件存储路径**  
在Web.config中`appSettings`节点下设置Key`BlogFilePath`和`NewBeeFilePath`的值为你想要的路径。

4. **cookie加密用向量和密钥**  
在`E:\ObjNullFile\`添加文件`Config.txt`，文件写两行文本，第一行16个随机字符作为向量，第二行32个随机字符作为密钥，不要是汉字。  
`E:\ObjNullFile\`实际来自Web.config中配置`NewBeeFilePath`项的路径，有问题在`Global.asax`文件中查看文件读取代码。

5. **权限控制**  
第一次运行注释**MVCWeb**项目**App_Start**文件夹**Filters.cs**文件中以下代码关闭权限控制  
![007](img/007.jpg)  
运行成功后，访问`http://localhost:2221/Manager/Manage`页面进行后台管理。点击`一键生成ActionRule`后就可以取消注释开启权限控制。


![haha](img/haha.png)
