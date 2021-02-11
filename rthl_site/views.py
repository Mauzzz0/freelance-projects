from django.shortcuts import render, redirect
from django.http import HttpResponse
from django.views.generic import DetailView
from .models import Team, Player, Match



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
    return render(request, "Team/teams.html")

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


    teamA_players = lambda x:x.object.lineup_match.get(team_side="A").players.all()
    teamA_coach = lambda x:x.object.lineup_match.get(team_side="A").players.get(adm_role="Главный Тренер")
    #teamA_players = lambda x:x.object.teamA.player_team.all()
    #teamA_coach = lambda x:x.object.teamA.player_team.all().filter(adm_role="Главный Тренер")[0]
    # [0], тк возвращается QuerySet<> <=> первый найденный гл тренер

    #teamB_players= lambda x:x.object.teamB.player_team.all()
    #teamB_coach = lambda x:x.object.teamB.player_team.all().filter(adm_role="Главный Тренер")[0]
    # [0], тк возвращается QuerySet<> <=> первый найденный гл тренер

    goalsA = lambda x:x.object.goal_match.all().filter(team_side="A")
    goalsB = lambda x:x.object.goal_match.all().filter(team_side="B")

    penaltiesA = lambda x:x.object.penalty_match.all().filter(team_side="A")
    penaltiesB = lambda x:x.object.penalty_match.all().filter(team_side="B")


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
