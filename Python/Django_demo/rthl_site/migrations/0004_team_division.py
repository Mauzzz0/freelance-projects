# Generated by Django 3.1.4 on 2021-01-31 16:30

from django.db import migrations, models


class Migration(migrations.Migration):

    dependencies = [
        ('rthl_site', '0003_auto_20210131_1746'),
    ]

    operations = [
        migrations.AddField(
            model_name='team',
            name='division',
            field=models.CharField(choices=[('Себербо', 'Себербо'), ('Золота', 'Золота'), ('Меть', 'Меть')], max_length=20, null=True, verbose_name='Дивизион'),
        ),
    ]