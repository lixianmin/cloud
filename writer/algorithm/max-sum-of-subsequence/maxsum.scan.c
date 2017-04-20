
/********************************************************************
created:    2017-04-20
author:     lixianmin#live.cn

Copyright (C) - All Rights Reserved
*********************************************************************/

#include <assert.h>
#include <stdio.h>
#include <stdlib.h>

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

void test (int data[], int length)
{
    int max = maxSum (data, length);
    printf("max=%d\n", max);
}

int main ()
{
    int data[] = {31, -41, 59, 26, -53, 58, 97, -93, -23, 84};
    test (data, sizeof(data)/sizeof(data[0]));
    return 0;
}
