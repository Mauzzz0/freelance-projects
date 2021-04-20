#include <stdio.h>

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
};

// 1. Включение нового элемента в таблицу.
// (key1, key2) - уникальные для Table.
// (key1) - уникально для KeySpace1.
// (key2, rel) - уникально для KeySpace2.
// При совпадении key: rel увеличивается на 1 и сохраняется.

// 2. Поиск элемента в таблице по (key1, key2).

// 3. Удаление элемента в таблице по (key1, key2).

// 4. Поиск по key1/key2.

// 5. Удаление из таблицы по key1/key2.

// 6. Вывод таблицы на экран.

// 7. Операции пространства ключей:
  // 7.1 - [KeySpace1] удаление элементов, заданных диапазоном ключей.
  // 7.2 - [KeySpace2] поиск всех версий элемента по (key) и (key, rel) -> возвращает новую таблицу

// 8. Вывод ошибок на экран

int main() {
    printf("Hello, World!\n");

    return 0;
}
