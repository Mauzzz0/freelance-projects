from django.contrib import admin
from django.utils.safestring import mark_safe

from .models import Team, Player
from modeltranslation.admin import TranslationAdmin

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