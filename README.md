# Imgbed 图床

嗯，图床

## 特性

* 支持除了 HEIC 之外的几乎全部图片格式
* 按组设置图片的编码、编码参数等
* 上传后自动转码为指定编码
* 自定义水印（待支持）
* IIC色彩空间处理（待支持）

## Roadmap

* ~~完成 webp 解码器~~
* ~~完成解码并发任务~~
* ~~实现 EncoderPool~~
* 完成 ffmpeg filter 相关内容（缩放及水印）
* 完成 Web API
* 完成身份认证与授权
* 编写前端
* 前后端对接
* IIC色彩空间转换（全部转到 sRGB 并嵌入配置文件防止有呆逼程序爆炸）
* patch ffmpeg 支持 heic（不确定）

## Contribution

Feel free to open issues and pull request, ~~because nobody will notice this project~~

## LICENSE

GPL v3, FREE AS IN FREEDOM!
