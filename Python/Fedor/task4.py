"""
Вводится число, затем строки слов через двоеточие и пробел.
Из каждой строки выбрать слова, длина которых не кратна числу, и вывести их через запятую и пробел
"""
import sys
c = 0
dig = 0
arr = list()
out = list()
for line in sys.stdin:
    out = list()
    if c == 0:
        dig = int(line)
    else:
        arr = line.split(": ")
        arr[-1] = arr[-1].replace("\n", "")
        for word in arr:
            if len(word) % dig != 0:
                out.append(word)
        print(*out, sep=", ")

    c += 1

