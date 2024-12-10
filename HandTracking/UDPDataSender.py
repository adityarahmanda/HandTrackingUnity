import time, random, socket
import Global

UDP_IP_ADDRESS = "127.0.0.1"
UDP_PORT_NO = 2368

def SendingPacket():
    print("Thread Sender Opened")
    clientSock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    
    while(Global.isRun):
        try:
            time.sleep(0.05)
            clientSock.sendto(bytes(Global.handPosition,'utf-8'), (UDP_IP_ADDRESS, UDP_PORT_NO))
        
        except:
            clientSock.sendto(bytes(Global.handPosition,'utf-8'), (UDP_IP_ADDRESS, UDP_PORT_NO))
            pass
        
        #print(f"Sent: {GeneralAttribute.positionHand}")

    print("Thread Sender Closed")