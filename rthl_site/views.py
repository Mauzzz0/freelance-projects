from django.shortcuts import render
from django.http import HttpResponse

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
