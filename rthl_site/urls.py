from django.urls import path
from . import views

urlpatterns = [
    path('', views.PassPage, name="home"), #TODO: Choose homepage
    path('schedule/',views.PassPage, name="schedule"), #TODO: Not work
    path('news/',views.PassPage, name="news"), #TODO: Not work
    path('teams/',views.PassPage, name="teams"), #TODO: Not work
    path('documents/',views.documents, name="documents"),
    path('insurance/',views.insurance, name="insurance"),
    path('contacts',views.contacts, name="contacts")
]