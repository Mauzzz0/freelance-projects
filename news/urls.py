from . import views
from django.urls import path
from django.conf.urls import url

urlpatterns = [
    url('^$', views.news_home,name="news_home"),
    url('^create$', views.create,name="news_create"),
    path('<int:pk>', views.NewsDetailView.as_view(), name="news_detail")
]
