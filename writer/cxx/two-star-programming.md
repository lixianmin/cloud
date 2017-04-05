
----
#### [two star programming](http://wordaligned.org/articles/two-star-programming)

双指针问题，最近在刷leetcode，发现不少链表的题使用dummy head来解决，在涉及到head节点调整删除时可以简化代码判断逻辑。

dummy head的缺点是需要在stack上生成ListNode对象，在C语言中没问题，但在类java语言中，因为类对象必须使用new生成，这会导致一些GC问题。

我感觉这是一个不错的技巧，像发现了新大陆一样。然后，我看到了[Linus说](https://meta.slashdot.org/story/12/10/11/0030249/linus-torvalds-answers-your-questions)：

>At the opposite end of the spectrum, I actually wish more people understood the really core low-level kind of coding. Not big, complex stuff like the lockless name lookup, but simply good use of pointers-to-pointers etc. For example, I’ve seen too many people who delete a singly-linked list entry by keeping track of the prev entry, and then to delete the entry, doing something like

    if (prev)
        prev->next = entry->next;
    else
        list_head = entry->next;
>and whenever I see code like that, I just go “This person doesn’t understand pointers”. And it’s sadly quite common.

>People who understand pointers just use a “pointer to the entry pointer”, and initialize that with the address of the list_head. And then as they traverse the list, they can remove the entry without using any conditionals, by just doing a *pp = entry->next.

好吧，说得就是我这种人，看来双指针是一个更好的技巧。然而，看了一段时间代码后发现了一件恐怖的事情：双指针似乎很难直观理解，于是我不断地琢磨啊琢磨，最终才发现：


### 函数是不能直接修改指针本身的，只能修改指针指向的对象

因此，如果需要在函数中修改指针本身，就需要使用二级指针（或一级指针的引用），常用于内存分配、删除链表节点等操作。

```
void remove (ListNode **head, int val)
{
    if (!head)
    {
        return;
    }
    
    for (ListNode **curr = head; *curr; )
    {
        ListNode *entry = *curr;
        if (entry->value == val)
        {
            // 因为curr是二级指针，因此*curr就是一级指针，可以通过*curr修改一级指针本身
            *curr = entry->next;
            delete entry;
        }
        else
        {
            // 指针操作->的优先级大于&操作
            curr = &entry->next;
        }
    }
}
```

----
#### 于是为了练习双指针把前两天做的leetcode上相关的习题都改了一遍

#   |Problem    |Solution   |Difficulty	
--- |---        |---        |---
|2	    | [Add Two Numbers](https://leetcode.com/problems/add-two-numbers) | [C++](https://github.com/lixianmin/leetcode/tree/master/algorithms/add-two-numbers) | Medium |
|19     | [Remove Nth Node From End of List](https://github.com/lixianmin/leetcode/tree/master/algorithms/remove-nth-node-from-end-of-list) | [C++](https://github.com/lixianmin/leetcode/tree/master/algorithms/remove-nth-node-from-end-of-list) | Medium |
|23	  | [Merge k Sorted Lists](https://leetcode.com/problems/merge-k-sorted-lists/)  | [C++](https://github.com/lixianmin/leetcode/tree/master/algorithms/merge-k-sorted-lists) | Hard |
|24	  | [Swap Nodes in Pairs](https://leetcode.com/problems/swap-nodes-in-pairs/) | [C++](https://github.com/lixianmin/leetcode/tree/master/algorithms/swap-nodes-in-pairs) | Medium |
