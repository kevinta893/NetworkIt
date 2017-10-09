from networkit import *

import RPi.GPIO as GPIO
import time


#See wiring diagram for demo setup. This code emulates what Arduino's start code looks like



ledPin = 11
buttonPin = 13

pressed = False
count = 0


network_client = None

def setup():
    #setup Pins
    GPIO.setmode(GPIO.BOARD)       # Numbers GPIOs by physical location
    GPIO.setup(ledPin, GPIO.OUT)   # Set LedPin's mode is output
    GPIO.setup(buttonPin, GPIO.IN, pull_up_down=GPIO.PUD_DOWN)
    GPIO.output(ledPin, GPIO.LOW) # Set LedPin high(+3.3V) to turn on led

    #setup network
    print "Connecting to network..."
    global network_client
    network_client = Client("demo_test_username", "localhost", 8000)
    network_client.register_listener(Client.EVENT_MESSAGE, on_message)
    network_client.register_listener(Client.EVENT_CONNECT, on_connect)
    network_client.register_listener(Client.EVENT_DISCONNECT, on_disconnect)
    network_client.register_listener(Client.EVENT_ERROR, on_error)
    network_client.start_connection()




def loop():
    while True:
        global pressed
        if((GPIO.input(buttonPin) == False) and (pressed == False)):
            #on button down
            print "Button pressed!"
            pressed = True

            global count
            global network_client
            message = Message("Poke!")
            message.add_field("num1", str(1))
            message.add_field("num2", str(4))
            count += 1
            message.add_field("count", str(count))
            message.deliverToSelf = False

            network_client.send_message(message)
        elif((GPIO.input(buttonPin) == True) and (pressed == True)):
            pressed = False






#=============================================
#Network Events

def on_message(message):
    print "Message recieved, count=" + message.get_field("count")

    blinkCount = int(message.get_field("count")) % 4
    while (blinkCount > 0):
        GPIO.output(ledPin, GPIO.HIGH)  # led on
        time.sleep(0.2)
        GPIO.output(ledPin, GPIO.LOW)  # led off
        time.sleep(0.2)

        blinkCount -= 1


def on_connect(args):
    print "Client Connected"

def on_disconnect(args):
    print "Client Disconnected"

def on_error(err):
    print "Error!" + str(err)





#=======================================
#cleanup methods


def destroy():
  GPIO.output(ledPin, GPIO.LOW)   # led off
  GPIO.cleanup()                  # Release resource
  global network_client
  network_client.close_connection()         #disconnect

if __name__ == '__main__':     # Program start from here
  setup()
  try:
    loop()
  except KeyboardInterrupt:  # When 'Ctrl+C' is pressed, the child program destroy() will be  executed.
    destroy()

