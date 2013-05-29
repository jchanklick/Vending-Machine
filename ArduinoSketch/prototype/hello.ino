#include <Keypad.h>


int pin_black_1 = 22;
int pin_black_2 = 24;
int pin_black_3 = 26;
int pin_black_4 = 28;
int pin_black_5 = 30;
int pin_black_6 = 32;
int pin_black_7 = 34;
int pin_black_8 = 36;

int pin_red_1 = 23;
int pin_red_2 = 25;
int pin_red_3 = 27;
int pin_red_4 = 29;
int pin_red_5 = 31;
int pin_red_6 = 33;


const byte ROWS = 4; 
const byte COLS = 4; 
char keys[ROWS][COLS] = {
  {'1','2','3','A'},
  {'4','5','6','B'},
  {'7','8','9','C'},
  {'*','0','#','D'}
};
byte rowPins[ROWS] = {2,3,4,5}; //connect to row pinouts 
byte colPins[COLS] = {6,7,8,9}; //connect to column pinouts

Keypad keypad = Keypad( makeKeymap(keys), rowPins, colPins, ROWS, COLS );


void setup()
{
  Serial.begin(9600);
  Serial1.begin(9600);
  //backlightOn();
  
  //black pins
  pinMode(pin_black_1, OUTPUT);
  pinMode(pin_black_2, OUTPUT);
  pinMode(pin_black_3, OUTPUT);
  pinMode(pin_black_4, OUTPUT);
  pinMode(pin_black_5, OUTPUT);
  pinMode(pin_black_6, OUTPUT);
  pinMode(pin_black_7, OUTPUT);
  pinMode(pin_black_8, OUTPUT);
  
  pinMode(pin_red_1, OUTPUT);
  pinMode(pin_red_2, OUTPUT);
  pinMode(pin_red_3, OUTPUT);
  pinMode(pin_red_4, OUTPUT);
  pinMode(pin_red_5, OUTPUT);
  pinMode(pin_red_6, OUTPUT);
  
  boolean pinstate = false;
  
  digitalWrite(pin_black_1, pinstate);
  digitalWrite(pin_black_2, pinstate);
  digitalWrite(pin_black_3, pinstate);
  digitalWrite(pin_black_4, pinstate);
  digitalWrite(pin_black_5, pinstate);
  digitalWrite(pin_black_6, pinstate);
  digitalWrite(pin_black_7, pinstate);
  digitalWrite(pin_black_8, pinstate);
  
  digitalWrite(pin_red_1, pinstate);
  digitalWrite(pin_red_2, pinstate);
  digitalWrite(pin_red_3, pinstate);
  digitalWrite(pin_red_4, pinstate);
  digitalWrite(pin_red_5, pinstate);
  digitalWrite(pin_red_6, pinstate);
  
  Serial1.write(0xFE);   //command flag
  Serial1.write(0x01);   //clear command.
  delay(50);
  Serial1.write("Hi there.");
  delay(2000);
  Serial1.write(0xFE);   //command flag
  Serial1.write(0x01);   //clear command.
}

int counter = 0;
int thenum = 0;
int tehkey = 0;
int first = 0;
int second = 0;

void loop()
{  
  char key = keypad.getKey();

  if (key != NO_KEY){
    tehkey = key;
    if(tehkey == 48) {
      thenum = 0;
    } else if(tehkey == 49) {
      thenum = 1;
    } else if(tehkey == 50) {
      thenum = 2;
    } else if(tehkey == 51) {
      thenum = 3;
    } else if(tehkey == 52) {
      thenum = 4;
    } else if(tehkey == 53) {
      thenum = 5;
    } else if(tehkey == 54) {
      thenum = 6;
    } else if(tehkey == 55) {
      thenum = 7;
    } else if(tehkey == 56) {
      thenum = 8;
    } else if(tehkey == 57) {
      thenum = 9;
    } else if(tehkey == 42) {
      thenum = 666;
    } else if(tehkey == 35) {
      thenum = 667;
    } else if(tehkey == 65) {
      thenum = 10;
    } else if(tehkey == 66) {
      thenum = 11;
    } else if(tehkey == 67) {
      thenum = 12;
    } else if(tehkey == 68) {
      thenum = 13;
    }
    
    if(counter == 0) {
      clearLCD();
      first = thenum;
      Serial1.print(first);
      counter++;
    } else if (counter == 1) {
      clearLCD();
      second = thenum;
      Serial1.print(second);
      counter++;
    }
    
    //Serial1.print(key);
  }
  if ((counter > 1) && (thenum == 667)) {
      clearLCD();
      counter = 0;
      Serial1.print(first);
      Serial1.print("-");
      Serial1.print(second);
      
      allpinsoff();
      
      if(first == 1) {
        digitalWrite(pin_red_1, HIGH);
      } else if(first == 2) {
        digitalWrite(pin_red_2, HIGH);
      } else if(first == 3) {
        digitalWrite(pin_red_3, HIGH);
      } else if(first == 4) {
        digitalWrite(pin_red_4, HIGH);
      } else if(first == 5) {
        digitalWrite(pin_red_5, HIGH);
      } else if(first == 6) {
        digitalWrite(pin_red_6, HIGH);
      }
      
      if(second == 1) {
        digitalWrite(pin_black_1, HIGH);
      } else if(second == 2) {
        digitalWrite(pin_black_2, HIGH);
      } else if(second == 3) {
        digitalWrite(pin_black_3, HIGH);
      } else if(second == 4) {
        digitalWrite(pin_black_4, HIGH);
      } else if(second == 5) {
        digitalWrite(pin_black_5, HIGH);
      } else if(second == 6) {
        digitalWrite(pin_black_6, HIGH);
      } else if(second == 7) {
        digitalWrite(pin_black_7, HIGH);
      } else if(second == 8) {
        digitalWrite(pin_black_8, HIGH);
      }
      
      delay(6789);
      
      allpinsoff();
      clearLCD();
      Serial1.print("Pins off...");
      delay(1500);
      clearLCD();
      
      
   }
   
  
  
}

void allpinsoff() {
  digitalWrite(pin_black_1, LOW);
  digitalWrite(pin_black_2, LOW);
  digitalWrite(pin_black_3, LOW);
  digitalWrite(pin_black_4, LOW);
  digitalWrite(pin_black_5, LOW);
  digitalWrite(pin_black_6, LOW);
  digitalWrite(pin_black_7, LOW);
  digitalWrite(pin_black_8, LOW);
  
  digitalWrite(pin_red_1, LOW);
  digitalWrite(pin_red_2, LOW);
  digitalWrite(pin_red_3, LOW);
  digitalWrite(pin_red_4, LOW);
  digitalWrite(pin_red_5, LOW);
  digitalWrite(pin_red_6, LOW);
}

void selectLineOne(){  //puts the cursor at line 0 char 0.
   Serial1.write(0xFE);   //command flag
   Serial1.write(128);    //position
   delay(10);
}
void selectLineTwo(){  //puts the cursor at line 0 char 0.
   Serial1.write(0xFE);   //command flag
   Serial1.write(192);    //position
   delay(10);
}
void goTo(int position) { //position = line 1: 0-15, line 2: 16-31, 31+ defaults back to 0
if (position<16){ Serial1.write(0xFE);   //command flag
              Serial1.write((position+128));    //position
}else if (position<32){Serial1.write(0xFE);   //command flag
              Serial1.write((position+48+128));    //position 
} else { goTo(0); }
   delay(10);
}

void clearLCD(){
   Serial1.write(0xFE);   //command flag
   Serial1.write(0x01);   //clear command.
   delay(10);
}
void backlightOn(){  //turns on the backlight
    Serial1.write(0x7C);   //command flag for backlight stuff
    Serial1.write(157);    //light level.
   delay(10);
}
void backlightOff(){  //turns off the backlight
    Serial1.write(0x7C);   //command flag for backlight stuff
    Serial1.write(128);     //light level for off.
   delay(10);
}
void serCommand(){   //a general function to call the command flag for issuing all other commands   
  Serial1.write(0xFE);
}
