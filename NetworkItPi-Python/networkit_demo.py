from networkit import Client, Message, Field

from Tkinter import Tk, Label, Button, Frame
import tkMessageBox


class MainWindow(Frame):

    count =0

    def __init__(self, master=None):

        self.server = None

        #setup window
        self.mainWindow = Tk()

        self.btnConnect = Button(self.mainWindow, text="Connect", command=self.connect_button)
        self.btnConnect.place(x=0, y=0)

        self.btnSend = Button(self.mainWindow, text="Send poke", command=self.send_poke)
        self.btnSend.place(x=0, y=30)


        self.mainWindow.protocol("WM_DELETE_WINDOW", self.on_closing)

        self.mainWindow.mainloop()






    def connect_button(self):

        # setup network
        self.server = Client("demo_test_username", "localhost", 8000)
        self.server.register_listener(Client.EVENT_MESSAGE, self.on_message)
        self.server.register_listener(Client.EVENT_CONNECT, self.on_connect)
        self.server.register_listener(Client.EVENT_DISCONNECT, self.on_disconnect)
        self.server.register_listener(Client.EVENT_ERROR, self.on_error)
        self.server.start_connection()


    def send_poke(self):
        #send a message
        message = Message("Poke!")
        message.add_field("num1", str(1))
        message.add_field("num2", str(4))
        self.count += 1
        message.add_field("count", str(self.count))
        message.deliverToSelf = False
        self.server.send_message(message)


    def on_message(self, args):
        print "Message recieved" + str(args)

    def on_connect(self, args):
        print "Client Connected"

    def on_disconnect(self, args):
        print "Client Disconnected"

    def on_error(self, args):
        print "Error!" + str(args)




    def on_closing(self):
        # on close window, disconnect from server
        if (self.server != None):
            self.server.close_connection()
        self.mainWindow.destroy()


#Launch the main window
MainWindow()



