from django.contrib import admin
from django.utils.safestring import mark_safe

from .models import *
from modeltranslation.admin import TranslationAdmin

admin.site.register(Season)
admin.site.register(ActionGoal)
admin.site.register(ActionPenalty)
admin.site.register(Lineup)
admin.site.register(FileUploadModel)
admin.site.site_header = 'Администрирование РТХЛ'

@admin.register(Team)
class TeamAdmin(admin.ModelAdmin):
    """Команды"""
    list_display = ("name","get_image")
    readonly_fields = ("get_image",)

    def get_image(self, obj):
        return mark_safe(f'<img src={obj.image.url} width="50" height="50"')

    get_image.short_description = "Текущий логотип команды"

@admin.register(Player)
class PlayerAdmin(admin.ModelAdmin):
    """Игроки"""
    list_display = ("fullname","role","get_image")
    readonly_fields = ("get_image",)

    def get_image(self,obj):
        return mark_safe(f'<img src={obj.image.url} width="50" height="50"')

    get_image.short_description = "Фото игрока"

@admin.register(Tournament)
class TournamentAdmin(admin.ModelAdmin):
    """Турниры"""
    list_display = ("name","season", "is_generated")

@admin.register(Match)
class MatchAdmin(admin.ModelAdmin):
    """Матчи"""
    list_display = ("name","tournament","loop","date")