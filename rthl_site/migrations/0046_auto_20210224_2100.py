# Generated by Django 3.1.4 on 2021-02-24 18:00

from django.db import migrations, models
import django.db.models.deletion


class Migration(migrations.Migration):

    dependencies = [
        ('rthl_site', '0045_auto_20210224_1828'),
    ]

    operations = [
        migrations.AddField(
            model_name='match',
            name='team_A',
            field=models.ForeignKey(null=True, on_delete=django.db.models.deletion.SET_NULL, related_name='match_teamA', to='rthl_site.team', verbose_name='Команда А'),
        ),
        migrations.AddField(
            model_name='match',
            name='team_B',
            field=models.ForeignKey(null=True, on_delete=django.db.models.deletion.SET_NULL, related_name='match_teamB', to='rthl_site.team', verbose_name='Команда B'),
        ),
    ]
