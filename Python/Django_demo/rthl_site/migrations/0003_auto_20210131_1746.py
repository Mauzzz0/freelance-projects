# Generated by Django 3.1.4 on 2021-01-31 14:46

from django.db import migrations, models


class Migration(migrations.Migration):

    dependencies = [
        ('rthl_site', '0002_player'),
    ]

    operations = [
        migrations.AlterModelOptions(
            name='player',
            options={'verbose_name': 'Игрок', 'verbose_name_plural': 'Игроки'},
        ),
        migrations.AddField(
            model_name='player',
            name='image',
            field=models.ImageField(default='img/player.jpg', upload_to='img/players', verbose_name='Фото'),
        ),
        migrations.AlterField(
            model_name='player',
            name='role',
            field=models.CharField(choices=[('Нападающий', 'Нападающий'), ('Защитник', 'Защитник'), ('Тренер', 'Тренер')], max_length=20, verbose_name='Роль'),
        ),
    ]
