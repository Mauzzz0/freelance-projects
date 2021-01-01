from django.db import models

class Articles(models.Model):
    title = models.CharField('Название',max_length=50)
    announce = models.CharField('Аннонс',max_length=250)
    text = models.TextField('Текст')
    date = models.DateTimeField('Дата публикации')

    def __str__(self):
        return self.title

    class Meta:
        verbose_name = "Новость"
        verbose_name_plural = "Новости"