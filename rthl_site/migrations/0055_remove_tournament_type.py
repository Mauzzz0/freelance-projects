# Generated by Django 3.1.4 on 2021-02-25 21:04

from django.db import migrations


class Migration(migrations.Migration):

    dependencies = [
        ('rthl_site', '0054_tournament_teams'),
    ]

    operations = [
        migrations.RemoveField(
            model_name='tournament',
            name='type',
        ),
    ]
