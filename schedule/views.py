from django.shortcuts import render

def index(request):
    data = {
        "tittle" : "переданный текст из словаря внутри метода index() в views.py",
        "values": ["text1","text2","text3"]
    }
    return render(request, 'mainApp/schedule.html', data)