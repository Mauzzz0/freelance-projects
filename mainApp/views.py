from django.http import HttpResponseRedirect
from django.shortcuts import render
from django.views.generic.edit import FormView
from django.contrib.auth import login
from django.contrib.auth import logout
from django.contrib.auth.forms import AuthenticationForm
from django.contrib.auth.forms import UserCreationForm


def index(request):
    return render(request, 'mainApp/homePage.html')

def Schedule(request):
    return render(request, 'mainApp/Schedule.html')

class RegisterFormView(FormView):
    form_class = UserCreationForm
    success_url = "/login"
    template_name = "mainApp/register.html"

    def form_valid(self, form):
        form.save()
        return super(RegisterFormView,self).form_valid(form)

    def form_invalid(self, form):
        return super(RegisterFormView,self).form_invalid(form)

class LoginFormView(FormView):
    form_class = AuthenticationForm
    template_name = "mainApp/login.html"
    success_url = "/"
    def form_valid(self, form):
        self.user = form.get_user()
        login(self.request,self.user)
        return super(LoginFormView,self).form_valid(form)

class LogoutFormView(FormView):
    def get(self,request):
        logout(request)
        return HttpResponseRedirect("/")