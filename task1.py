def morning(*kwargs):
    words = [el for el in kwargs]
    output = dict()
    for word in words:
        for letter in word:
            if letter.lower() in ["a", "e", "i", "o", "u", "y"]:
                letter = letter.lower()
                output[letter] = output.get(letter, 0) + 1
    return output

data = ["The", "sky", "was", "blue", "and", "shining"]
print(morning(*data))
