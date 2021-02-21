from django import forms
from .models import FileUploadModel

class UploadFileForm(forms.ModelForm):
    class Meta:
        model = FileUploadModel
        fields = ['name','image']

