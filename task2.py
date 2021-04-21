from PIL import Image, ImageDraw
# К зелёным прибавить ((красная+синяя)/2)/2, не может стать БОЛЬШЕ 255
# От красных и синих отнять 20, не может стать меньше 0
# Отзеркалить по горизонту
def flattery(filename1, filename2):
    img = Image.open(filename1)
    idraw = ImageDraw.Draw(img)
    pix = img.load()

    for x in range(img.size[0]):
        for y in range(img.size[1]):
            red = pix[x, y][0]
            green = pix[x, y][1]
            blue = pix[x, y][2]

            HalfHalfRedGreen = ((red + green)/2)/2 # По условию нужна "половина от среднего значения)


            new_green = int(green + HalfHalfRedGreen)
            if new_green > 255:
                new_green = 255

            new_red = red - 20
            if new_red < 0:
                new_red = 0

            new_blue = blue - 20
            if new_blue < 0:
                new_blue = 0



            idraw.point((x, y), (new_red, new_green, new_blue))

    img.save(filename2)


flattery("car.jpeg", "result.jpeg")
