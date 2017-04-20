#!/usr/bin/ruby
#encoding:utf-8
#
######################################################################
# created:    2017-04-20
# author:     lixianmin#live.cn
#
# Copyright (C) - All Rights Reserved
######################################################################

# 这个算法在数组值全负的情况下会返回最大的一个负数
def max_sum (data)
    return if !data or data.length <= 0

    maxSoFar, maxEndingHere = 0, 0
    data.each do |item|
        maxEndingHere += item
        if maxEndingHere > maxSoFar
            maxSoFar = maxEndingHere
        elsif maxEndingHere < 0
            maxEndingHere = 0
        end
    end

    maxSoFar;
end

def test (data)
    max = max_sum (data)
    puts "[#{data.join(',')}] => #{max}"
end

def main
    test [1, 3, -2, 4, -5]
    test [31, -41, 59, 26, -53, 58, 97, -93, -23, 84]
end

if __FILE__ == $0
    main
end

