import datetime

from django.shortcuts import render, redirect
from django.http import HttpResponse, HttpResponseRedirect
from django.utils import timezone
from django.views.generic import DetailView, TemplateView
from pytz import utc

from mysite import settings
from .models import Team, Player, Match, Season, Tournament, ActionGoal, ActionPenalty, Lineup
from django.db.models import Q
from django.db.utils import IntegrityError
import operator
from django.contrib import messages
from .forms import UploadFileForm, GoalForm, CreatePlayerForm, CreateTeamForm, EditPlayerForm, EditMatchForm, CreateTournamentForm
from itertools import chain



def contacts(request):
    return render(request, "Other/contacts.html")

def insurance(request):
    return render(request,"Other/insurance.html")

def documents(request):
    return render(request,"Other/documents.html")

def schedule(request):
    return render(request, "Schedule/schedule.html")

def news(request):
    return render(request, "News/news.html")

def teams(request):
    tournaments = Tournament.objects.all()

    data = {
        "tournaments" : tournaments
    }
    return render(request, "Team/teams.html", data)

def Home(request):
    tournaments = Tournament.objects.all()

    data = {
        "tournaments": tournaments
    }
    return render(request, "Home/home.html", data)

def create_app(request):
    return render(request, "App/create_app.html")

def match(request):
    return render(request, "Matches/match.html")

def scoreboard(request):
    return render(request, "Scoreboard/scoreboard.html")

class ZipView(TemplateView):
    template_name = "dev_zip.html"

    def get(self, request,  *args, **kwargs):
        return render(request, self.template_name)

    def post(self, request):
        print("POST")
        if request.is_ajax():
            print('POST is_ajax')
            print(request.POST)
            """
            Code for new goal or penalty. 
            """
        else:
            print('POST')
            print(request.POST)
        return HttpResponseRedirect(request.path)

class CreatePlayerDetailView(DetailView):
    model = Player
    template_name = "Player/create_player.html"
    context_object_name = "player"

    def get(self, request, *args, **kwargs):
        form = CreatePlayerForm()
        return render(request, self.template_name, {"form": form})

    def post(self, request, *args, **kwargs):
        form = CreatePlayerForm(request.POST, request.FILES)
        if form.is_valid():
            form.save()
            messages.success(request, "Игрок добавлен")
            return HttpResponseRedirect(request.path)
        else:
            messages.error(request, "Форма некорректна")
            return HttpResponseRedirect(request.path)

class CreateTeamDetailView(DetailView):
    model = Team
    template_name = "Team/create_team.html"
    context_object_name = "team"

    def get(self, request, *args, **kwargs):
        form = CreateTeamForm()
        return render(request, self.template_name, {"form": form})

    def post(self, request, *args, **kwargs):
        form = CreateTeamForm(request.POST, request.FILES)
        if form.is_valid():
            form.save()
            messages.success(request, "Команда добавлен")
            return HttpResponseRedirect(request.path)
        else:
            messages.error(request, "Форма некорректна")
            return HttpResponseRedirect(request.path)

class CreateTournamentDetailView(DetailView):
    model = Tournament
    template_name = "Tournament/create_tournament.html"
    context_object_name = "tournament"

    def get(self, request, *args, **kwargs):
        form = CreateTournamentForm()
        return render(request, self.template_name, {"form": form})

    def post(self, request, *args, **kwargs):
        form = CreateTournamentForm(request.POST, request.FILES)
        if form.is_valid():
            form.save()
            messages.success(request, "Турнир добавлен")
            return HttpResponseRedirect(request.path)
        else:
            messages.error(request, "Форма некорректна")
            return HttpResponseRedirect(request.path)

class EditPlayerDetailView(DetailView):
    model = Player
    template_name = "Player/edit_player.html"
    context_object_name = "player"

    def get(self, request, *args, **kwargs):
        self.object = self.get_object()
        form = EditPlayerForm(instance=self.object)
        return render(request, self.template_name, {"form" : form})

    def post(self, request, *args, **kwargs):
        self.object = self.get_object()
        form = EditPlayerForm(request.POST, request.FILES, instance=self.object)
        if form.is_valid():
            form.save()
            messages.success(request, "Игрок обновлён")
            return HttpResponseRedirect(request.path)
        else:
            messages.error(request, "Форма некорректна")
            return HttpResponseRedirect(request.path)

class EditMatchDetailView(DetailView):
    model = Match
    template_name = "Matches/edit_match.html"
    context_object_name = "match"

    def get(self, request, *args, **kwargs):
        self.object = self.get_object()
        form = EditMatchForm(instance=self.object)
        return render(request, self.template_name, {"form": form})

    def post(self, request, *args, **kwargs):
        self.object = self.get_object()
        form = EditMatchForm(request.POST, request.FILES, instance=self.object)
        if form.is_valid():
            form.save()
            messages.success(request, "Матч обновлён")
            return HttpResponseRedirect(request.path)
        else:
            messages.error(request, "Форма некорректна")
            return HttpResponseRedirect(request.path)

class GoalDetailView(DetailView):
    model = ActionGoal
    template_name = "Scoreboard/each_goal.html"
    context_object_name = "goal"

    def get(self, request, *args, **kwargs):
        self.object = self.get_object()
        _goal = ActionGoal.objects.get(id=self.object.id)
        form = GoalForm(instance=_goal)
        return render(request, self.template_name, {"form": form})

class MatchDetailView(DetailView):
    model = Match
    template_name = "Matches/each_match.html"
    context_object_name = "match"
    ampluas = {
        "Вратарь": "Вратари",
        "Защитник": "Защитники",
        "Нападающий": "Нападающие"
    }

    lineup = lambda x:x.object.lineup_match.all()
    teamA = lambda x: x.object.lineup_match.get(team_side="A").team
    teamB = lambda x: x.object.lineup_match.get(team_side="B").team

    teamA_players = lambda x:x.object.lineup_match.get(team_side="A").players.all()
    teamA_goalkeepers = lambda x:x.object.lineup_match.get(team_side="A").players.filter(role="Вратарь")
    teamA_attackers = lambda x:x.object.lineup_match.get(team_side="A").players.filter(role="Нападающий")
    teamA_defenders = lambda x:x.object.lineup_match.get(team_side="A").players.filter(role="Защитник")
    teamA_coach = lambda x:x.object.lineup_match.get(team_side="A").players.get(adm_role="Главный Тренер")

    teamB_players = lambda x:x.object.lineup_match.get(team_side="B").players.all()
    teamB_coach = lambda x:x.object.lineup_match.get(team_side="B").players.get(adm_role="Главный Тренер")
    teamB_goalkeepers = lambda x: x.object.lineup_match.get(team_side="B").players.filter(role="Вратарь")
    teamB_attackers = lambda x: x.object.lineup_match.get(team_side="B").players.filter(role="Нападающий")
    teamB_defenders = lambda x: x.object.lineup_match.get(team_side="B").players.filter(role="Защитник")

    goalsA = lambda x:x.object.goal_match.all().filter(team_side="A")
    goalsB = lambda x:x.object.goal_match.all().filter(team_side="B")
    #penaltiesA = lambda x:x.object.penalty_match.get(team_side="A")
    #penaltiesB = lambda x:x.object.penalty_match.get(team_side="B")

    goalkeepers_count = lambda x: max(len(x.teamA_goalkeepers()),len(x.teamB_goalkeepers()))
    goalkeepers = lambda x:[x.teamA_goalkeepers(),x.teamB_goalkeepers()]

    goals = lambda x:x.object.goal_match.all()
    penalties = lambda x:x.object.penalty_match.all()

    def teamA_statistics_for_attackers(self):
        _attackers = [x for x in self.teamA_attackers()]
        #template = [[0],     [0],      [0],     [0]]
        #         [goals,passes,scores,penalties]

        res = dict()
        for player in _attackers:
            res[player] = [0,0,0,0]
        teamA_main_goals_players = [x.player_score for x in self.goalsA() if x.player_score in _attackers]
        teamA_notmain_goals_players = []
        for goal in self.goalsA():
            for player in goal.players_passes.all():
                if player in _attackers:
                    teamA_notmain_goals_players.append(player)

        for player in teamA_main_goals_players:
            res[player][0] += 1
        for player in teamA_notmain_goals_players:
            res[player][1] += 1
        for player in res.keys():
            res[player][2] = res[player][0] + res[player][1]
        for player in [x.player for x in self.penalties().filter(team_side="A") if x.player in _attackers]:
            res[player][3] += 1
        return res

    def teamB_statistics_for_attackers(self):
        _attackers = [x for x in self.teamB_attackers()]
        #template = [[0],     [0],      [0],     [0]]
        #         [goals,passes,scores,penalties]

        res = dict()
        for player in _attackers:
            res[player] = [0,0,0,0]
        teamB_main_goals_players = [x.player_score for x in self.goalsB() if x.player_score in _attackers]
        teamB_notmain_goals_players = []
        for goal in self.goalsB():
            for player in goal.players_passes.all():
                if player in _attackers:
                    teamB_notmain_goals_players.append(player)

        for player in teamB_main_goals_players:
            res[player][0] += 1
        for player in teamB_notmain_goals_players:
            res[player][1] += 1
        for player in res.keys():
            res[player][2] = res[player][0] + res[player][1]
        for player in [x.player for x in self.penalties().filter(team_side="B") if x.player in _attackers]:
            res[player][3] += 1
        return res

    def teamA_statistics_for_defenders(self):
        _defenders = [x for x in self.teamA_defenders()]
        #template = [[0],     [0],      [0],     [0]]
        #         [goals,passes,scores,penalties]

        res = dict()
        for player in _defenders:
            res[player] = [0,0,0,0]
        teamA_main_goals_players = [x.player_score for x in self.goalsA() if x.player_score in _defenders]
        teamA_notmain_goals_players = []
        for goal in self.goalsA():
            for player in goal.players_passes.all():
                if player in _defenders:
                    teamA_notmain_goals_players.append(player)

        for player in teamA_main_goals_players:
            res[player][0] += 1
        for player in teamA_notmain_goals_players:
            res[player][1] += 1
        for player in res.keys():
            res[player][2] = res[player][0] + res[player][1]
        for player in [x.player for x in self.penalties().filter(team_side="A") if x.player in _defenders]:
            res[player][3] += 1
        return res

    def teamB_statistics_for_defenders(self):
        _defenders = [x for x in self.teamB_defenders()]
        #template = [[0], [0], [0], [0]]
        #         [goals,passes,scores,penalties]

        res = dict()
        for player in _defenders:
            res[player] = [0, 0, 0, 0]
        teamB_main_goals_players = [x.player_score for x in self.goalsB() if x.player_score in _defenders]
        teamB_notmain_goals_players = []
        for goal in self.goalsB():
            for player in goal.players_passes.all():
                if player in _defenders:
                    teamB_notmain_goals_players.append(player)

        for player in teamB_main_goals_players:
            res[player][0] += 1
        for player in teamB_notmain_goals_players:
            res[player][1] += 1
        for player in res.keys():
            res[player][2] = res[player][0] + res[player][1]
        for player in [x.player for x in self.penalties().filter(team_side="B") if x.player in _defenders]:
            res[player][3] += 1
        return res

    def state_for_each_goal(self):
        _goals = sorted(self.goals(), key=operator.attrgetter('time_minute', 'time_second'))
        res = dict()
        a = 0
        b = 0
        for goal in _goals:
            if goal.team_side == "A":
                a += 1
            elif goal.team_side == "B":
                b += 1
            res[goal] = [a,b]
        return res

    def actions1period(self):
        _goals = [x for x in self.goals() if x.time_minute < 20]
        _penalties = [x for x in self.penalties() if x.time_minute < 20]
        res = list()
        res.extend(_goals)
        res.extend(_penalties)
        res = sorted(res, key=operator.attrgetter('time_minute','time_second'))
        return res

    def actions2period(self):
        _goals = [x for x in self.goals() if 20 <= x.time_minute < 40]
        _penalties = [x for x in self.penalties() if 20 <= x.time_minute < 40]
        res = list()
        res.extend(_goals)
        res.extend(_penalties)
        res = sorted(res, key=operator.attrgetter('time_minute', 'time_second'))
        return res

    def actions3period(self):
        _goals = [x for x in self.goals() if 40 <= x.time_minute < 60]
        _penalties = [x for x in self.penalties() if 40 <= x.time_minute < 60]
        res = list()
        res.extend(_goals)
        res.extend(_penalties)
        res = sorted(res, key=operator.attrgetter('time_minute', 'time_second'))
        return res

    def result1period(self):
        _goalsA = [x for x in self.goals() if x.time_minute < 20 and x.team_side == "A"]
        _goalsB = [x for x in self.goals() if x.time_minute < 20 and x.team_side == "B"]
        return str(len(_goalsA)) + ":" + str(len(_goalsB))

    def result2period(self):
        _goalsA = [x for x in self.goals() if 20 <= x.time_minute < 40 and x.team_side == "A"]
        _goalsB = [x for x in self.goals() if 20 <=x.time_minute < 40 and x.team_side == "B"]
        return str(len(_goalsA)) + ":" + str(len(_goalsB))

    def result3period(self):
        _goalsA = [x for x in self.goals() if 40 <= x.time_minute < 60 and x.team_side == "A"]
        _goalsB = [x for x in self.goals() if 40 <= x.time_minute < 60 and x.team_side == "B"]
        return str(len(_goalsA)) + ":" + str(len(_goalsB))

class MatchScoreboardDetailView(DetailView):
    http_method_names = ['get', 'post', 'delete']
    model = Match
    template_name = "Scoreboard/scoreboard.html"
    context_object_name = "match"
    ampluas = {
        "Вратарь": "Вратари",
        "Защитник": "Защитники",
        "Нападающий": "Нападающие"
    }

    teamA = lambda x: x.object.lineup_match.get(team_side="A").team
    teamB = lambda x: x.object.lineup_match.get(team_side="B").team

    teamA_players = lambda x: x.object.lineup_match.get(team_side="A").players.all()
    teamA_players_sorted_by_game_number = lambda x: sorted([pl for pl in x.teamA_players()], key=operator.attrgetter('game_number'))
    teamA_goalkeepers = lambda x: x.object.lineup_match.get(team_side="A").players.filter(role="Вратарь")
    teamA_attackers = lambda x: x.object.lineup_match.get(team_side="A").players.filter(role="Нападающий")
    teamA_defenders = lambda x: x.object.lineup_match.get(team_side="A").players.filter(role="Защитник")

    teamB_players = lambda x: x.object.lineup_match.get(team_side="B").players.all()
    teamB_players_sorted_by_game_number = lambda x: sorted([pl for pl in x.teamB_players()], key=operator.attrgetter('game_number'))
    teamB_goalkeepers = lambda x: x.object.lineup_match.get(team_side="B").players.filter(role="Вратарь")
    teamB_attackers = lambda x: x.object.lineup_match.get(team_side="B").players.filter(role="Нападающий")
    teamB_defenders = lambda x: x.object.lineup_match.get(team_side="B").players.filter(role="Защитник")

    penalties = lambda x: x.object.penalty_match.all()
    goals = lambda x: x.object.goal_match.all()
    goalsA = lambda x: x.object.goal_match.all().filter(team_side="A")
    goalsB = lambda x: x.object.goal_match.all().filter(team_side="B")

    def post(self, request, *args, **kwargs):
        print("ПОЛУЧЕН ПОСТ")
        self.object = self.get_object()
        if 'goalA_button_remove' in request.POST:
            ActionGoal.objects.get(id=request.POST['goalA_button_remove']).delete()

        elif 'goalB_button_remove' in request.POST:
            ActionGoal.objects.get(id=request.POST['goalB_button_remove']).delete()

        elif 'penaltyA_button_remove' in request.POST:
            ActionPenalty.objects.get(id=request.POST['penaltyA_button_remove']).delete()

        elif 'penaltyB_button_remove' in request.POST:
            ActionPenalty.objects.get(id=request.POST['penaltyB_button_remove']).delete()

        elif 'button_goalA' in request.POST and 'teamA_goal_player' in request.POST:

            A_time_minute = request.POST.get('A_goal_time_minute')
            A_time_second = request.POST.get('A_goal_time_second')

            A_goal_assistant1 = request.POST.get('teamA_goal_assistant1')
            A_goal_assistant2 = request.POST.get('teamA_goal_assistant2')
            A_goal_player = request.POST.get('teamA_goal_player')

            A_goal_ass1_id = -1
            A_goal_ass2_id = -1
            A_goal_player_id = -1

            for player in self.teamA_players():
                if str(player.game_number) == str(A_goal_assistant1):
                    A_goal_ass1_id = player.pk
                elif str(player.game_number) == str(A_goal_assistant2):
                    A_goal_ass2_id = player.pk
                elif str(player.game_number) == str(A_goal_player):
                    A_goal_player_id = player.pk

            new_goal = ActionGoal(
                player_score_id = A_goal_player_id,
                match_id = self.object.id,
                team_side="A",
                time_minute=A_time_minute,
                time_second=A_time_second
            )

            if A_goal_player_id == -1:
                messages.error(self.request, 'Номер одного из ассистентов совпадает с номером игрока '
                                             'забившего гол')

            elif A_goal_ass1_id != -1 and A_goal_ass2_id != -1:
                new_goal.save()
                new_goal.players_passes.add(
                    Player.objects.get(id=A_goal_ass1_id),
                    Player.objects.get(id=A_goal_ass2_id))
                messages.success(self.request, 'Гол А добавлен')

            elif A_goal_ass1_id == -1 and A_goal_ass2_id == -1:
                new_goal.save()
                messages.success(self.request, 'Гол А добавлен без ассистентов')

            else:
                messages.error(self.request, 'Неправильно выбраны игроки для гола. Асистенты '
                                             'должны либо отсутствовать, либо различаться')

        elif 'button_goalB' in request.POST and 'teamB_goal_player' in request.POST:
            B_time_minute = request.POST.get('B_goal_time_minute')
            B_time_second = request.POST.get('B_goal_time_second')

            B_goal_assistant1 = request.POST.get('teamB_goal_assistant1')
            B_goal_assistant2 = request.POST.get('teamB_goal_assistant2')
            B_goal_player = request.POST.get('teamB_goal_player')

            B_goal_ass1_id = -1
            B_goal_ass2_id = -1
            B_goal_player_id = -1

            for player in self.teamB_players():
                if str(player.game_number) == str(B_goal_assistant1):
                    B_goal_ass1_id = player.pk
                elif str(player.game_number) == str(B_goal_assistant2):
                    B_goal_ass2_id = player.pk
                elif str(player.game_number) == str(B_goal_player):
                    B_goal_player_id = player.pk

            new_goal = ActionGoal(
                player_score_id = B_goal_player_id,
                match_id = self.object.id,
                team_side="B",
                time_minute=B_time_minute,
                time_second=B_time_second
            )

            if B_goal_player_id == -1:
                messages.error(self.request, 'Номер одного из ассистентов совпадает с номером игрока '
                                             'забившего гол')

            elif B_goal_ass1_id != -1 and B_goal_ass2_id != -1:
                new_goal.save()
                new_goal.players_passes.add(
                    Player.objects.get(id=B_goal_ass1_id),
                    Player.objects.get(id=B_goal_ass2_id))
                messages.success(self.request, 'Гол B добавлен')

            elif B_goal_ass1_id == -1 and B_goal_ass2_id == -1:
                new_goal.save()
                messages.success(self.request, 'Гол B добавлен без ассистентов')

            else:
                messages.error(self.request, 'Неправильно выбраны игроки для гола. Асистенты '
                                             'должны либо отсутствовать, либо различаться')

        elif 'teamA_penalty_player' in request.POST:

            A_penalty_player = request.POST.get('teamA_penalty_player')
            A_penalty_player_id = -1

            for player in self.teamA_players():
                if str(player.game_number) == str(A_penalty_player):
                    A_penalty_player_id = player.pk

            new_penalty = ActionPenalty(
                player_id=A_penalty_player_id,
                match_id=self.object.id,
                paragraph="A example paragraph",
                team_side="A",
                removal_time=7,
                time_minute=request.POST['A_penalty_time_minute'],
                time_second=request.POST['A_penalty_time_second']
            )
            new_penalty.save()
            messages.success(self.request, 'Штраф A добавлен')

        elif 'teamB_penalty_player' in request.POST:

            B_penalty_player = request.POST.get('teamB_penalty_player')
            B_penalty_player_id = -1

            for player in self.teamB_players():
                if str(player.game_number) == str(B_penalty_player):
                    B_penalty_player_id = player.pk

            new_penalty = ActionPenalty(
                player_id=B_penalty_player_id,
                match_id=self.object.id,
                paragraph="B example paragraph",
                team_side="B",
                removal_time=7,
                time_minute=request.POST['B_penalty_time_minute'],
                time_second=request.POST['B_penalty_time_second']
            )
            new_penalty.save()
            messages.success(self.request, 'Штраф B добавлен')

        return HttpResponseRedirect(request.path)

    def actions1period(self):
        _goals = [x for x in self.goals() if x.time_minute < 20]
        _penalties = [x for x in self.penalties() if x.time_minute < 20]
        res = list()
        res.extend(_goals)
        res.extend(_penalties)
        res = sorted(res, key=operator.attrgetter('time_minute', 'time_second'))
        return res

    def actions2period(self):
        _goals = [x for x in self.goals() if 20 <= x.time_minute < 40]
        _penalties = [x for x in self.penalties() if 20 <= x.time_minute < 40]
        res = list()
        res.extend(_goals)
        res.extend(_penalties)
        res = sorted(res, key=operator.attrgetter('time_minute', 'time_second'))
        return res

    def actions3period(self):
        _goals = [x for x in self.goals() if 40 <= x.time_minute < 60]
        _penalties = [x for x in self.penalties() if 40 <= x.time_minute < 60]
        res = list()
        res.extend(_goals)
        res.extend(_penalties)
        res = sorted(res, key=operator.attrgetter('time_minute', 'time_second'))
        return res

class TeamDetailView(DetailView):
    model = Team
    template_name = "Team/each_team.html"
    context_object_name = 'team'
    slug_field = "url"
    ampluas = {
        "Вратарь" : "Вратари",
        "Защитник" : "Защитники",
        "Нападающий" : "Нападающие",
        "Тренер" : "Тренеры"
    }
    players = lambda x: x.object.player_team.all()
    coach = lambda x:x.object.player_team.get(adm_role="Главный Тренер")

    #lineups = lambda x:x.object.lineup_team.all()
    #matches = lambda x: [lineup.match for lineup in x.lineups()]
    tournaments_set = lambda x: set([j for j in x.object.tournament_teams.all()])

    def post(self, request, *args, **kwargs):
        self.object = self.get_object()
        print("ПОЛУЧЕН ПОСТ")
        if 'delete_player_id' in request.POST:
            player = Player.objects.get(id = request.POST['delete_player_id'])
            player.team_name.remove(self.object)
            messages.success(self.request, 'Игрок {} удалён из команды'.format(player))
        return HttpResponseRedirect(request.path)


    def future_matches(self):
        _matches_teamA = [m for m in self.object.match_teamA.all() if not m.is_past_due]
        _matches_teamB = [m for m in self.object.match_teamB.all() if not m.is_past_due]
        matches = _matches_teamA + _matches_teamB
        matches = sorted(matches, key=operator.attrgetter('date'), reverse=False)
        return matches

    def past_matches(self):
        _matches_teamA = [m for m in self.object.match_teamA.all() if m.is_past_due]
        _matches_teamB = [m for m in self.object.match_teamB.all() if m.is_past_due]
        matches = _matches_teamA + _matches_teamB
        matches = sorted(matches, key=operator.attrgetter('date'), reverse=True)
        return matches

class TeamAppDetailView(DetailView):
    model = Team
    template_name = "App/create_app.html"
    context_object_name = 'team'
    slug_field = "url"

    players = lambda x: x.object.player_team.all()
    players_outfield = lambda x:x.object.player_team.all().filter(
        Q(role="Нападающий") | Q(role="Защитник")
    )
    players_goalkeepers = lambda x:x.object.player_team.all().filter(role="Вратарь")
    players_adm = lambda x:x.object.player_team.all().filter(role="Тренер")

    def post(self, request, *args, **kwargs):
        self.object = self.get_object()
        print("ПОЛУЧЕН ПОСТ")
        if 'selected_game_numbers' in request.POST:
            if self.nearest_match() is None:
                messages.error(request,"Ближайший матч не найден")
            else:
                game_numbers = [int(x) for x in request.POST.getlist('selected_game_numbers')]
                players_to_match = [x for x in self.players() if x.game_number in game_numbers]
                team_side = "A" if self.nearest_match().team_A == self.object else "B"
                lineup = Lineup(
                    match_id=self.nearest_match().id,
                    team = self.object,
                    team_side=team_side
                )
                try:
                    lineup.save()
                    for player in players_to_match:
                        lineup.players.add(
                            Player.objects.get(id=player.id)
                        )
                except IntegrityError:
                    messages.error(request,'Состав команды для данного матча уже утверждён')

        return HttpResponseRedirect(request.path)

    def nearest_match(self):
        _matches_teamA = [m for m in self.object.match_teamA.all() if not m.is_past_due]
        _matches_teamB = [m for m in self.object.match_teamB.all() if not m.is_past_due]
        matches = _matches_teamA + _matches_teamB
        matches = sorted(matches, key=operator.attrgetter('date'), reverse=False)
        return matches[0] if len(matches) >0 else None

class PlayerDetailView(DetailView):
    model = Player
    template_name = "Player/each_player.html"
    context_object_name = 'player'

    lineups = lambda x:x.object.lineup_player.all()
    matches = lambda x:[lineup.match for lineup in x.lineups()]
    tournaments = lambda x:[_match.tournament for _match in x.matches()]
    teams = lambda x:[lineup.team for lineup in x.lineups()]

    def get_games_count(self):
        #self.object.games_count = self.lineups().count()
        #self.object.save()
        return self.lineups().count()

    def get_goals_count(self):
        # self.object.games_count = self.lineups().count()
        # self.object.save()
        count = 0
        for lineup in self.lineups():
            for goal in lineup.match.goal_match.all():
                if goal.player_score == self.object:
                    count += 1
            return count

    def get_passes_count(self):
        # self.object.games_count = self.lineups().count()
        # self.object.save()
        count = 0
        for lineup in self.lineups():
            for goal in lineup.match.goal_match.all():
                for player in goal.players_passes.all():
                    if player == self.object:
                        count +=1
        return count

    def get_penalties_count(self):
        # self.object.games_count = self.lineups().count()
        # self.object.save()
        count = 0
        for lineup in self.lineups():
            for penalty in lineup.match.penalty_match.all():
                if penalty.player == self.object:
                    count += 1
        return count

    def tournament_team(self):
        dic = dict()
        for tournament in self.tournaments():
            for lineup in self.lineups():
                    if lineup.match.tournament == tournament:
                        dic[tournament] = lineup.team
        return dic

    def get_matches_by_team(self):
        dic = dict()
        _teams = [lineup.team for lineup in self.object.lineup_player.all()]
        for team in _teams:
            matches = [lineup.match for lineup in self.object.lineup_player.all().filter(team=team)]
            dic[team] = matches
        #matches = self.object.lineup_player.all()
        return dic

class TournamentDetailView(DetailView):
    model = Tournament
    template_name = 'Tournament/each_tournament.html'
    context_object_name = 'tournament'


    def list_of_tables(self):
        list_of_teams = [x for x in self.object.get_teams_set()]
        query_of_matches = self.object.match_tournament.all()
        teams_count = len(list_of_teams)
        list_of_tables = []
        for loop_counter in range(self.object.loop_count):
            table = [[None for x in range(teams_count+1)] for y in range(teams_count+1) ]
            list_of_tables.append(table)

            """Generating header-row[0] and header-column[0]"""
            table[0] = list_of_teams
            for i in range(1,len(table)):
                table[i][0] = list_of_teams[i-1]

            """Filling with matches"""
            for row in range(1,len(table)):
                for column in range(1, len(table)):
                    row_header = table[row][0]
                    column_header = table[column][0]

                    _match = query_of_matches.filter(loop=loop_counter+1,
                                                     team_A=row_header,
                                                     team_B=column_header)
                    if not _match:
                        _match = query_of_matches.filter(loop=loop_counter+1,
                                                     team_A=column_header,
                                                     team_B=row_header)
                        if not _match:
                            _match = None
                        else:
                            _match = _match[0]
                    else:
                        _match = _match[0]
                    table[row][column] = _match


        for table in list_of_tables:
            for line in table:
                print(line)

        return list_of_tables

    def post(self, request, *args, **kwargs):
        print(request.POST)
        if 'GenerateMatches' in request.POST:
            print('Получен запрос генерации матчей')
            self.object = self.get_object()

            teams_list = [x for x in self.object.teams.all()]
            if self.object.is_generated is False:
                for i in range(self.object.loop_count):
                    for team1_i in range(len(teams_list)):
                        for team2_i in range(team1_i+1,len(teams_list)):
                            teamA = teams_list[team1_i]
                            teamB = teams_list[team2_i]

                            new_match = Match(
                                name = "Матч "+teamA.name+teamB.name,
                                date =
                                timezone.localtime() + datetime.timedelta(days=10*(i+1)),
                                loop = i+1,
                                tournament_id=self.object.id,
                                team_A_id=teamA.id,
                                team_B_id=teamB.id,
                                place="Место по умолчанию"
                            )
                            new_match.save()
                            self.object.is_generated = True
                            self.object.save()


        return HttpResponseRedirect(request.path)