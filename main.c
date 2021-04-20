#include <stdio.h>

typedef struct Item{
    char *info;
    int key1;
    int key2;
    int ind1;
    int ind2;
    //PointerType1 *p1;
    //PointerType2 *p2;
} Item;

typedef struct KeySpace1{
    int key;
    Item item;
} KeySpace1;

typedef struct KeySpace2{
    int busy;
    int key;
    int rel;
    Item item;
} KeySpace2;

typedef struct Table{
    KeySpace1 *ks1;
    KeySpace2 *ks2;
    int msize1;
    int msize2;
    int csize1;
    int csize2;
};

int main() {
    printf("Hello, World!\n");
    return 0;
}
