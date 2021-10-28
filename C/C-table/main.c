#include <stdio.h>
#include <malloc.h>
#include <string.h>

typedef struct Item{
    char *info;
    int key1; // опциональное поле
    int key2; // опциональное поле
    int ind1; // опциональное поле
    int ind2; // опциональное поле
    //PointerType1 *p1;
    //PointerType2 *p2;
} Item;

typedef struct KeySpace1{
    // Максимальная вместимость - msize1
    // (key) - уникальный
    // 7.1 - удаление элементов, заданных диапазоном ключей.
    int key;
    Item item;
} KeySpace1;

typedef struct KeySpace2{
    // Максимальная вместимость - msize2
    // Доступ происходит по двойному хэшированию
    // (key, rel) - уникальный
    // 7.2 - поиск всех версий элемента по (key) и (key, rel) -> возвращает новую таблицу
    int busy;
    int key;
    int rel;
    Item item;
} KeySpace2;

typedef struct Table{
    KeySpace1 *ks1;
    KeySpace2 *ks2;
    int msize1; // опциональное поле
    int msize2; // опциональное поле
    int csize1; // опциональное поле
    int csize2; // опциональное поле
} Table;

// 1. Включение нового элемента в таблицу.                        +
// (key1, key2) - уникальные для Table.                           +
// (key1) - уникально для KeySpace1.                              +
// (key2, rel) - уникально для KeySpace2.                         +
// При совпадении key: rel увеличивается на 1 и сохраняется.      +

// 2. Поиск элемента в таблице по (key1, key2).                   +

// 3. Удаление элемента в таблице по (key1, key2).                +

// 4. Поиск по key1/key2.                                         +

// 5. Удаление из таблицы по key1/key2.                           +

// 6. Вывод таблицы на экран.

// 7. Операции пространства ключей:
  // 7.1 - [KeySpace1] удаление элементов, заданных диапазоном ключей.
  // 7.2 - [KeySpace2] поиск всех версий элемента по (key) и (key, rel) -> возвращает новую таблицу

// 8. Вывод ошибок на экран
/*
int _FindInFirstKeyspace(Table table, int el){
    int i = 0;
    for (; i < table.csize1; i++){
        if (table.ks1[i].key == el){
            return i;
        }
    }
    return -1;
}

int _FindInSecondKeyspace(Table table, int el){
    // TODO: Доработать
    int r = -1;
    for (int i = 0; i < table.csize2; i++){
        if (table.ks2[i].busy == 1 & table.ks2[i].key == el){
            r ++;
        }
    }
    return r;
}

int _hash(Table *table, int el){
    el = el % (table->msize2);
    return el;
}

int _InsertToSecondKeyspace(Table *table, int el, char *info){
    int i = hash(table, el);
    int r = 0;
    while (table->ks2[i].busy == 1){
        i = (i + 3) % table->msize2;
    }
    int f = FindInSecondKeyspace(*table, el);
    if (f >= 0){
        r = f + 1;
    }

    table->ks2[i].key = el;
    printf("KeySpace2[%d]=%d\n", i, table->ks2[i].key);
    table->csize2++;
    table->ks2[i].item.info = (char*)malloc(sizeof(char)*(strlen(info)+1));

    memset(table->ks2[i].item.info, "0", sizeof(char)*(strlen(info)+1));
    memcpy(table->ks2[i].item.info, info, strlen(info)*(sizeof(char)+1));
    printf("%s\n", table->ks2[i].item.info);
    table->ks2[i].rel = r;
    table->ks2[i].busy=1;
    return i;
}

int _AddNewElement(Table *table){
    if (table->msize1 == table->csize1){
        printf("Error. KeySpace1 is overflow\n");
        return 2; // Overflow
    }
    if (table->msize2 == table->csize2){
        printf("Error. KeySpace2 is overflow\n");
        return 2; // Overflow
    }

    int k1, k2;
    char *info;
    info = (char*) calloc(20, sizeof(char));
    printf("Input key in table #1:\n");
    scanf("%d", &k1);
    printf("Input key in table #2:\n");
    scanf("%d", &k2);

    if (_FindInFirstKeyspace(*table, k1) >= 0){
        printf("Error. First key found in KeySpace1");
    }

    printf("Input info:\n");
    scanf("%s", info);

    int i2 = _InsertToSecondKeyspace(table, k2, info);


    return 0;
}
*/

void DisplayTable(Table table){
    for (int i=0; i<table.csize1; i++){
        printf("Item %s", table.ks1[i].item.info);
        printf("Indexes: %d, %d", table.ks1[i].item.ind1, table.ks1[i].item.ind2);
        printf("Keys: %d, %d", table.ks1[i].item.key1, table.ks1[i].item.key2);
    }
}

int DeleteByFirstKey(Table *table, int k1){
    for (int i=0; i<table->csize1; i++){
        if (table->ks1[i].key == k1){
            table->ks2[table->ks1[i].item.ind2].key = 0;
            table->ks2[table->ks1[i].item.ind2].item.info = 0;
            table->ks2[table->ks1[i].item.ind2].item.ind1 = 0;
            table->ks2[table->ks1[i].item.ind2].item.ind2 = 0;
            table->ks2[table->ks1[i].item.ind2].item.key1 = 0;
            table->ks2[table->ks1[i].item.ind2].item.key2 = 0;
            table->ks1[i].key = 0;
            table->ks1[i].item.info = 0;
            table->ks1[i].item.ind1 = 0;
            table->ks1[i].item.ind2 = 0;
            table->ks1[i].item.key1 = 0;
            table->ks1[i].item.key2 = 0;
            return 1;
        }
    }
    return -1;
}

int DeleteBySecondKey(Table *table, int k2){
    for (int i=0; i<table->csize2; i++){
        if (table->ks2[i].key == k2){
            table->ks1[table->ks2[i].item.ind1].key = 0;
            table->ks1[table->ks2[i].item.ind1].item.info = 0;
            table->ks1[table->ks2[i].item.ind1].item.ind1 = 0;
            table->ks1[table->ks2[i].item.ind1].item.ind2 = 0;
            table->ks1[table->ks2[i].item.ind1].item.key1 = 0;
            table->ks1[table->ks2[i].item.ind1].item.key2 = 0;
            table->ks2[i].key = 0;
            table->ks2[i].busy = 0;
            table->ks2[i].rel = 0;
            table->ks2[i].item.info = 0;
            table->ks2[i].item.ind1 = 0;
            table->ks2[i].item.ind2 = 0;
            table->ks2[i].item.key1 = 0;
            table->ks2[i].item.key2 = 0;
            return 1;
        }
    }
    return -1;
}

int DeleteByBothKeys(Table *table, int k1, int k2){
    for (int i=0; i<table->csize1; i++){
        if (table->ks1[i].key == k1 && table->ks2[table->ks1[i].item.ind2].key == k2){
            table->ks2[table->ks1[i].item.ind2].key = 0;
            table->ks2[table->ks1[i].item.ind2].rel = 0;
            table->ks2[table->ks1[i].item.ind2].busy = 0;
            table->ks2[table->ks1[i].item.ind2].item.info = 0;
            table->ks2[table->ks1[i].item.ind2].item.ind1 = 0;
            table->ks2[table->ks1[i].item.ind2].item.ind2 = 0;
            table->ks2[table->ks1[i].item.ind2].item.key1 = 0;
            table->ks2[table->ks1[i].item.ind2].item.key2 = 0;
            table->ks1[i].key = 0;
            table->ks1[i].item.info = 0;
            table->ks1[i].item.ind1 = 0;
            table->ks1[i].item.ind2 = 0;
            table->ks1[i].item.key1 = 0;
            table->ks1[i].item.key2 = 0;
            return 1;
        }
    }
    return -1;
}

Item FindBySecondKey(Table table, int k2){
    for (int i=0; i<table.csize1; i++){
        if (table.ks2[i].key == k2){
            return table.ks2[i].item;
        }
    }
}

Item FindByFirstKey(Table table, int k1){
    for (int i=0; i<table.csize1; i++){
        if (table.ks1[i].key == k1){
            return table.ks1[i].item;
        }
    }
}

Item FindByBothKeys(Table table, int k1, int k2){
    for (int i=0; i<table.csize1; i++){
        if (table.ks1[i].key == k1 && table.ks2[table.ks1[i].item.ind2].key == k2){
            return table.ks1[i].item;
        }
    }
}

int FindKeyInFirstKeyspace(Table table, int el){
    for(int i = 0; i <= table.csize1; i++){
        if (table.ks1[i].key == el){
            return i;
        }
    }
    return -1;
}

int AddNewElement(Table *table){
    if (table->msize1 == table->csize1){
        printf("Error. KeySpace1 is overflow\n");
        return 2; // Overflow
    }
    if (table->msize2 == table->csize2){
        printf("Error. KeySpace2 is overflow\n");
        return 2; // Overflow
    }

    int k1, k2;
    char *info;

    printf("Input key in table #1:\n");
    scanf("%d", &k1);
    printf("Input key in table #2:\n");
    scanf("%d", &k2);
    printf("Input info (ONLY CHAR):\n");
    scanf("%s", info);



    if (FindKeyInFirstKeyspace(*table, k1) >= 0){
        printf("Error. Key found in keyspace1");
        return -1;
    }

    // Добавление в первый кейспейс
    table->ks1[table->csize1].key = k1;
    table->ks1[table->csize1].item.key1 = k1;
    table->ks1[table->csize1].item.ind1 = table->csize1;

    table->ks1[table->csize1].item.info = (char*)malloc(sizeof(char)*strlen(info)+1);
    memset(table->ks1[table->csize1].item.info, 'Z', sizeof(char)*strlen(info));
    table->csize1++; // Добавили -> увеличим текущую заполненность

    // Перебор второго кейспейса для нахождения rel
    int max_rel = 1;
    for (int i=0; i <= table->csize2; i++){
        if (table->ks2[i].key == k2){
            if (table->ks2[i].rel > max_rel){
                max_rel = table->ks2[i].rel+1;
            }
        }
    }
    // Добавление во второй кейспейс. Айтем сразу знает инфу о k1,k2,ind1,ind2
    table->ks2[table->csize2].key = k2;
    table->ks2[table->csize2].rel = max_rel;
    table->ks2[table->csize2].busy = 1;
    table->ks2[table->csize2].item.key1 = k1;
    table->ks2[table->csize2].item.ind1 = table->csize1-1; // Потому что csize1 уже был увеличен
    table->ks2[table->csize2].item.key2 = k2;
    table->ks2[table->csize2].item.ind2 = table->csize2;
    table->ks2[table->csize2].item.info = (char*)malloc(sizeof(char)*strlen(info)+1);
    memset(table->ks2[table->csize2].item.info, 'Z', sizeof(char)*strlen(info));
    table->csize2++;

    // Добавим инфу о k2, ind2 айтема в первом кейсете
    table->ks1[table->csize1-1].item.key2 = k2;
    table->ks1[table->csize1-1].item.ind2 = table->csize2-1; // Потому что csize2 уже был увеличен


    return 1;
}

Table CreateNewTable(){
    // Создание новой таблицы с размером 10.
    Table newTable = {NULL, NULL, 10, 10, 0,0};
    newTable.ks1 = (void*)calloc(newTable.msize1, sizeof(int));
    newTable.ks2 = (void*)calloc(newTable.msize2, sizeof(int));
    return newTable;
}

int menu (Table *table){
    int m, k1,k2;
    Item item;
    printf("Input - 1\nFind - 2.[By both keys]  3.[By 1st key]  4.[By 2nd key]"
           "\nDelete - 5.[By both keys]  6.[By 1st key]  7.[By 2nd key]\n0.Quit\n");
    scanf("%d", &m);
    while (m!=0) {
        switch (m) {
            case 1:
                AddNewElement(table);
                break;
            case 2:
                printf("write k1");
                scanf("%d", &k1);
                printf("write k2");
                scanf("%d", &k2);
                item = FindByBothKeys(*table, k1, k2);
                printf(item.info);
                break;
            case 3:
                printf("write k1");
                scanf("%d", &k1);
                item = FindByFirstKey(*table, k1);
                printf(item.info);
                break;
            case 4:
                printf("write k2");
                scanf("%d", &k2);
                item = FindBySecondKey(*table, k2);
                printf(item.info);
                break;
            case 5:
                printf("write k1");
                scanf("%d", &k1);
                printf("write k2");
                scanf("%d", &k2);
                DeleteByBothKeys(table, k1,k2);
                break;
            case 6:
                printf("write k1");
                scanf("%d", &k1);
                DeleteByFirstKey(table, k1);
                break;
            case 7:
                printf("write k2\n");
                scanf("%d", &k2);
                DeleteBySecondKey(table, k2);
                break;
        }
        printf("Input - 1\nFind - 2.[By both keys]  3.[By 1st key]  4.[By 2nd key]"
               "\nDelete - 5.[By both keys]  6.[By 1st key]  7.[By 2nd key]\n0.Quit\n");
        scanf("%d", &m);
    }
}

int main() {
    Table MyTable = CreateNewTable();
    menu(&MyTable);
    return 0;
}
