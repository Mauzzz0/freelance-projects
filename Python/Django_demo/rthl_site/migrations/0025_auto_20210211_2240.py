# Generated by Django 3.1.4 on 2021-02-11 19:40

from django.db import migrations, models


class Migration(migrations.Migration):

    dependencies = [
        ('rthl_site', '0024_auto_20210211_2237'),
    ]

    operations = [
        migrations.AlterModelOptions(
            name='actiongoal',
            options={'verbose_name': 'Гол', 'verbose_name_plural': 'Голы'},
        ),
        migrations.AlterModelOptions(
            name='actionpenalty',
            options={'verbose_name': 'Штраф', 'verbose_name_plural': 'Штрафы'},
        ),
        migrations.AddField(
            model_name='actionpenalty',
            name='paragraph',
            field=models.CharField(blank=True, max_length=255, verbose_name='Причина'),
        ),
    ]
