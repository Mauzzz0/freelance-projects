from django import forms
from .models import FileUploadModel, ActionGoal, Player


class CreatePlayerForm(forms.ModelForm):
    class Meta:
        model = Player
        fields = [
            'name', 'surname', 'patronymic',
            'game_number', 'role', 'adm_role',
            'team_name', 'image', 'growth',
            'weight', 'birth_date', 'grip',
            'qualification', 'biography'
        ]

class UploadFileForm(forms.ModelForm):
    class Meta:
        model = FileUploadModel
        fields = ['name','image']

class GoalForm(forms.ModelForm):
    class Meta:
        model = ActionGoal
        fields = ['player_score','players_passes','match','team_side','time_minute','time_second']
