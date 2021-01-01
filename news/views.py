from django.shortcuts import render, redirect
from .models import Articles
from .forms import ArticlesForm
from django.views.generic import DetailView

def news_home(request):
    news = Articles.objects.order_by('-date') # [:2] Выбор последних n записей
    return render(request, "news/news_home.html",{"news" : news})

def create(request):
    error = ""
    if request.method == 'POST':
        form = ArticlesForm(request.POST)
        if form.is_valid():
            form.save()
            return redirect('/')
        else:
            error = "Форма была неверной"
    form = ArticlesForm()

    data= {
        'form' : form,
        'error' : error
    }
    return render(request, 'news/create.html',data)

class NewsDetailView(DetailView):
    model = Articles
    template_name = 'news/details_view.html'
    context_object_name = 'article'
