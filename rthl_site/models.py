from django.db import models
from datetime import date

class Team(models.Model): # TODO: Сделать .url через Slug/SlugField
    """Команда"""   # TODO: Подкрутить в профиле игрока ссылки на команды
    name = models.CharField("Название", max_length=50, unique=True)
    image = models.ImageField("Логотип", upload_to="img/teams",default="img/team.jpg")
    city = models.CharField("Город",max_length=30, default="Москва")
    division = models.CharField("Дивизион", max_length=50,default="Нет",
                                choices=(
                                    ("Золотой","Золотой"),
                                    ("Бронзовый","Бронзовый"),
                                    ("Серебряный","Серебряный"))
                                )
    url = models.SlugField(
        help_text="Адрес на сайте. Пример: rthl.ru/avangard (\"rthl.ru/\" не вводить)",
        max_length=30,
        unique=True)

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
    role = models.CharField("Роль",max_length=20, default="Нет",
                            choices=(
                                ("Нет", "Нет"),
                                ("Вратарь", "Вратарь"),
                                ("Нападающий", "Нападающий"),
                                ("Защитник", "Защитник"),
                                ("Тренер", "Тренер"))
                            )
    adm_role = models.CharField("Административная роль", max_length=20, default="Нет",
                            choices=(
                                ("Нет","Нет"),
                                ("Ассистент", "Ассистент"),
                                ("Капитан", "Капитан"),
                                ("Главный Тренер", "Главный тренер"))
                            )
    team_name = models.ManyToManyField(
        Team,
        verbose_name="Название команды",
        related_name="player_team"
    )
    image = models.ImageField("Фото", upload_to="img/players",default="img/player.jpg")
    growth = models.PositiveSmallIntegerField("Рост",null=True)
    weight = models.PositiveSmallIntegerField("Вес",null=True)
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

    def age(self):
        today = date.today()
        bd = self.birth_date
        return today.year - bd.year - ((today.month,today.day) < (bd.month, bd.day))

    class Meta:
        verbose_name = "Игрок"
        verbose_name_plural = "Игроки"