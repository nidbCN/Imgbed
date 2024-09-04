# 图像缩放

## 规格

## 参考

非常小格式的短边像素为 `2,3,4,5,6,9,10,135` 的最小公倍数 `540`。

| 尺寸 | 简写 | 短边分辨率 |
| ---- | ---- | ---------- |
| 极小 | xs   | 540        |
| 中小 | sm   | 1080       |
| 中等 | m    | 1620       |
| 大   | l    | 2160       |
| 极大 |      |            |

写到这发现想法好像不太对，可能白算了，明天想想再说吧。

### 纳入考虑的比例列表

#### 摄影

这部分以常见的专业数码相机传感器画幅与胶片相机的胶卷/底片为参考。

* 1:1 - 120 6X6 胶卷相机
* 3:2 - 135mm 胶卷/全画幅数码相机、APS-C/APS-H 数码相机、120 6X9 胶卷相机
* 4:3 - 120 645/6X8 胶卷相机、早期小尺寸传感器相机、M4/3画幅数码相机
* 5:3 - Surper 16mm 胶卷相机
* 5:4 - 120 6X7 胶卷相机、4X5/8X10 大画幅相机
* 7:5 - 5X7 大画幅相机
* 16:9 - 索尼黑卡系列

#### PC显示

* 4:3
* 16:9
* 16:10
* 21:9
* 32:9

#### 数字电影

考虑图床用户上传可能包含电影截屏等，故统计某网站 BDRip、BlueRay 和 WEB-DL 数字电影资源的分辨率比例以及 ARRI、RED 等常见电影摄影机的输出比例。

摄影胶片本身由于其为模拟信号，不会像数字图像一样因为缩放产生小数像素从而轻微改变比例。并且由于其后期可能的裁剪、变形镜头等使其比例在图床场景下意义不大。

* 8:9 - 2743x3086(ARRI ALEXA 35)
* 6:5 - 3328x2790(ARRI ALEXA 35)
* 4:3 - 35mm 胶片无声、有声电影
* 3:2 - 4608x3164(ARRI ALEXA 35)
* 1.85:1(3027:1636) - 6054x3272(SONY CineAltaV 2)
* 16:9 - 4096x2304、3840x2160、2048x1152、1920x1080
* 17:9(256:135) - 6144x3240、5120x2700、2048x1080(ALL)
* 2:1 - 4096x2048(ARRI ALEXA 35)
* 2.39:1(139:58=2.39655) - 4448x1856(ARRI ALEXA Mini LF)
* 2.39:1(1024:429=2.3869) - 2048x858(ARRI ALEXA Mini)
* 2.39:1(1024:429=) - 4096x1716(SONY CineAltaV 2)
* 2.39:1(432:181=2.3867) - 6048x2534(SONY CineAltaV 2)
* 2.40:1(64:27=2.370) - 6144x2592(RED KOMODO 6K)、8192x3456(RED RAPTOR 8K)

然后我意识到拍摄和输出似乎完全就是两个东西了，以上这些行业主流的比例可谓是奇奇怪怪，但是输出的把 `2.4:1` 变成了 `12:5` 就显得非常正常了。`2.35:1` 由历史上的宽荧幕发展而来，到现在一般是 `2.39:1` 或 `2.40:1` 不同厂家的叫法也不一样，ARRI两台机器的 `2.39:1` 比例也不同。

不过 `17:9` 这个比例倒是索尼和 ARRI 都是 `256:135`。

因此我还是觉得统计观众所能在自己设备上看到的影片的比例作为参考。

##### 统计数据

以下统计了某个网站，电影做种数前25、电影完成数前25、剧集做种数前10、纪录片做种数前10四个数据其资源的片名、片源、分辨率以及标称和实际比例。

**某网站电影分类做种数前25的分辨率及比例**

| 序号 | 片名                       | 片源    | 分辨率    | 标称比例 | 实际比例         |
| ---- | -------------------------- | ------- | --------- | -------- | ---------------- |
| 1    | 周處除三害                 | WEB-DL  | 1920x1080 | 16:9     | 16:9(1.778:1)    |
| 2    | Oppenheimer                | BlueRay | 1920x1080 | 1.78:1   | [2]16:9          |
| 3    | Despicable Me 4            | WEB-DL  | 1920x1034 | 1.85:1   | 960:517(1.857)   |
| 4    | Harry Potter Collection    | BlueRay | 3840x1600 | 2.40:1   | 12:5(2.4:1)      |
| 5    | 一一                       | BlueRay | 1920x1038 | 1.85:1   | 320:173(1.849:1) |
| 6    | Dune                       | BlueRay | 1920x800  | 2.40:1   | [2]12:5          |
| 7    | A Perfect World            | BlueRay | 1920x800  | 2.40:1   | [3]12:5          |
| 8    | Inside Out 2               | WEB-DL  | 1918x802  | 2.40:1   | 959:401(2.392:1) |
| 9    | 首尔之春                   | WEB-DL  | 1920x1080 | -        | [3]16:9          |
| 10   | 长安三万里                 | WEB-DL  | 3840x1640 | 2.35:1   | 96:41(2.341:1)   |
| 11   | Forrest Gump               | BlueRay | 1920x800  | 1.778:1  | [4]12:5          |
| 12   | 隐入尘烟                   | WEB-DL  | 1672x1080 | -        | 209:135(1.548:1) |
| 13   | 让子弹飞                   | BlueRay | 1920x1080 | 16:9     | [4]16:9          |
| 14   | 你好，李焕英               | WEB-DL  | 1920x796  | 2.40:1   | 480:199(2.41:1)  |
| 15   | 關於我和鬼變成家人的那件事 | WEB-DL  | 1920x1080 | 1.778    | [5]16:9          |
| 16   | 年会不能停！               | WEB-DL  | 3840x1608 | 2.40:1   | 160:67(2.388:1)  |
| 17   | Slumdog Millionaire        | BlueRay | 1920x1080 | 16:9     | [6]16:9          |
| 18   | 名探偵コナン 黒鉄の魚影    | BlueRay | 1920x1080 | 1.778    | [7]16:9          |
| 19   | 走走停停                   | WEB-DL  | 3840x1608 | 2.40:1   | [2]160:67        |
| 20   | Maharaja                   | WEB-DL  | 1920x1080 | 16:9     | [8]16:9          |
| 21   | Oppenheimer                | BlueRay | 3840x2160 | 1.78:1   | [9]16:9          |
| 22   | 第二十条                   | WEB-DL  | 1920x800  | -        | [5]12:5          |
| 23   | 宇宙探索编辑部             | WEB-DL  | 3840:2160 | 16:9     | [10]16:9         |
| 24   | 花束みたいな恋をした       | BlueRay | 1920:1038 | 1.85:1   | [2]320:173       |
| 25   | スラムダンク               | BlueRay | 3840x2160 | 16:9     | [11]16:9         |

做种数较多的通常是当前热门或质量较高（保种人较多）因此制作的分辨率一般较为常见，其中 `16:9` 共 11 部，`12:5（即2.4:1）` 共 5 部，其余 `320:173` 2 部，`160:67` 2 部，他们都是比例接近 `1.85:1` 的。

**某网站电影分类完成数前25的分辨率及比例**

注：序号内带有括号的为之前做种数中已经统计过的，不再计入

| 序号   | 片名                              | 片源    | 分辨率    | 标称比例 | 实际比例          |
| ------ | --------------------------------- | ------- | --------- | -------- | ----------------- |
| 1(1)   | 周處除三害                        | WEB-DL  | 1920x1080 | 16:9     | 16:9              |
| 2      | Seeking Justice                   | BlueRay | 1280x546  | 2.35:1   | 640:273(2.344:1)  |
| 3(14)  | 你好，李焕英                      | WEB-DL  | 1920x796  | 2.40:1   | 480:199(2.41:1)   |
| 4      | 这个杀手不太冷静                  | WEB-DL  | 1920x800  | -        | 12:5(2.4:1)       |
| 5      | 扬名立万                          | WEB-DL  | 1920x808  | 1.376:1  | 240:101(2.376:1)  |
| 6      | 让子弹飞                          | DVDRip  | 672x272   | -        | 42:17(2.47:1)     |
| 7      | 流浪地球                          | WEB-DL  | 3840x1728 | 16:9     | 20:9(2.222:1)     |
| 8(12)  | 隐入尘烟                          | WEB-DL  | 1672x1080 | -        | 209:135           |
| 9      | 白鹿原                            | HDTV    |           |          |                   |
| 10     | Spider-Man No Way Home            | BlueRay | 1920x800  | 2.40:1   | [2]12:5           |
| 11     | 铜雀台                            | HDTV    | 1280x536  | 2.38:1   | 160:67(2.388:1)   |
| 12     | 我和我的家乡                      | WEB-DL  | 2048x858  | 2.40:1   | 1024:429(2.387:1) |
| 13     | Rise of the Planet of the Apes    | BlueRay | 1280x544  | -        | 40:17(2.353:1)    |
| 14     | 疯狂的外星人                      | WEB-DL  | 1920x750  | 16:9     | 64:25(2.56:1)     |
| 15     | 边境风云                          | HDTV    | 1280x544  | -        | [2]40:17          |
| 16     | 杀生                              | HDTV    | 1280x582  | 2.2:1    | 640:291(2.199:1)  |
| 17     | Everything Everywhere All at Once | BlueRay | 1920x1036 | 1.85:1   | 480:259(1.853:1)  |
| 18     | 我不是药神                        | WEB-DL  | 3840x2072 | 1.85:1   | [2]480:259        |
| 19     | 失恋33天                          | DVDRip  |           |          |                   |
| 20     | 一出好戏                          | WEB-DL  | 3840x1608 | 16:9     | [2]160:67         |
| 21     | 乘风破浪                          | WEB-DL  | 1920x1040 | 16:9     | 24:13(1.846:1)    |
| 22     | 俄囧                              | WEB-DL  | 1920x1080 | 16:9     | 16:9              |
| 23     | 消失的子弹                        | HDTV    | 1280x532  | 2.40:1   | 320:133(2.406:1)  |
| 24(15) | 關於我和鬼變成家人的那件事        | WEB-DL  | 1920x1080 | 1.778    | 16:9              |
| 25     | 夺冠                              | WEB-DL  | 1920x1000 | 16:9     | 48:25             |

由于制作的年代、压制组不同，分辨率的种类非常多而且几乎没有重叠，都不是特别多见的。`12:5` 、`40:17` 与 `160:67` 、 `480:259` 都各有 2 部，而 `16:9` 居然只有 1 部，其余比例为 `640:273` `240:101` `42:17` `20:9` 等。

**某网站剧集分类做种数前10的分辨率及比例**

| 序号 | 片名                    | 片源    | 分辨率    | 标称比例 | 实际比例         |
| ---- | ----------------------- | ------- | --------- | -------- | ---------------- |
| 1    | Game of Thrones         | BlueRay | 1920x1080 | 1.778:1  | 16:9             |
| 2    | 3 Body Problem          | WEB-DL  | 1920x1080 | 16:9     | [2]16:9          |
| 3    | 唐朝诡事录之西行        | WEB-DL  | 1920x800  | -        | 12:5             |
| 4    | 漫长的季节              | WEB-DL  | 3840x1608 | 2.40:1   | 160:67           |
| 5    | The Big Bang Theory     | BlueRay | 1920x1080 | 1.778:1  | [3]16:9          |
| 6    | 狂飙                    | WEB-DL  | 1920x1080 | -        | [4]16:9          |
| 7    | Friends Season 1-10     | BlueRay | 1920x1080 | 16:9     | [5]16:9          |
| 8    | 三体                    | WEB-DL  | 3840x1634 | -        | 1920:817(2.35:1) |
| 9    | Breaking Bad Season 1-5 | BlueRay | 3840x2160 | 1.778    | [6]16:9          |
| 10   | 四大名著（央视版）      | DVDRip  | 704x528   | 4:3      | 4:3              |

剧集中 `16:9` 占据了半壁江山，除此之外还有一些国产影视剧采用了 `2.35:1` 与 `2.40:1` 的比例，因此出现了之前出现过的在 `2.4:1` 附近的 `12:9`、`160:67`，以及央视版老电视剧的 `4:3`。

**某网站纪录片分类做种数前10的分辨率及比例**

| 序号 | 片名                        | 片源    | 分辨率    | 标称比例 | 实际比例       |
| ---- | --------------------------- | ------- | --------- | -------- | -------------- |
| 1    | 中国通史                    | HDTV    | 1280x720  | 16:9     | 16:9           |
| 2    | Taylor Swift: The Eras Tour | WEB-DL  | 3840x1600 | 2.40:1   | 12:5           |
| 3    | The Green Planet            | BlueRay | 3840x2160 | 1.78:1   | [2]16:9        |
| 4    | BBC: Africa                 | BlueRay | 1920x1080 | 16:9     | [3]16:9        |
| 5    | The Universe                | BlueRay | 1920x1080 | 16:9     | [4]16:9        |
| 6    | Taylor Swift: The Eras Tour | WEB-DL  | 3840x1600 | 2.40:1   | [2]12:5        |
| 7    | The Blue Planet             | BlueRay | 1920x1080 | 1.78:1   | [5]16:9        |
| 8    | Blue Planet II              | BlueRay | 3840x2160 | 16:9     | [6]16:9        |
| 9    | The Miracle of Love         | DVDRip  | 708x369   | 16:9     | 236:123(1.919) |
| 10   | The World at War            | BlueRay | 1920x1080 | 1.78:1   | [7]16:9        |

纪录片中 `16:9` 更是占了 70%，`12:5` 占了 20%。

因此，以上统计的视频中，绝大多数为 `16:9` 与 `12:5` ，除此之外，`160:67` 最多为 5 部，`209:135`、`480:199`、`40:17`、`480:259` 均为 2 部。

处于兼容考虑，短边像素应为比例的分母最小公倍数的整数倍，对于兼容电影这部分来说，是 `9`、`5`、`67`、`17`、`135`，同样拥有 2 部的 `199` 与 `259` 加入后过大且数字疑似来自于压制时候缩放差了1个像素。

这些数字的最小公倍数是 `153765`，明显大于一般的图片尺寸，因此去除67再做计算，得出结果 `2295`，还是偏大，去除 `135` 结果为 `765`，去除 `17` 结果为 `135`。

#### 手机屏幕

* 20:9 - 2400x1080(Redmi Note 12T/Samsung Galaxy S21 5G)、2800x1260(IQOO NEO 8)、3200x1440(Redmi K60)
* 19.5:9 - ?:?(iPhone X - iPhone 14)
* 19:9 - 3040x1440(Samsung Galaxy S20 5G)
* 16:9
* 11:5 - 2376x1080(HUAWEI MATE 40)
* 13:6 - 2340x1080(HUAWEI P40/Xiaomi 9)
