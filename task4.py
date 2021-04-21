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
    if c == 0:
        dig = int(line)
    elif c == 1:
        arr = line.split(": ")
        break
    c += 1
arr[-1] = arr[-1].replace("\n", "")
for word in arr:
    if len(word) % dig != 0:
        out.append(word)
print(dig)
print(arr)
print(*out,sep=", ")