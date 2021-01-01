from django.contrib import admin
from django.urls import path, include
from django.conf.urls import url

urlpatterns = [
    url('^admin/', admin.site.urls),
    url('', include('mainApp.urls')),
    url('^webexample/', include('webexample.urls')),
    url('^schedule/', include('schedule.urls')),
    url('^news/', include('news.urls')),
]
