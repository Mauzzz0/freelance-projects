from django import forms
from .models import FileUploadModel, ActionGoal, Player, Team, Match, Tournament


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

class EditPlayerForm(forms.ModelForm):
    class Meta:
        model = Player
        fields = [
            'name', 'surname', 'patronymic',
            'game_number', 'role', 'adm_role',
            'team_name', 'image', 'growth',
            'weight', 'birth_date', 'grip',
            'qualification', 'biography'
        ]

class EditMatchForm(forms.ModelForm):
    class Meta:
        model = Match
        fields = [ 'name', 'date', 'loop',
                   'team_A', 'team_B',
                   'place']

class CreateTeamForm(forms.ModelForm):
    class Meta:
        model = Team
        fields = [
            'name', 'image', 'city', 'url'
        ]

class CreateTournamentForm(forms.ModelForm):
    class Meta:
        model = Tournament
        fields = ['name', 'season','loop_count','teams']

class UploadFileForm(forms.ModelForm):
    class Meta:
        model = FileUploadModel
        fields = ['name','image']

class GoalForm(forms.ModelForm):
    class Meta:
        model = ActionGoal
        fields = ['player_score','players_passes','match','team_side','time_minute','time_second']
