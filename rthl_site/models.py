from django.core.exceptions import ValidationError
from django.db import models
from datetime import date

class Team(models.Model): # TODO: Сделать PK по имени-городу
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
    patronymic = models.CharField("Отчество", max_length=50,blank=True)
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
                                ("П","Правый")),
                            default="Л"
                            )
    qualification = models.CharField("Квалификация",max_length=20,default="Нет")
    games_count = models.PositiveSmallIntegerField("Кол-во игр",default=0)
    goals_count = models.PositiveSmallIntegerField("Кол-во голов",default=0)
    passes_count = models.PositiveSmallIntegerField("Кол-во передач",default=0)
    penalties_count = models.PositiveSmallIntegerField("Кол-во штрафов",default=0)
    disqualified_games_count = models.PositiveSmallIntegerField("Кол-во дисквал игр",default=0)
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

class Season(models.Model):
    """Сезон"""
    start_year = models.PositiveSmallIntegerField("Год начала",unique=True)
    end_year = models.PositiveSmallIntegerField("Год конца",unique=True)

    def __str__(self):
        return str(self.start_year) + "-" + str(self.end_year)

    class Meta:
        verbose_name = "Сезон"
        verbose_name_plural = "Сезоны"

class Tournament(models.Model):
    """Турнир"""
    name = models.CharField("Название",max_length=50,unique=True)
    type = models.CharField("Тип",max_length=20)
    season = models.ForeignKey(
        Season,
        verbose_name="Сезон чемпионата",
        on_delete=models.SET_NULL,
        null = True
        )

    def get_teams(self):
        teams_all = list()
        for _match in self.match_tournament.all():
            _lineups = _match.lineup_match.all()
            for _lineup in _lineups:
                teams_all.append(_lineup.team)
        return teams_all


    def __str__(self):
        return self.name

    class Meta:
        verbose_name = "Турнир"
        verbose_name_plural = "Турниры"


class Match(models.Model):
    """Матч"""
    # TODO: Результаты для трёх раундов
    name = models.CharField("Название",max_length=50)
    date = models.DateTimeField("Дата и время")
    #teamA = models.ForeignKey(
    #    Team,
    #    verbose_name="Команда 1",
    #    related_name="team1",
    #    on_delete=models.SET_NULL,
    #    null=True
    #)
    #teamB = models.ForeignKey(
    #    Team,
    #    verbose_name="Команда 2",
    #    related_name="team2",
    #    on_delete=models.SET_NULL,
    #    null=True
    #)
    tournament = models.ForeignKey(
        Tournament,
        verbose_name="В рамках турнира",
        related_name="match_tournament",
        on_delete=models.SET_NULL,
        null=True
    )
    #goals_list = models.ManyToManyField(
    #    ActionGoal,
    #    verbose_name="Голы",
    #    related_name="match_goals"
    #)
    #penalties_list = models.ManyToManyField(
    #    ActionPenalty,
    #    verbose_name="Штрафы",
    #    related_name="match_penalties"
    #)
    place = models.CharField("Место проведения",max_length=50,blank=True)
    #teamA_total_goals = models.PositiveSmallIntegerField("Общее кол-во голов первой команды",null=True)
    #teamB_total_goals = models.PositiveSmallIntegerField("Общее кол-во голов первой команды",null=True)


    #def __str__(self): # TODO: Починить
    #    return str(self.lineup_match.teamA) + "-" + str(self.teamB)

    class Meta:
        verbose_name = "Матч"
        verbose_name_plural = "Матчи"

class ActionGoal(models.Model):
    """Забитый гол"""
    players = models.ManyToManyField(
        Player,
        verbose_name="Игроки, забившие гол",
        related_name="goal_players"
    )
    match = models.ForeignKey(
        Match,
        verbose_name="Матч",
        related_name="goal_match",
        on_delete=models.CASCADE
    )
    team_side = models.CharField("Команда", max_length=1,
                                 choices=(
                                     ("A","A"),
                                     ("B","B")
                                 ))
    time_minute = models.PositiveSmallIntegerField("Минута матча")
    time_second = models.PositiveSmallIntegerField("Секунда матча")


    #def __str__(self):
        #res = "".join([str(x) + " " for x in (self.match,self.team_side,self.time_minute,self.time_second)])
        #res = str(self.match.date.date()) + " " +\
        #        self.match.teamA.name + " - " +\
        #        self.match.teamB.name + " " +\
        #        str(self.time_minute) + "мин:" +\
        #        str(self.time_second) + "сек команда" +\
        #        str(self.team_side)
        #return res # TODO: Починить

    class Meta:
        verbose_name="Гол"
        verbose_name_plural="Голы"

class ActionPenalty(models.Model):
    """Полученный штраф"""
    # TODO: Заранее список штрафов
    player = models.ForeignKey(
        Player,
        verbose_name="Игрок, получивший штраф",
        related_name="penalty_player",
        on_delete=models.CASCADE
    )
    match = models.ForeignKey(
        Match,
        verbose_name="Матч",
        related_name="penalty_match",
        on_delete=models.CASCADE
    )
    paragraph = models.CharField("Причина",max_length=255,blank=True)
    team_side = models.CharField("Команда", max_length=1,
                                 choices=(
                                     ("A", "A"),
                                     ("B", "B")
                                 ))
    time_minute = models.PositiveSmallIntegerField("Минута матча")
    time_second = models.PositiveSmallIntegerField("Секунда матча")

    def __str__(self):
        return self.player.fullname()

    class Meta:
        verbose_name="Штраф"
        verbose_name_plural="Штрафы"

class Lineup(models.Model):
    """Конкретный состав команды в конкретном матче"""
    match = models.ForeignKey(
        Match,
        verbose_name="Матч",
        related_name="lineup_match",
        on_delete=models.CASCADE
    )
    team = models.ForeignKey(
        Team,
        verbose_name="Название команды",
        related_name="lineup_team",
        on_delete=models.SET_NULL,
        null=True
    )
    players = models.ManyToManyField(
        Player,
        verbose_name="Игрок",
        related_name="lineup_player"
    )
    team_side = models.CharField("Сторона", max_length=1,
                                 choices=(
                                     ("A", "A"),
                                     ("B", "B")
                                 ))

    def __str__(self):
        res = str(self.match.date.date()) + " " +\
            self.team.name + " сторона-" +\
            self.team_side
        return res

    class Meta:
        verbose_name="Состав"
        verbose_name_plural="Составы"