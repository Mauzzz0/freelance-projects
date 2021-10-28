from django.urls import path
from . import views

urlpatterns = [
    path('', views.Home, name="home"),
    path('schedule/',views.schedule, name="schedule"),
    path('news/',views.news, name="news"),
    path('teams/',views.teams, name="teams"),
    path('documents/',views.documents, name="documents"),
    path('insurance/',views.insurance, name="insurance"),
    path('contacts/',views.contacts, name="contacts"),

    path('create_player',views.CreatePlayerDetailView.as_view()),
    path('create_team',views.CreateTeamDetailView.as_view()),
    path('create_tournament',views.CreateTournamentDetailView.as_view()),
    path('dev_news', views.news, name="news"),
    path('dev_scoreboard', views.scoreboard, name="scoreboard"),
    path('dev_zip', views.ZipView.as_view(), name="zip"),

    path('match/<int:pk>/scoreboard', views.MatchScoreboardDetailView.as_view()),
    path('match/<int:pk>', views.MatchDetailView.as_view()),
    path('match/<int:pk>/edit', views.EditMatchDetailView.as_view()),
    path('team/<slug:slug>', views.TeamDetailView.as_view()),
    path('team/<slug:slug>/create_app', views.TeamAppDetailView.as_view()),
    path('player/<int:pk>', views.PlayerDetailView.as_view()),
    path('player/<int:pk>/edit', views.EditPlayerDetailView.as_view()),
    path('tournament/<int:pk>', views.TournamentDetailView.as_view())
]