
#include <assert.h>
#include <stdio.h>
#include <stdlib.h>

//////////////////////////////////////////////////////////////////////////////////
struct ListNode
{
    int val;
    struct ListNode *next;
};

struct ListNode* list_create (int* values, int count)
{
    if (NULL == values || count < 0)
    {
        return NULL;
    }

    struct ListNode *head = NULL, **p= &head;
    for (int i= 0; i< count; ++i)
    {
        struct ListNode *current = (struct ListNode*) malloc (sizeof(struct ListNode));
        current->val = values[i];
        current->next = NULL;

        *p = current;
        p = &current->next;
    }

    return head;
}

void list_destroy (struct ListNode** node)
{
    while (*node)
    {
        struct ListNode* p = *node;
        *node = (*node)->next;
        free (p);
    }
}

void list_print (const char* title, struct ListNode *head)
{
    if (title)
    {
        printf("%-10s", title);
    }

    struct ListNode* p = head;
    while (p)
    {
        printf("%d->", p->val);
        p = p->next;
    }

    puts("\n");
}

//////////////////////////////////////////////////////////////////////////////////

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
            *p = (*p)->next;
            free (kill);
            break;
        }

        p = &(*p)->next;
    }

    return head;
}

int main ()
{
    int a[] = {1, 2, 3, 4, 5, 6, 7};
    struct ListNode* data = list_create(a, sizeof a/sizeof a[0]);

    list_print ("before:", data);

    data = list_remove_normal (data, 2);
    data = list_remove_normal (data, 1);
    data = list_remove_normal (data, 0);

    data = list_remove_dummy_head (data, 4);
    data = list_remove_dummy_head (data, 3);
    data = list_remove_normal (data, 0);

    data = list_remove_two_star(data, 6);
    data = list_remove_two_star(data, 5);
    data = list_remove_normal (data, 0);

    list_print ("after remove:", data);

    list_destroy(&data);
    return 0;
}
