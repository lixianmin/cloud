
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

在链表类问题中，头节点是否为NULL很可能会导致处理逻辑完全迥异，特别是牵扯到链表结构调整的时候（插入、删除等）。由于dummy head与双指针都可以避免头节点为NULL的问题，使代码用统一的方案书写，因此这两种方案都在链表问题中得到了广泛应用，特别是双指针由于不需要分配一个dummy head，因此代码看起来来会更短更简洁。

比如，针对删除链表的头节点的代码：

```

struct ListNode
{
    int val;
    struct ListNode *next;
};

struct ListNode* list_remove_normal (struct ListNode* head, int val)
{
    if (NULL == head)
    {
        return NULL;
    }

    if (head->val == val)
    {
        struct ListNode* p = head->next;
        free (head);
        return p;
    }

    struct ListNode *p = head->next, *q = head;
    while (NULL != p)
    {
        if (p->val == val)
        {
            q->next = p->next;
            free (p);
            break;
        }

        q = p;
        p = p->next;
    }

    return head;
}

struct ListNode* list_remove_dummy_head (struct ListNode* head, int val)
{
    struct ListNode dummy, *p = head, *q = &dummy;
    dummy.next = head;

    while (NULL != p)
    {
        if (p->val == val)
        {
            q->next = p->next;
            free (p);
            break;
        }

        q = p;
        p = p->next;
    }

    return dummy.next;
}

struct ListNode* list_remove_two_star (struct ListNode *head, int val)
{
    struct ListNode **p = &head;
    while (*p)
    {
        if ((*p)->val == val)
        {
            struct ListNode* kill = *p;
            *p = (*p)->next; // 因为p是二级指针，因此*p就是一级指针，可以通过设置*p的值修改一级指针本身
            free (kill);
            break;
        }

        p = &(*p)->next;     // 指针操作->的优先级大于&操作
    }

    return head;
}

```

----
#### 于是为了练习双指针把前两天做的leetcode上相关的习题都改了一遍

|#   |Problem    |Solution   |Difficulty	
--- |---        |---        |---
|2	    | [Add Two Numbers](https://leetcode.com/problems/add-two-numbers) | [C++](https://github.com/lixianmin/leetcode/tree/master/algorithms/add-two-numbers) | Medium |
|19     | [Remove Nth Node From End of List](https://github.com/lixianmin/leetcode/tree/master/algorithms/remove-nth-node-from-end-of-list) | [C++](https://github.com/lixianmin/leetcode/tree/master/algorithms/remove-nth-node-from-end-of-list) | Medium |
|23	  | [Merge k Sorted Lists](https://leetcode.com/problems/merge-k-sorted-lists/)  | [C++](https://github.com/lixianmin/leetcode/tree/master/algorithms/merge-k-sorted-lists) | Hard |
|24	  | [Swap Nodes in Pairs](https://leetcode.com/problems/swap-nodes-in-pairs/) | [C++](https://github.com/lixianmin/leetcode/tree/master/algorithms/swap-nodes-in-pairs) | Medium |
