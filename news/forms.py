from .models import Articles
from django.forms import ModelForm, TextInput, DateTimeInput, Textarea

class ArticlesForm(ModelForm):
    class Meta:
        model = Articles
        fields = ["title","announce","text","date"]

        widgets = {
            "title" : TextInput(attrs={
                'class' : 'form-control',
                'placeholder' : 'Название новости'
            }),
            "announce" : TextInput(attrs={
                'class' : 'form-control',
                'placeholder' : 'Аннонс'
            }),
            "text" : Textarea(attrs={
               'class' : 'form-control',
               'placeholder' : 'Текст'
            }),
            "date" : DateTimeInput(attrs={
                'class' : 'form-control',
                'placeholder' : 'Дата публикации'
            })
        }