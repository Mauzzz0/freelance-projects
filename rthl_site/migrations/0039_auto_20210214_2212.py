# Generated by Django 3.1.4 on 2021-02-14 19:12

from django.db import migrations


class Migration(migrations.Migration):

    dependencies = [
        ('rthl_site', '0038_auto_20210214_1657'),
    ]

    operations = [
        migrations.RenameField(
            model_name='actionpenalty',
            old_name='player',
            new_name='players',
        ),
    ]
