# Generated by Django 3.1.4 on 2021-02-18 15:46

from django.db import migrations, models


class Migration(migrations.Migration):

    dependencies = [
        ('rthl_site', '0041_auto_20210215_2217'),
    ]

    operations = [
        migrations.AddField(
            model_name='actionpenalty',
            name='removal_time',
            field=models.PositiveSmallIntegerField(default=2, verbose_name='Минуты удаления игрока'),
            preserve_default=False,
        ),
    ]
