

---
#### 基础理论

当我们无法基于规则进行判断时， 我们使用概率。

##### [Probability vs. Statistics](http://stats.stackexchange.com/questions/665/whats-the-difference-between-probability-and-statistics/)

在某种程度上，概率论和统计学的目的是完全相反(inverse)的：

1.  In probability theory we consider some underlying process which has some randomness or uncertainty modeled by random variables, and we figure out what happens. 在概率论中，我们是基于已有的理论模型，推断未知事件发生的概率。

2. In statistics we observe something that has happened, and try to figure out what underlying process would explain those observations.在统计学中，我们观察数据，并推断什么样的理论模型可以解释我们观察到的数据。

Bayes是用于推理的，而**推理讲究证据**，因此如果非要归类的话，Bayes会属于统计学范畴而不是概率论。

##### [The diachronic interpretation](http://www.greenteapress.com/thinkbayes/html/thinkbayes002.html)

在很多书中使用字母A、B表示事件，使用P(A|B)表示条件概率，这相对太抽象。我们使用另外一套字母体系：H和E(D)，其中H= hypothesis，E= evidence（或D=data）。这样Bayes的推理过程可以表述为：通过不断的收集证据E来强化对假设事件H的信心。

这种表述方法称为diachronic interpretation，其中diachronic是“随时间变化”的意思。在Bayes理论中，就是指每当我们收集到一个新的证据之后，都可以加入到原有Bayes系统中用于调整对原有事件的看法（可能是增删改 + - x），因此事件H的概率会不断调整。

Bayes定理公式如下：

$$
P(H|E) = \frac{P(H) \cdot P(E|H)}{P(E)}
$$

公式中的每一项都有一个单独的名字：

1. P(H) ⇒ **先验概率(prior probability)**，又叫基础概率，是无任何条件限制下事件H发生的概率

2. P(H|E) ⇒ **后验概率(posterior probability)**

3. P(E|H) ⇒ **条件似然(conditional likelihood)**， 有时候我自己称之为似然概率：
	-  **_物以类聚，人以群分_**，如果我们把H与~H看作两类人，比如男人和女人，那么**这两类人针对同一件事情会有不同的看法和倾向**，比如男人可能更喜欢踢足球，而女人可能更喜欢逛街，似然概率描述的就是这两类不同的人针对事件$E_i$表现出的倾向概率
	- 由于P(E|H)与P(E|~H)是针对两类不同的人的概率，因此它们之间并不互斥， **P(E|H) + P(E|~H) ≠ 1**

4. P(E) ⇒ 在所有情况下证据E发生的概率，不管事件H发生还是不发生，称为**整体似然(total likelihood)**，因为它起到归一化的作用，所以又称为归一化常量(normalizing constant)

##### 性质分析

在Bayes推理过程中，可以不断加入新证据到Bayes系统中，当各证据$E_i$相互独立时，可以得到如下朴素Bayes分类器速算公式：

 $$
 P(H|E_1 E_2 \cdots) = \frac{ \frac{P(H)}{P(\overline H)} \cdot \frac{P(E_1|H)}{P(E_1|\overline H)} \cdot \frac{P(E_2|H)}{P(E_2|\overline H)} \times \cdots }{ \frac{P(H)}{P(\overline H)} \cdot \frac{P(E_1|H)}{P(E_1|\overline H)} \cdot \frac{P(E_2|H)}{P(E_2|\overline H)} \times \cdots  + 1} 
 $$

整个计算过程可以解读为：

 - Posterior odds ratio = Prior odds ratio x Likelihood ratio
 - **先验比 x 似然比1 x 似然比2 x ...，然后normalize**
 - 当只存在两种分类目标H与~H时，由于P(H) + P(~H) = 1，因此先验比往往比较容易计算

假定事件E和事件F独立，那么F就不能影响E，于是P(E|F)=P(E)。把P(E|F)展开，就成了P(E∩F)/P(F)=P(E)，或者P(E∩F)=P(E)*P(F)，这不就是“两个独立事件同时发生的概率”的计算公式么。

---
#### 举例

>问题1： 一机器在良好状态生产合格产品几率是90%，在故障状态生产合格产品几率是30%，机器良好的概率是75%，若一日第一件产品是合格品，那么此日机器良好的概率是多少。 

分析：假定事件A代表机器良好，事件B代表某一日生产的是合格品，则目标概率是P(A|B)，而已知条件包括：

1. 先验概率P(A)= 0.75
2. 我们前面所谓的两类人在本题中指的就是A机器良好和~A机器故障，而题目中给出的似然概率就是这两种不同的机器生产产品时的合格率是不同的

因此解题如下：

1. 先验比，PPR = 0.75 : (1-0.75) = 3 : 1
2. 似然比(Likelihood Ratio)，LR = 0.9 : 0.3 = 3
3. 两者相乘，得后验比率 = 9 : 1，然后
4. 标准化（normalize），得后验概率 = 9 / (9+1) = 0.9


>问题1.1：回到原题。若问，假设这个机器第一天不是生产了 1 个零件，而是生产了 3 个零件，而且 3 个都合格（零件合格的概率互相独立），那机器良好的概率是多少？

先验比 x 似然比 =  (0.75/0.25)*(0.9/0.3)^3 = 81/1

归一化结果为 81/(1+81) = 81/82 = 0.987

>问题1.2：假设机器生产了 10 个零件，6 个合格，4 个不合格（各个零件的生产相互独立），机器良好的概率是多少？

先验比 x 合格似然比 x 不合格似然比 = (0.75/0.25) * (0.9/0.3)^6 * (0.1/0.7)^4 = 2187/2401

归一化结果为 2187/(2401+2187) = 2187/4588 = 0.477


>问题2：某个医院早上收了六个门诊病人，如下表

|症状　　| 职业　　　|疾病   |
|---    |---      |---     |
|打喷嚏　| 护士　　　| 感冒  |
|打喷嚏　| 农夫　　　| 过敏  |
|头痛　　| 建筑工人　| 脑震荡 |
|头痛　　| 建筑工人　| 感冒  |
|打喷嚏　| 教师　　　| 感冒  |
|头痛　　| 教师　　　| 脑震荡 |

现在又来了第七个病人，是一个打喷嚏的建筑工人。请问他患上感冒的概率有多大？[[朴素贝叶斯分类器的应用]](http://www.ruanyifeng.com/blog/2013/12/naive_bayes_classifier.html)

1. 先验比 ⇒ 无任何限定条件下普通人得感冒的概率为P(感冒) = 3/6 = 0.5，因此先验比 PPR = 0.5/(1-0.5) = 1
2. 似然概率1 ⇒ 针对打喷嚏这件事情，感冒的人的不感冒的人所表现出概率分别为：P(打喷嚏|感冒)=2/3，P(打喷嚏|不感冒)= 1/3，因此似然比 LR1 = (2/3)/(1/3) = 2
3. 似然概率2 ⇒ 针对建筑工人这件事情，感冒的人与不感冒的人表现出的概率分别为：P(建筑工人|感冒)=1/3，P(建筑工人|不感冒)= 1/3，因此似然比 LR2 = (1/3)/(1/3) = 1
4. 因此 PPR x LR1 x LR2 = 1x2x1 = 2
5. 归一化结果得到P(感冒|打喷嚏x建筑工人) = 2/3 = 0.66

---
#### 先验概率谬误

先验概率的大小会严重影响检测结果，很多时候会反直觉。先验概率数据不一定在每种情况下都存在，但是假如确实有这个数据你却不用，那么，你将毁于先验概率谬误，即忽略事前数据并因此作出错误决策。

下面展示贝叶斯定理在检测吸毒者时的应用。假设一个常规的检测结果的敏感度与可靠度均为99%，即吸毒者每次检测呈阳性（+）的概率为99%。而不吸毒者每次检测呈阴性（-）的概率为99%。从检测结果的概率来看，检测结果是比较准确的，但是贝叶斯定理卻可以揭示一个潜在的问题。假设某公司对全体雇员进行吸毒检测，已知0.5%的雇员吸毒。请问每位检测结果呈阳性的雇员吸毒的概率有多高？

令“D”为雇员吸毒事件，“N”为雇员不吸毒事件，“+”为检测呈阳性事件。可知：

1. P(D)代表雇员吸毒的概率，不考虑其他情况，该值为0.005。因为公司的预先统计表明该公司的雇员中有0.5%的人吸食毒品，所以这个值就是D的先验概率。
2. P(N)代表雇员不吸毒的概率，显然，该值为0.995，也就是1-P(D)。
3. P(+|D)代表吸毒者阳性检出率，这是一个条件概率，由于阳性检测准确性是99%，因此该值为0.99。
4. P(+|N)代表不吸毒者阳性检出率，也就是出错检测的概率，该值为0.01，因为对于不吸毒者，其检测为阴性的概率P(-|N)为99%，因此，其被误检测成阳性的概率为1 - 0.99 = 0.01。
5. P(+)代表不考虑其他因素的影响的阳性检出率。该值为0.0149或者1.49%。我们可以通过全概率公式计算得到：


检测呈阳性的概率  = 吸毒者阳性检出率（0.5% x 99% = 0.495%) + 不吸毒者阳性检出率（99.5% x 1% = 0.995%) = 0.0149

即：

P(+)= P(+,D) + P(+,N) = P(D) x P(+|D) + P(N) x P(+|N)
= 0.5% x 99% + 99.5% x 1%
= 0.0149

根据上述描述，我们可以计算某人检测呈阳性时确实吸毒的条件概率

P(D|+)  = P(D) x P(+|D) / P(+)
= 0.5% x 99% / 0.0149
= 0.3322

1. 吸毒检测的准确率高达99%，直觉上我们会觉得如果一个人检测呈阳性了，他基本上就是已经在吸毒了，但贝叶斯定理告诉我们：如果某人检测呈阳性，其吸毒的概率只有大约33%，不吸毒的可能性比较大。**假阳性高，则检测的结果不可靠，这可能会反直觉**。

2. 贝叶斯定理计算的是条件概率，换句话说，在不知道任何条件之前对每个员工我们认为他吸毒的概率是0.5%，但在检测之后，对于检测结果呈阳性的员工而言，他吸毒的概率变成了33%，是未检测之前的66倍。其实**P(D)与P(D|+)都是描述同一件事情，只不过P(D|+)是在得到某些新证据后计算出的一个更加精确的概率**。在针对该员工的新一轮的验证计算中，P(D|+)将会替代原P(D)的角色参与计算，贝叶斯公式可以通过不断的增加新证据叠加应用，这也是该公式的牛B之处。

---
#### [Bayesian Thinking](https://www.youtube.com/watch?v=BrK7X_XlGB8)
1. Remember you priors
2. Imagine your theory's wrong. Would the world look different?
3. Update incrementally (snowflakes of evidence).

---
#### 参考文献

[怎样用非数学语言讲解贝叶斯定理（Bayes' theorem）](https://www.zhihu.com/question/19725590/answer/32177811#)

[概率论教你说谎：直觉思维的科学解释](http://www.matrix67.com/blog/archives/2517)

[贝叶斯推断及其互联网应用](http://www.ruanyifeng.com/blog/2011/08/bayesian_inference_part_one.html)

[数学之美番外篇：平凡而又神奇的贝叶斯方法](http://mindhacks.cn/2008/09/21/the-magical-bayesian-method/)

[简捷启发式](https://book.douban.com/subject/1599035/)

[How to Write a Spelling Corrector](http://norvig.com/spell-correct.html)

[贝叶斯定理](https://zh.wikipedia.org/zh/%E8%B4%9D%E5%8F%B6%E6%96%AF%E5%AE%9A%E7%90%86)

[似然函数](https://en.wikipedia.org/wiki/Likelihood_function)
