# Generated by Django 3.1.4 on 2021-02-01 22:23

from django.db import migrations, models
import django.db.models.deletion


class Migration(migrations.Migration):

    dependencies = [
        ('rthl_site', '0019_match_date'),
    ]

    operations = [
        migrations.AddField(
            model_name='match',
            name='tournament',
            field=models.ForeignKey(null=True, on_delete=django.db.models.deletion.SET_NULL, to='rthl_site.tournament', verbose_name='В рамках турнира'),
        ),
        migrations.AlterField(
            model_name='match',
            name='date',
            field=models.DateTimeField(verbose_name='Дата и время'),
        ),
    ]