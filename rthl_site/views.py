from django.shortcuts import render
from django.http import HttpResponse
from django.views.generic import DetailView
from .models import Team, Player


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
    return render(request, "Team/team.html")

def Home(request): # TODO: inProgress
    return render(request, "empty.html")

class TeamDetailView(DetailView):
    model = Team
    template_name = "Team/each_team.html"
    context_object_name = 'team'

class PlayerDetailView(DetailView):
    model = Player
    template_name = "Player/each_player.html"
    context_object_name = 'player'
