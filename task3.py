"""
Напишите класс NaughtyChild, экземпляр которого инициализируется с аргументами: имя, громкость, крик. Класс должен обеспечивать функциональность(nc-экземпляр класса): +
change_cry(line)-поменять крик. Если длина нового крика больше, громкость увеличивается на разность длин, но не больше 100;+
К экземпляру класса можно прибавить число: nc += number, в этом случае громкость меняется на эту величину, но не может стать больше 100 и мнеьше 1;+
Экземпляр класса можно вызвать с аргументом-строкой, возвращается эта строка с большой буквы(остальные маленькие), повторенная столько громкость // 10, но не менее 1; +
_str_ возвращает строку: Naughty Child <имя> has <громкость> loudness +
Экземпляры класса можно сравнивать: сначала по громкости, затем по длинне крика, затем по имени по алфавиту; для этого нужно реализовать методы сравнения: <, >,<=,>=,==,!=.
"""


class NaughtyChild:

    def __init__(self, name, volume, cry):
        self.name = name
        self.volume = int(volume)
        self.cry = cry

    def change_cry(self, line):
        if len(line) > len(self.cry):
            diff = len(line) - len(self.cry)
            self.volume += diff
            if self.volume > 100:
                self.volume = 100

    def __iadd__(self, other):
        self.volume += int(other)
        if self.volume < 0:
            self.volume = 0
        elif self.volume > 100:
            self.volume = 100

    def __str__(self):
        # _str_ возвращает строку: Naughty Child <имя> has <громкость> loudness
        return "Naughty Child " + self.name + " has " + str(self.volume) + " loudness"

    # Громкость, длина, имя по алфавиту
    def __lt__(self, sec):
        " x < y"
        if self.volume < sec.volume:
            return True
        elif len(self.cry) < len(sec.cry):
            return True
        elif self.name < sec.name:
            return True
        return False



    def __le__(self, sec):
        " x <= y"
        if self.volume <= sec.volume:
            return True
        elif len(self.cry) <= len(sec.cry):
            return True
        elif self.name <= sec.name:
            return True
        return False

    def __eq__(self, sec):
        " x == y"
        if self.volume == sec.volume:
            return True
        elif len(self.cry) == len(sec.cry):
            return True
        elif self.name == sec.name:
            return True
        return False

    def __ne__(self, sec):
        " x != y"
        if self.volume != sec.volume:
            return True
        elif len(self.cry) != len(sec.cry):
            return True
        elif self.name != sec.name:
            return True
        return False

    def __gt__(self, sec):
        " x > y"
        if self.volume > sec.volume:
            return True
        elif len(self.cry) > len(sec.cry):
            return True
        elif self.name > sec.name:
            return True
        return False

    def __ge__(self, sec):
        " x >= y"
        if self.volume >= sec.volume:
            return True
        elif len(self.cry) >= len(sec.cry):
            return True
        elif self.name >= sec.name:
            return True
        return False

    def __call__(self, *args, **kwargs):
        new_str = args[0][0].upper() + args[0][1:]
        if self.volume < 10:
            return new_str
        else:
            return new_str * (self.volume//10)
