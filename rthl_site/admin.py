from django.contrib import admin
from django.utils.safestring import mark_safe

from .models import Team, Player
from modeltranslation.admin import TranslationAdmin

admin.site.register(Team)

@admin.register(Player)
class PlayerAdmin(admin.ModelAdmin):
    """Игроки"""
    list_display = ("name","surname","patronymic","role","team_name","get_image")
    readonly_fields = ("get_image",)

    def get_image(self,obj):
        return mark_safe(f'<img src={obj.image.url} width="50" height="50"')

    get_image.short_description = "Фото игрока"