# objnull  
象空 - 开源Github用户社区 [http://objnull.com/](http://objnull.com/) QQ群：612309214 
![haha](img/haha.png)
## 项目运行注意事项
1. MySQL数据库
修改连接字符串后在MVCWeb.Model.DBContext.`MyDBContext`类中设置数据库初始化器，切换注释`Database.SetInitializer(new MyDBInitializer());`

2. Redis
连接配置在MVCWeb.Redis.Base.`MyRedisDBFactory`类中修改

3. cookie加密用向量和密钥
添加文件`E:\ObjNullFile\Config.txt`，文件路径可在Web.config中修改配置`NewBeeFilePath`项。文件写两行文本，第一行16个随机字符作为向量，第二行32个随机字符作为密钥，不要是汉字。