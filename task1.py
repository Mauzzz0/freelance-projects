def morning(*kwargs):
    words = [el for el in kwargs]
    output = dict()
    for word in words:
        old_letter = ""
        for letter in word:

            if letter.lower() in ["a", "e", "i", "o", "u", "y"] and letter.lower() not in old_letter:
                letter = letter.lower()
                output[letter] = output.get(letter, 0) + 1
                old_letter += letter
    return output

data = ["The", "sky", "was", "blue", "and", "shining"]
print(morning(*data))
