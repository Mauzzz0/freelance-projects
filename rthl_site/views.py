from django.shortcuts import render
from django.http import HttpResponse

def contacts(request):
    return render(request, "Other/contacts.html")

def insurance(request):
    return render(request,"Other/insurance.html")

def documents(request):
    return render(request,"Other/documents.html")

def PassPage(request): # TODO: inProgress
    pass
