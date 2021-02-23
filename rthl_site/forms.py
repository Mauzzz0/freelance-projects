from django import forms
from .models import FileUploadModel, ActionGoal

class UploadFileForm(forms.ModelForm):
    class Meta:
        model = FileUploadModel
        fields = ['name','image']

class GoalForm(forms.ModelForm):
    class Meta:
        model = ActionGoal
        fields = ['player_score','players_passes','match','team_side','time_minute','time_second']
