from django.shortcuts import render, redirect
from django.http import HttpResponse
from django.views.generic import DetailView
from .models import Team, Player, Match, Season, Tournament



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
    teamA_coach = lambda x:x.object.lineup_match.get(team_side="A").players.get(adm_role="Главный Тренер")

    teamB_players = lambda x:x.object.lineup_match.get(team_side="B").players.all()
    teamB_coach = lambda x:x.object.lineup_match.get(team_side="B").players.get(adm_role="Главный Тренер")
    teamB_goalkeepers = lambda x: x.object.lineup_match.get(team_side="B").players.filter(role="Вратарь")

    goalsA = lambda x:x.object.goal_match.get(team_side="A")
    goalsB = lambda x:x.object.goal_match.get(team_side="B")

    penaltiesA = lambda x:x.object.penalty_match.get(team_side="A")
    penaltiesB = lambda x:x.object.penalty_match.get(team_side="B")

    goalkeepers_count = lambda x: max(len(x.teamA_goalkeepers()),len(x.teamB_goalkeepers()))
    goalkeepers = lambda x:[x.teamA_goalkeepers(),x.teamB_goalkeepers()]

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

    def VVKPI(self):
        """ВывестиВсеКомандыПервогоИгрока"""
        pl = Player.objects.first()
        objects = pl.team_name.all()
        return objects

    def VVIPK(self):
        """ВывестиВсехИгроковПервойКоманды"""
        obj = Team.objects.first()
        pls = obj.player_team.all()
        return pls

    def VVITK(self):
        """ВывестиВсехИгроковТекущейКоманды"""
        pls = self.object.player_team.all()
        return pls

    #def get_coach(self):
    #    for pl in self.object.player_team.all():
    #        if pl.role == "Гл. Тренер":
    #            return pl.surname + " " + pl.name

class PlayerDetailView(DetailView):
    model = Player
    template_name = "Player/each_player.html"
    context_object_name = 'player'

    #matches = lambda x:[lineup.match for lineup in x.object.lineup_player.all()]
    teams = lambda x:[lineup.team for lineup in x.object.lineup_player.all()]

    def get_matches_by_team(self):
        dic = dict()
        _teams = [lineup.team for lineup in self.object.lineup_player.all()]
        for team in _teams:
            matches = [lineup.match for lineup in self.object.lineup_player.all().filter(team=team)]
            dic[team] = matches
        #matches = self.object.lineup_player.all()
        return dic
