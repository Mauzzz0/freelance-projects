from django.urls import path
from django.views.generic import TemplateView
from . import views

urlpatterns = [
    path('', views.Home, name="home"), #TODO: Choose homepage
    path('schedule/',views.schedule, name="schedule"),
    path('news/',views.news, name="news"),
    path('teams/',views.teams, name="teams"),
    path('documents/',views.documents, name="documents"),
    path('insurance/',views.insurance, name="insurance"),
    path('contacts/',views.contacts, name="contacts"),

    path('dev_create_app', views.create_app, name="create_app"),
    path('dev_match', views.match, name="match"),
    path('dev_news', views.news, name="news"),
    path('dev_scoreboard', views.scoreboard, name="scoreboard"),
    path('match/<int:pk>', views.MatchDetailView.as_view()),

    path('team/<slug:slug>', views.TeamDetailView.as_view()),
    path('player/<int:pk>', views.PlayerDetailView.as_view())
]