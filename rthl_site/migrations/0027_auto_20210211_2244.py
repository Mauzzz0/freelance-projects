# Generated by Django 3.1.4 on 2021-02-11 19:44

from django.db import migrations, models


class Migration(migrations.Migration):

    dependencies = [
        ('rthl_site', '0026_auto_20210211_2243'),
    ]

    operations = [
        migrations.AlterField(
            model_name='actiongoal',
            name='time',
            field=models.TimeField(auto_now_add=True, verbose_name='Время'),
        ),
        migrations.AlterField(
            model_name='actionpenalty',
            name='time',
            field=models.TimeField(auto_now_add=True, verbose_name='Время'),
        ),
    ]
