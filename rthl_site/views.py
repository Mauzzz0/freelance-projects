from django.shortcuts import render, redirect
from django.http import HttpResponse
from django.views.generic import DetailView
from .models import Team, Player, Match, Season, Tournament
from django.db.models import Q
import operator
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
    return render(request, "Home/home.html")

def create_app(request):
    return render(request, "App/create_app.html")

def match(request):
    return render(request, "Matches/match.html")

def scoreboard(request):
    return render(request, "Scoreboard/scoreboard.html")

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

    lineups = lambda x:x.object.lineup_team.all()
    matches = lambda x: [lineup.match for lineup in x.lineups()]
    tournaments_set = lambda x: set([_match.tournament for _match in x.matches()])

    def future_matches(self):
        _lineups = self.object.lineup_team.all()
        _matches = [x.match for x in _lineups]
        _future_matches = [x for x in _matches if x.is_past_due == False]
        return _future_matches

    def past_matches(self):
        _lineups = self.object.lineup_team.all()
        _matches = [x.match for x in _lineups]
        _past_matches = [x for x in _matches if x.is_past_due == True]
        return _past_matches

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

    def nearest_match(self):
        _lineups = self.object.lineup_team.all()
        _matches = [x.match for x in _lineups]
        _future_matches = [x for x in _matches if x.is_past_due == False]
        nearest_match = sorted(_future_matches,key=lambda x:x.date)[0]
        return nearest_match


class PlayerDetailView(DetailView):
    model = Player
    template_name = "Player/each_player.html"
    context_object_name = 'player'

    lineups = lambda x:x.object.lineup_player.all()
    matches = lambda x:[lineup.match for lineup in x.lineups()]
    tournaments = lambda x:[_match.tournament for _match in x.matches()]
    teams = lambda x:[lineup.team for lineup in x.lineups()]

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
