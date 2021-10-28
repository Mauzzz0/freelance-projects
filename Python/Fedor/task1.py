def morning(*kwargs):
    words = [el for el in kwargs]
    output = dict({"a" : 0, "e" : 0, "i":0, "o":0,"u":0, "y":0})
    for word in words:
        old_letter = ''
        letters = ["a", "e", "i", "o", "u", "y"]
        for letter in word:
            letter = letter.lower()
            if letter in letters and letter not in old_letter:
                output[letter] = output.get(letter) + 1
                old_letter += letter
    return output

data = ["The", "sky", "was", "blue", "and", "shining"]
print(morning(*data))
