# Generated by Django 3.1.4 on 2021-02-24 15:28

from django.db import migrations


class Migration(migrations.Migration):

    dependencies = [
        ('rthl_site', '0044_fileuploadmodel'),
    ]

    operations = [
        migrations.AlterUniqueTogether(
            name='player',
            unique_together={('name', 'surname', 'patronymic', 'birth_date')},
        ),
    ]