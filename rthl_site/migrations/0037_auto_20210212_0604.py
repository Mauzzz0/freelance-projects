# Generated by Django 3.1.4 on 2021-02-12 03:04

from django.db import migrations, models


class Migration(migrations.Migration):

    dependencies = [
        ('rthl_site', '0036_auto_20210212_0116'),
    ]

    operations = [
        migrations.RemoveField(
            model_name='match',
            name='teamA',
        ),
        migrations.RemoveField(
            model_name='match',
            name='teamB',
        ),
        migrations.AlterField(
            model_name='player',
            name='grip',
            field=models.CharField(choices=[('Л', 'Левый'), ('П', 'Правый')], default='Л', max_length=6, verbose_name='Хват'),
        ),
    ]
