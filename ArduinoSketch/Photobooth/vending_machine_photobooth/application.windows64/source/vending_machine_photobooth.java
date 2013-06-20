import processing.core.*; 
import processing.data.*; 
import processing.event.*; 
import processing.opengl.*; 

import processing.video.*; 
import java.util.Properties; 
import javax.mail.*; 
import javax.mail.internet.*; 

import com.sun.mail.imap.protocol.*; 
import javax.mail.search.*; 
import javax.mail.*; 
import com.sun.activation.viewers.*; 
import com.sun.mail.iap.*; 
import com.sun.activation.registries.*; 
import javax.mail.internet.*; 
import javax.activation.*; 
import com.sun.mail.smtp.*; 
import com.sun.mail.handlers.*; 
import com.sun.mail.util.*; 
import javax.mail.event.*; 
import com.sun.mail.imap.*; 
import com.sun.mail.pop3.*; 

import java.util.HashMap; 
import java.util.ArrayList; 
import java.io.File; 
import java.io.BufferedReader; 
import java.io.PrintWriter; 
import java.io.InputStream; 
import java.io.OutputStream; 
import java.io.IOException; 

public class vending_machine_photobooth extends PApplet {

/*
Vending Machine Photobooth by Shu Ito
*/

 //import video library




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
public void setup() {
  size(cols, rows, JAVA2D);
  colorMode(RGB);
  cam = new Capture(this, 640, 480); //maximum supported resolution of camera
  cam.start();
  noSmooth();
}

//main display window
public void draw() {
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
public void takePictures() {
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
public void sendMail() {
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


  static public void main(String[] passedArgs) {
    String[] appletArgs = new String[] { "vending_machine_photobooth" };
    if (passedArgs != null) {
      PApplet.main(concat(appletArgs, passedArgs));
    } else {
      PApplet.main(appletArgs);
    }
  }
}
