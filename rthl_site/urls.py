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
    path('contacts/',views.contacts, name="contacts")
]