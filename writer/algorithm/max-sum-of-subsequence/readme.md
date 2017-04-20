
---
#### 最大子序列和

>求取数组中最大连续子序列和，例如给定数组为A={1， 3， -2， 4， -5}， 则最大连续子序列和为6，即1+3+（-2）+ 4 = 6。

本来只是认为这是一个面试题，前两天做leetcode的时候突然发现它的一个变种是具有实际意义的，具体就是[Best Time to Buy and Sell Stock](https://leetcode.com/problems/best-time-to-buy-and-sell-stock/) 问题。

---
#### 分治算法

值得注意的是，考虑分治算法的时候经常会用到一个事实：**当要求的值m跨分治边界的时候，左边的子向量会用到右边界，而右边的子向量会用到左边界**，这听起来理所当然又很傻，然而这才是解决问题的关键。

```
def do_max_sum (data, _begin, _end)
    return data[_begin] if _end - _begin == 1

    mid = (_begin + _end)/2
    lmax = do_max_sum(data, _begin, mid)
    rmax = do_max_sum(data, mid, _end)
    result = lmax > rmax ? lmax : rmax

    lmax, temp = 0, 0
    (mid-1).downto(0) do |i|
        temp += data[i]
        lmax = temp if lmax < temp
    end

    rmax, temp = 0, 0
    mid.upto(_end-1) do |i|
        temp += data[i]
        rmax = temp if rmax < temp
    end

    cross_value = lmax + rmax
    result = cross_value if result < cross_value
    result
end

def max_sum (data)
    return 0 if !data or data.length <= 0
    do_max_sum(data, 0, data.length)
end

```

---
#### 扫描算法

```
// 这个算法在数组值全负的情况下会返回最大的一个负数
int maxSum (int data[], int length)
{
    if (NULL == data || length <= 0)
    {
        return 0;
    }

    int maxSoFar = data[0];
    int maxEndingHere = 0;
    for (int i= 1; i< length; ++i)
    {
        maxEndingHere += data[i];
        if (maxEndingHere > maxSoFar)
        {
            maxSoFar = maxEndingHere;
        }
        else if (maxEndingHere < 0)
        {
            maxEndingHere = 0;
        }
    }

    return maxSoFar;
}
```

---
#### refercence
1. 《编程珠玑 第二版 》第8章 算法设计艺术， P77
2. [最大连续子序列和](http://blog.csdn.net/sgbfblog/article/details/8032464)
3. [Best Time to Buy and Sell Stock] (https://leetcode.com/problems/best-time-to-buy-and-sell-stock/)


