# <center> 软件开发说明 #

### 设备列表 list，通过以太网连接到分区监控电脑A,B,C 电脑作为client ###
* 按照设备列表所对应的ip 端口建立连接列表
* client 依次循环查询设备获取设备状态，Delaysetting 延迟时间内循环查询：按照在该循环内，若某一设备状态n次查询中一次为正常则设备运行正常，正常：3，阻塞：2，警告：1，不工作：0
* 获取状态数组[]转换成字符串发送给服务器

* 在Delaysetting 时间完成后向监控中心电脑Server 发送一次数据
* server 端刷新页面，插入数据库


### 修改方案 ###
* 以车间为单位上传数据：车间号，时间戳
* 稼动率等，PC端完成后上传，时间戳
* 2019-01-13

### 监控中心 ###
* 其他设置


### 修改 2019年1月27日14:07:35 ###
* 不亮时间算入黄灯时间
* 网页不完全
* 短信设置
