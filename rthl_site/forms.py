from django import forms

class ZipForm(forms.Form):
    post = forms.CharField()