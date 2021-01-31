from django.db import models

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
    team_name = models.ForeignKey(
        Team, verbose_name="Название команды", on_delete=models.SET_NULL, null=True
    )
    image = models.ImageField("Фото", upload_to="img/players",default="img/player.jpg")

    def __str__(self):
        return self.name + " " + self.surname + " " + self.patronymic

    class Meta:
        verbose_name = "Игрок"
        verbose_name_plural = "Игроки"