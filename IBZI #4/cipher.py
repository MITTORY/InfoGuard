import random
from math import pow

class Cipher():
    q = random.randint(pow(10, 20), pow(10, 50))
    g = random.randint(2, q)

    def exponentiation(num1, num2, num3):
        x = 1
        y = num1
        while num2 > 0:
            if num2 % 2 == 0:
                x = (x * y) % num3
            y = (y * y) % num3
            num2 = int(num2 / 2)
        return x % num3

    def gcd(a,b):
        if a < b:
            return Cipher.gcd(b, a)
        elif a % b == 0:
            return b
        else:
            return Cipher.gcd(b, a % b)

    def key_generation(q):
        key = random.randint(pow(10, 20), q)
        while Cipher.gcd(q, key) != 1:
            key = random.randint(pow(10, 20), q)
        return key

    def encryptNew(text):
        ct = []
        open_key = Cipher.key_generation(Cipher.q)
        close_key = Cipher.exponentiation(Cipher.g, open_key, Cipher.q)
        s = Cipher.exponentiation(close_key, open_key, Cipher.q)
        p = Cipher.exponentiation(Cipher.g, open_key, Cipher.q)
        close_keyForNewText = Cipher.exponentiation(p, open_key, Cipher.q)
        for i in range(0, len(text)):
            ct.append(text[i])
        for i in range(0, len(ct)):
            ct[i] = s * ord(ct[i])
        print(ct)
        return ct, p, open_key, close_keyForNewText
    
    def encrypt(text, open_key):
        ct = []
        close_key = Cipher.exponentiation(Cipher.g, open_key, Cipher.q)
        s = Cipher.exponentiation(close_key, open_key, Cipher.q)
        p = Cipher.exponentiation(Cipher.g, open_key, Cipher.q)
        close_keyForNewText = Cipher.exponentiation(p, open_key, Cipher.q)
        for i in range(0, len(text)):
            ct.append(text[i])
        for i in range(0, len(ct)):
            ct[i] = s * ord(ct[i])
        return ct, p, close_keyForNewText
    
    def decryptNewText(text, close_key):
        pt = []
        for i in range(0, len(text)):
            pt.append(chr(int(text[i] / close_key)))
        return pt
    
    def decrypt(text, p, open_key):
        pt = []
        close_key = Cipher.exponentiation(p, open_key, Cipher.q)
        for i in range(0, len(text)):
            pt.append(chr(int(text[i] / close_key)))
        return pt




