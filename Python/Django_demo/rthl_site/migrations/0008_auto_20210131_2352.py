# Generated by Django 3.1.4 on 2021-01-31 20:52

from django.db import migrations, models


class Migration(migrations.Migration):

    dependencies = [
        ('rthl_site', '0007_auto_20210131_2323'),
    ]

    operations = [
        migrations.AlterField(
            model_name='player',
            name='disqualified_games_count',
            field=models.PositiveSmallIntegerField(verbose_name='Кол-во дисквал игр'),
        ),
    ]
