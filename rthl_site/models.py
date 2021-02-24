import operator

from django.core.exceptions import ValidationError
from django.db import models
from datetime import date, datetime

from django.utils.timezone import utc

class FileUploadModel(models.Model):
    name = models.CharField(max_length=40)
    image = models.ImageField(upload_to='images/test')

class Team(models.Model): # TODO: Сделать PK по имени-городу
    """Команда"""
    name = models.CharField("Название", max_length=50, unique=True)
    image = models.ImageField("Логотип", upload_to="img/teams",default="img/team.jpg")
    city = models.CharField("Город",max_length=30, default="Москва")
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
    qualification = models.CharField("Квалификация",max_length=20,
                                     choices=(
                                         ("Нет","Нет"),
                                         ("Практик","Практик"),
                                         ("Практик 2.0", "Практик 2.0"),
                                         ("Красный теоретик","Красный теоретик"),
                                         ("Красный практик", "Красный практик"),
                                         ("Теоретик", "Теоретик"),
                                         ("Любитель", "Любитель"),
                                         ("Юниор", "Юниор"),
                                         ("Спортсмен", "Спортсмен"),
                                         ("Мастер спорта", "Мастер спорта")),
                                     default="Нет"
                                     )
    games_count = models.PositiveSmallIntegerField("Кол-во игр",default=0)
    goals_count = models.PositiveSmallIntegerField("Кол-во голов",default=0)
    passes_count = models.PositiveSmallIntegerField("Кол-во передач",default=0)
    penalties_count = models.PositiveSmallIntegerField("Кол-во штрафов",default=0)
    disqualified_games_count = models.PositiveSmallIntegerField("Кол-во дисквал игр",default=0)
    biography = models.TextField("Биография", blank=True, default="Нет")

    def two_letters_qualification(self):
        dic = {
            "Нет" : "NO",
            "Практик" : "ПР",
            "Практик 2.0" : "П2.0",
            "Красный теоретик" : "КТР",
            "Красный практик" : "КПР",
            "Теоретик" : "ТР",
            "Любитель" : "ЛБ",
            "Юниор" : "ЮН",
            "Спортсмен" : "СП",
            "Мастер спорта" : "МС"
        }
        return str(dic[self.qualification])

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
        unique_together = (("name", "surname","patronymic", "birth_date"),)

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

    def get_teams_set(self):
        teams_all = list()
        for _match in self.match_tournament.all():
            teams_all.append(_match.team_A)
            teams_all.append(_match.team_B)
        return set(teams_all)

    def get_2past_matches(self):
        # Короче, эта штука долджна быть умной и понимать сколько предыдущих
        # и будущих выводить, а то весь слайдер ломается
        # TODO:
        _matches = [x for x in self.match_tournament.all()]
        res = list()
        for match in _matches:
            if match.is_past_due and match not in res:
                res.append(match)
        res = sorted(res, key=operator.attrgetter('date'),reverse=False)[:2]
        return res

    def get_3nearest_matches(self):
        # Короче, эта штука долджна быть умной и понимать сколько предыдущих
        # и будущих выводить, а то весь слайдер ломается
        # TODO:
        _matches = [x for x in self.match_tournament.all()]
        res = list()
        for match in _matches:
            if not match.is_past_due and match not in res:
                res.append(match)
        res = sorted(res, key=operator.attrgetter('date'))[:3]
        return res


    def __str__(self):
        return self.name

    class Meta:
        verbose_name = "Турнир"
        verbose_name_plural = "Турниры"

class Match(models.Model):
    """Матч"""
    # TODO: Результаты для трёх раундов
    name = models.CharField("Title",max_length=50)
    date = models.DateTimeField("Date and time")

    tournament = models.ForeignKey(
        Tournament,
        verbose_name="In tournament",
        related_name="match_tournament",
        on_delete=models.SET_NULL,
        null=True
    )

    team_A = models.ForeignKey(
        Team,
        verbose_name="team А",
        related_name="match_teamA",
        on_delete=models.SET_NULL,
        null=True
    )

    team_B = models.ForeignKey(
        Team,
        verbose_name="team B",
        related_name="match_teamB",
        on_delete=models.SET_NULL,
        null=True
    )

    place = models.CharField("Place",max_length=50,blank=True)

    @property
    def is_past_due(self):
        return datetime.now(utc) > self.date

    @property
    def teamA(self):
        return self.lineup_match.get(team_side="A").team

    @property
    def teamB(self):
        return self.lineup_match.get(team_side="B").team

    @property
    def get_result_str(self):
        goalsA = [i for i in self.goal_match.all().filter(team_side="A")]
        goalsB = [i for i in self.goal_match.all().filter(team_side="B")]
        return str(len(goalsA)) + ":" + str(len(goalsB))

    def info(self):
        res = ""
        localdate = self.date.astimezone()
        fdate = localdate.strftime("%d.%m.%Y %H:%M")
        res += str(fdate) + " "
        res += str(self.place) + " "
        res += self.team_A.name
        res += "-"
        res += self.team_B.name
        return res


    def __str__(self):
        res = str(self.date.strftime("%Y-%m-%d %H:%M "))
        teams = [x.team.name for x in self.lineup_match.all()]
        for i in range(len(teams)):
            res += teams[i]
            res += " "
        return res

    class Meta:
        verbose_name = "Матч"
        verbose_name_plural = "Матчи"

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
        unique_together = (('match','team_side', 'team'),)

class ActionGoal(models.Model):
    """Забитый гол"""
    #players = models.ManyToManyField(
    #    Player,
    #    verbose_name="Игроки, забившие гол",
    #    related_name="goal_players"
    #)
    player_score = models.ForeignKey(
        Player,
        verbose_name="Игрок, забивший гол",
        related_name="goal_main_player",
        on_delete=models.SET_NULL,
        null=True
    )
    players_passes = models.ManyToManyField(
        Player,
        verbose_name="Игроки, сделавшие пас",
        related_name="goal_notmain_players"
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
    type = "Гол"

    def __str__(self):
        #res = "".join([str(x) + " " for x in (self.match,self.team_side,self.time_minute,self.time_second)])
        res = str(self.match.date.date()) + " " +\
                str(self.time_minute) + "мин:" +\
                str(self.time_second) + "сек команда" +\
                str(self.team_side)
        return res

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
    removal_time = models.PositiveSmallIntegerField("Минуты удаления игрока")
    team_side = models.CharField("Команда", max_length=1,
                                 choices=(
                                     ("A", "A"),
                                     ("B", "B")
                                 ))
    time_minute = models.PositiveSmallIntegerField("Минута матча")
    time_second = models.PositiveSmallIntegerField("Секунда матча")
    type = "Штраф"

    def __str__(self):
        return str(self.time_minute) + " " + str(self.time_second) + " " + self.player.fullname()

    class Meta:
        verbose_name="Штраф"
        verbose_name_plural="Штрафы"