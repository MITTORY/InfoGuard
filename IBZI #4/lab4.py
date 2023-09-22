import sys
from PyQt5.QtWidgets import QApplication, QPushButton, QMainWindow, QLineEdit, QPlainTextEdit
from PyQt5.uic import loadUi
from cipher import Cipher

pForClose = None
encryptText = None
closeKeyForNewText = None

class MainApp(QMainWindow):
    
    def __init__(self):
        super().__init__()

        loadUi('guiLab4.ui', self)  

        self.lineOpenKey = self.findChild(QLineEdit, 'lineEdit')
        self.lineCloseKey = self.findChild(QLineEdit, 'lineEdit_2')
        self.plainText = self.findChild(QPlainTextEdit, 'plainTextEdit')
        self.plainEncryptedText = self.findChild(QPlainTextEdit, 'plainTextEdit_2')
        self.plainDecryptedText = self.findChild(QPlainTextEdit, 'plainTextEdit_3')

        self.encrypt = self.findChild(QPushButton, 'pushButton')
        self.encrypt.clicked.connect(self.encrypt_text)
        
        self.decrypt = self.findChild(QPushButton, 'pushButton_2')
        self.decrypt.clicked.connect(self.decrypt_text)

        self.encrypt.setEnabled(True)
        self.decrypt.setEnabled(True)

    def encrypt_text(self):
        global pForClose
        global encryptText
        if not self.lineOpenKey.text():
            text = self.plainText.toPlainText()
            encryptText, pForClose, open_key, closeKeyForNewText = Cipher.encryptNew(text)  
            encryptText_str = ' '.join(map(str, encryptText))
            self.plainEncryptedText.setPlainText(encryptText_str)
            self.lineOpenKey.setText(str(open_key))
            self.lineCloseKey.setText(str(closeKeyForNewText))
        else:
            text = self.plainText.toPlainText()
            key = self.lineOpenKey.text()
            open_key = int(key)
            encryptText, pForClose, closeKeyForNewText = Cipher.encrypt(text, open_key)  
            encryptText_str = ' '.join(map(str, encryptText))
            self.plainEncryptedText.setPlainText(encryptText_str)
            self.lineCloseKey.setText(str(closeKeyForNewText))


    def decrypt_text(self):
        if not self.plainText.toPlainText():
            key = self.lineCloseKey.text()
            close_key = int(key)
            text = self.plainEncryptedText.toPlainText()
            lines = text.split()
            list = [int(line) for line in lines if line.strip()]
            decrypted_text = Cipher.decryptNewText(list, close_key)
        else:
            key = self.lineOpenKey.text()
            open_key = int(key)
            decrypted_text = Cipher.decrypt(encryptText, pForClose, open_key)
        decrypted_text_str = ''.join(decrypted_text) 
        self.plainDecryptedText.setPlainText(decrypted_text_str)

if __name__ == "__main__":
    app = QApplication(sys.argv)
    main_app = MainApp()
    main_app.show()
    sys.exit(app.exec_())
