from django.shortcuts import render
from django.http import HttpResponse
# Create your views here.

def index(request):
    return HttpResponse("<h3>Хэдер3. Хардкод из .py (webexample/views.py)</h3>")