# Generated by Django 3.1.4 on 2021-02-11 19:51

from django.db import migrations, models
import django.db.models.deletion


class Migration(migrations.Migration):

    dependencies = [
        ('rthl_site', '0029_auto_20210211_2246'),
    ]

    operations = [
        migrations.RemoveField(
            model_name='match',
            name='goals_list',
        ),
        migrations.RemoveField(
            model_name='match',
            name='penalties_list',
        ),
        migrations.AddField(
            model_name='actiongoal',
            name='match',
            field=models.ForeignKey(default=1, on_delete=django.db.models.deletion.CASCADE, related_name='goal_match', to='rthl_site.match', verbose_name='Матч'),
            preserve_default=False,
        ),
        migrations.AddField(
            model_name='actionpenalty',
            name='match',
            field=models.ForeignKey(default=1, on_delete=django.db.models.deletion.CASCADE, related_name='penalty_match', to='rthl_site.match', verbose_name='Матч'),
            preserve_default=False,
        ),
    ]
