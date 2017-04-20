#!/usr/bin/ruby
#encoding:utf-8
#
######################################################################
# created:    2017-04-20
# author:     lixianmin#live.cn
#
# Copyright (C) - All Rights Reserved
######################################################################

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

def test (data)
    max = max_sum(data)
    puts "[#{data.join(',')}] => #{max}"
end

def main
    test [1, 3, -2, 4, -5]
    test [31, -41, 59, 26, -53, 58, 97, -93, -23, 84]
end

if __FILE__ == $0
    main
end

