# Generated by Django 3.1.4 on 2021-02-24 22:58

from django.db import migrations, models


class Migration(migrations.Migration):

    dependencies = [
        ('rthl_site', '0048_auto_20210225_0152'),
    ]

    operations = [
        migrations.AlterField(
            model_name='team',
            name='division',
            field=models.CharField(choices=[('Золотой', 'Золотой'), (None, None), ('Бронзовый', 'Бронзовый'), ('Серебряный', 'Серебряный')], default='Нет', max_length=50, null=True, verbose_name='Дивизион'),
        ),
    ]
