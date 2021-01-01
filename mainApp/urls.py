from django.urls import path, include
from django.conf.urls import url
from . import views

urlpatterns = [
    url('^$', views.index, name='index'),
    url(r'^register/$', views.RegisterFormView.as_view()),
    url(r'^login/$', views.LoginFormView.as_view()),
    url(r'^logout/$', views.LogoutFormView.as_view())
]
