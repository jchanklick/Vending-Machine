/**
 * Photobooth.pde by Shu Ito
 */

import processing.video.*;

// resolution: 1592x1080
int cols = 1592;
int rows = 1080; 

Capture cam;

int mainwidth  = cols;
int mainheight = rows;

// variables for taking and numbering four pictures consecutively
int startTime;
int i = 1;
int WAIT_DURATION = 5000;
boolean takePicture = false;
int folderName = 0;
int pictureNumber = 0;

void setup() {
  frameRate(30);
  size(mainwidth, mainheight, JAVA2D);
  colorMode(RGB);
  cam = new Capture(this, 1920, 1080);
  cam.start();
  noSmooth();
  background(0);
}

void draw() {
  if (cam.available()) {
    cam.read();
    image(cam, 0, 0);
  }
   if (takePicture) {
    takePictures();
     }
}

void keyPressed() {
    if (key == ' ') {
      // setup takePictures() to run and increase folder name by 1 every time the function is initiated
      takePicture = true;
      startTime = millis();
      folderName = folderName + 1;
      pictureNumber = pictureNumber + 1;
      i = 1;
  }
}

void takePictures() { 
// setting up file path to save a group of 4 pictures to a single folder
String partialFilePath = "D:\\My Pictures\\Logitech Webcam\\";
String fullFilePath = partialFilePath + folderName;
String pictureName = "\\picture-" + pictureNumber + ".jpg";

if (i == 5) {
// stop the program from running when 4 pictures have been taken
  takePicture = false;
} else if ((millis() - startTime > WAIT_DURATION) && (i < 5)) {
  saveFrame(fullFilePath + pictureName);
// ensure that 4 consecutive pictures are taken
  startTime = millis();
  i = i + 1;
  println("Picture taken!");
} else {
// display a 5 second countdown until picture is taken
  int showTime = round((6000 - (millis() - startTime)) / 1000); 
  println(showTime);
}
}
