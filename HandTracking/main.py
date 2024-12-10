from tkinter import *
from tkinter import ttk
import cv2, pygame.image, pygame.camera

#tracking
from cvzone.HandTrackingModule import HandDetector
from cvzone.FPS import FPS 
import GeneralAttribute, UDPDataSender
import cv2, numpy as np, threading
    
def SetupNotification(content):
    lblNotif.config(text=content)

def StartExWebcam(index):
    global cap, scaleSet, webcamStatus
    cap = cv2.VideoCapture(index)
    HandVisualizing()

def ValidationInputExWebcam():
    global webcamStatus
    if comboWebcam.get() != "Select Your Camera":
        for i in range(0, len(list_cam)):
            if comboWebcam.get() == list_cam[i]:
                StartExWebcam(i)
    else:
        webcamStatus = False
        exWebcamButton(True)
        SetupNotification("Start Web Camera First.")

def ScaleSetting(x):
    global scaleSet
    scaleSet = -x

def BrightnessSetting(x):
    global brightnessSet
    brightnessSet = x

def ContrastSetting(x):
    global contrastSet
    contrastSet = x

def HandVisualizing():
    global cap, scaleSet, brightnessSet, contrastSet, webcamStatus
    
    SetupNotification("Your Camera Opened. Now Launch The Game.")
    GeneralAttribute.isRun = True
    thread_sender = threading.Thread(target=UDPDataSender.SendingPacket)
    thread_sender.start()

    winName = 'HandTracking'

    fpsReader = FPS()
    cap.set(3, 1280)
    cap.set(4, 720)

    detector = HandDetector(detectionCon=0.8, maxHands=4)
    hands_array = np.empty(10, dtype=object)

    cv2.namedWindow(winName)
    cv2.createTrackbar('Scale', winName, 0, 100, ScaleSetting)
    cv2.setTrackbarMin('Scale', winName, -50)
    cv2.setTrackbarMax('Scale', winName, 0)
    cv2.setTrackbarPos('Scale', winName, -25)

    cv2.createTrackbar('Brightness', winName, 25, 75, BrightnessSetting)
    cv2.setTrackbarMin('Brightness', winName, 25)
    cv2.setTrackbarMax('Brightness', winName, 75)
    cv2.setTrackbarPos('Brightness', winName, 50)

    cv2.createTrackbar('Contrast', winName, 100, 150, ContrastSetting)
    cv2.setTrackbarMin('Contrast', winName, 100)
    cv2.setTrackbarMax('Contrast', winName, 150)
    cv2.setTrackbarPos('Contrast', winName, 125)
    
    while True:
        totalHand = 0
        success, img = cap.read()

        #get the webcam size
        height, width, channels = img.shape

        #prepare the crop
        centerX, centerY = int(height/2), int(width/2)
        radiusX, radiusY = int(scaleSet * height/100), int(scaleSet * width/100)

        minX, maxX = centerX - radiusX, centerX + radiusX
        minY, maxY = centerY - radiusY, centerY + radiusY

        cropped = img[minX:maxX, minY:maxY]
        resized_cropped = cv2.resize(cropped, (width, height))

        if isNormal.get() == True:
            result = resized_cropped
        elif isVertical.get() == True and isHorizontal.get() == True:
            resultTemp = cv2.flip(resized_cropped, 0)
            result = cv2.flip(resultTemp, 1)
        elif isVertical.get() == True:
            result = cv2.flip(resized_cropped, 0)
        elif isHorizontal.get() == True:
            result = cv2.flip(resized_cropped, 1)

        result = cv2.cvtColor(result, cv2.COLOR_BGR2HSV)
        result[:,:,2] = np.clip((contrastSet / 100) * result[:,:,2] + brightnessSet, 0, 255)
        result = cv2.cvtColor(result, cv2.COLOR_HSV2BGR)

        hands, result = detector.findHands(result)  # With Draw
        # hands = detector.findHands(img, draw=False)  # No Draw

        if hands:
            for index, hand_landmark in enumerate(hands):
                hands_array[index] = hands[index]
                disallowed_characters = "( )"
                totalHand = index

                if index == 0:
                    position_index_1 = str(hands_array[0]["center"])

                    GeneralAttribute.positionHand = f"{index + 1},{position_index_1}"
                    for char in disallowed_characters:
                        GeneralAttribute.positionHand = GeneralAttribute.positionHand.replace(char, "")

                elif index == 1:
                    position_index_1 = str(hands_array[0]["center"])
                    position_index_2 = str(hands_array[1]["center"]) 

                    GeneralAttribute.positionHand = f"{index + 1},{position_index_1},{position_index_2}"
                    for char in disallowed_characters:
                        GeneralAttribute.positionHand = GeneralAttribute.positionHand.replace(char, "")

                elif index == 2:
                    position_index_1 = str(hands_array[0]["center"])
                    position_index_2 = str(hands_array[1]["center"])  
                    position_index_3 = str(hands_array[2]["center"]) 

                    GeneralAttribute.positionHand = f"{index + 1},{position_index_1},{position_index_2},{position_index_3}"
                    for char in disallowed_characters:
                        GeneralAttribute.positionHand = GeneralAttribute.positionHand.replace(char, "")

                elif index == 3:
                    position_index_1 = str(hands_array[0]["center"]) 
                    position_index_2 = str(hands_array[1]["center"])  
                    position_index_3 = str(hands_array[2]["center"]) 
                    position_index_4 = str(hands_array[3]["center"])  

                    GeneralAttribute.positionHand = f"{index + 1},{position_index_1},{position_index_2},{position_index_3},{position_index_4}"
                    for char in disallowed_characters:
                        GeneralAttribute.positionHand = GeneralAttribute.positionHand.replace(char, "")

                #sock.sendto(str.encode(str(GeneralAttribute.positionHand)), serverAddressPort)
                
        else:
            GeneralAttribute.positionHand = ""
        
        cv2.putText(result, f'Esc To Stop Camera', (50, 50), cv2.FONT_HERSHEY_SIMPLEX, 0.5, (0, 0, 128), 2)
        cv2.putText(result, f'Slide To Right For Scalling Camera Zoom', (50, 80), cv2.FONT_HERSHEY_SIMPLEX, 0.5, (0, 0, 255), 2)
        fps, img = fpsReader.update(result,pos=(50, 610),bgColor=(128,0,0),scale=1.5,thickness=2)
        cv2.putText(result, f'Sending: {GeneralAttribute.positionHand}', (50, 640), cv2.FONT_HERSHEY_SIMPLEX, 0.5, (0, 0, 128), 2)
        cv2.putText(result, f'Total Hand Detected: {totalHand + 1}', (50, 670), cv2.FONT_HERSHEY_SIMPLEX, 0.5, (0, 0, 128), 2)
        
        result = cv2.resize(result, (0, 0), None, 0.7, 0.7)
        cv2.imshow(winName, result)
        
        if cv2.waitKey(1) == 27:
            cap.release()
            cv2.destroyAllWindows()
            GeneralAttribute.isRun = False
            webcamStatus = False
            
            SetupNotification("Start Webcam and Launch The Game")
            exWebcamButton(True)
            break

def GetListWebcam():
    pygame.camera.init()
    cameras = pygame.camera.list_cameras()
    totalCamera = len(cameras)

    for i in range(0, totalCamera):
        list_cam.append(str(cameras[i]))

def exWebcamButton(index):
    global webcamStatus

    if index == True:
        if webcamStatus == True:
            SetupNotification("Please Close Your Webcam With Esc Button!")
        else:
            webcamStatus = False
            btnExWebcam.configure(command=lambda:exWebcamButton(False))

    elif index == False:
        webcamStatus = True
        btnExWebcam.configure(command=lambda:exWebcamButton(True))

        if isNormal.get() == True and isVertical.get() == True or isNormal.get() == True and isHorizontal.get() == True:
            webcamStatus = False
            exWebcamButton(True)
            SetupNotification("You Only Choose Normal Or Set Horizontal And Vertical!")
        elif isNormal.get() == isVertical.get() == isHorizontal.get() == False:
            webcamStatus = False
            exWebcamButton(True)
            SetupNotification("Please Select One Of Flip Type Above!")
        else:
            SetupNotification("Loading... Please Wait!")
            thread_webcam = threading.Thread(target=lambda:ValidationInputExWebcam())
            thread_webcam.start()

#define capture device
cap = None
webcamStatus = False
list_cam = list()
GetListWebcam()

root = Tk()
root.title("Hand Tracking")
root.geometry("400x75")
root.resizable(0, 0)

#setting gui
lblWebcam = Label(root, text="Flipping Type")
lblWebcam.place(x=10, y=0)

isNormal = BooleanVar()
isVertical = BooleanVar()
isHorizontal = BooleanVar()

flipNormal = Checkbutton(root, text="Normal", variable=isNormal, onvalue=True, offvalue=False)
flipNormal.place(x=100, y=0)
flipNormal.select()

flipVertical = Checkbutton(root, text="Vertically", variable=isVertical, onvalue=True, offvalue=False)
flipVertical.place(x=170, y=0)

flipHorizontal = Checkbutton(root, text="Horizontally", variable=isHorizontal, onvalue=True, offvalue=False)
flipHorizontal.place(x=245, y=0)

lblWebcam = Label(root, text="External Webcam")
lblWebcam.place(x=10, y=25)

comboWebcam = ttk.Combobox(root, value=list_cam, width=25)
comboWebcam.set("Select Your Camera")
comboWebcam.place(x=125, y=25)

btnExWebcam = Button(root, text ="Start", command=lambda:exWebcamButton(False))
btnExWebcam.place(x=310, y=25)

lblNotif = Label(root, text="Setup and Start Webcam First! Then Launch The Game")
lblNotif.place(x=10, y=50)

root.mainloop()