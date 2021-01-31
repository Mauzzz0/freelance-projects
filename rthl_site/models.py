from django.db import models
from datetime import datetime

class Team(models.Model):
    """Команда"""
    name = models.CharField("Название", max_length=50, unique=True)

    def __str__(self):
        return self.name

    class Meta:
        verbose_name = "Команда"
        verbose_name_plural = "Команды"

class Player(models.Model):
    """Игрок"""
    name = models.CharField("Имя", max_length=50)
    surname = models.CharField("Фамилия", max_length=50)
    patronymic = models.CharField("Отчество", max_length=50)
    game_number = models.PositiveSmallIntegerField("Номер игрока")
    role = models.CharField("Роль",max_length=20,
                            choices=(
                                ("Нападающий", "Нападающий"),
                                ("Защитник", "Защитник"),
                                ("Тренер", "Тренер"))
                            )
    team_name = models.ManyToManyField(
        Team,
        verbose_name="Название команды",
        related_name="player_team"
    )
    image = models.ImageField("Фото", upload_to="img/players",default="img/player.jpg")
    growth = models.PositiveSmallIntegerField("Рост")
    weight = models.PositiveSmallIntegerField("Вес")
    birth_date = models.DateField("Дата рождения")
    grip = models.CharField("Хват",max_length=6,
                            choices=(
                                ("Л","Левый"),
                                ("П","Правый"))
                            )
    qualification = models.CharField("Квалификация",max_length=20,default="Нет")
    games_count = models.PositiveSmallIntegerField("Кол-во игр")
    goals_count = models.PositiveSmallIntegerField("Кол-во голов")
    passes_count = models.PositiveSmallIntegerField("Кол-во передач")
    penalties_count = models.PositiveSmallIntegerField("Кол-во штрафов")
    disqualified_games_count = models.PositiveSmallIntegerField("Кол-во дисквал игр")
    biography = models.TextField("Биография", blank=True, default="Нет")

    def __str__(self):
        return self.name + " " + self.surname + " " + self.patronymic

    def fullname(self):
        return self.surname + " " + self.name + " " + self.patronymic

    class Meta:
        verbose_name = "Игрок"
        verbose_name_plural = "Игроки"