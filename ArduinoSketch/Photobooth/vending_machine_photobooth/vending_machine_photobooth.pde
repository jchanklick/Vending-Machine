/*
Vending Machine Photobooth by Shu Ito
*/

import processing.video.*; //import video library
import java.util.Properties;
import javax.mail.*;
import javax.mail.internet.*;

Capture cam; //webcam initiation

//screen display resolution = maximum resolution of camera
int cols = 640;
int rows = 480;

//file path information for saving images
String file_path = "C:\\Users\\Administrator\\Desktop\\vending_machine_pics\\";
String file_format = ".jpg";
String file_name;

//ready the sketch to listen for serial input
int startTime = millis();
final int CAMERA_INIT_TIMER = 5000;

//initiate sketch
void setup() {
  size(cols, rows, JAVA2D);
  colorMode(RGB);
  cam = new Capture(this, 640, 480); //maximum supported resolution of camera
  cam.start();
  noSmooth();
}

//main display window
void draw() {
  if (cam.available()) {
    cam.read();
    image(cam, 0, 0);
  }

  if (millis() - startTime > CAMERA_INIT_TIMER) {
      takePictures();
      sendMail();
      exit ();
      
  }
}

//function to take and save pictures
void takePictures() {
  //time stamp as file name for identification
  int y = year();
  int mo = month();
  int d = day();
  int h = hour();
  int mi = minute();
  int s = second();
  
  file_name = y + mo + d + "_" + h + mi + s;
  
  saveFrame(file_path + file_name + file_format);
  
}
void sendMail() {
  // Create a session
  String host="10.1.1.50";
  Properties props=new Properties();

  // SMTP Session
  props.put("mail.transport.protocol", "smtp");
  props.put("mail.smtp.host", host);
  props.put("mail.smtp.port", "25");
  props.put("mail.smtp.auth", "false");

  try
  {
    
  String from = "vending@klick.com";
  String to = "jchan@klick.com";

  // Get system properties
  Properties properties = System.getProperties();

  // Setup mail server
  properties.setProperty("mail.smtp.host", host);

  // Get the default Session object.
  Session session = Session.getDefaultInstance(properties);

  // Define message
  Message message = new MimeMessage(session);
  message.setFrom(new InternetAddress(from));
  message.addRecipient(Message.RecipientType.TO,
  new InternetAddress(to));
  message.setSubject("#KlickVendingMachine");

  // Create the message part 
  BodyPart messageBodyPart = new MimeBodyPart();

  // Fill the message
  messageBodyPart.setText("");

  Multipart multipart = new MimeMultipart();
  multipart.addBodyPart(messageBodyPart);

  // Part two is attachment
  messageBodyPart = new MimeBodyPart();
  String filename = file_path + file_name + file_format;
  DataSource source = new FileDataSource(filename);
  messageBodyPart.setDataHandler(new DataHandler(source));
  messageBodyPart.setFileName(filename);
  multipart.addBodyPart(messageBodyPart);

  // Put parts in message
  message.setContent(multipart);

  // Send the message
  Transport.send(message);
  // println("Msg Send ...."); 
  }
  catch(Exception e)
  {
    e.printStackTrace();
  }

}


