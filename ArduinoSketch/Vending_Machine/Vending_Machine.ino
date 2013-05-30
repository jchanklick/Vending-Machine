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
  
  boolean pinstate = LOW;
  
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
  Serial1.println("System Ready");
  Serial.println("System Ready");
  delay(2000);
  Serial1.write(0xFE);   //command flag
  Serial1.write(0x01);   //clear command.
}

char key_array[2];
#define INLENGTH 16
#define INTERMINATOR 13
char inString[INLENGTH+1];
String InputString;
int inCount;

void loop()
{  
  inCount = 0;
  InputString = "";
    do
    {
      while (!Serial.available());             // wait for input
      inString[inCount] = Serial.read();       // get it
      if (inString [inCount] == INTERMINATOR){
        clearLCD();
        Serial1.print(InputString);
        break;
      }

      InputString += inString;

    }while(true);
    if (InputString == "9999"){
        SelectItem(key_array);
        Serial.print(key_array[0]);
        Serial.print(",");  
        Serial.println(key_array[1]);
        
    }else if (InputString == "1234"){
        VendItem(key_array);
        Serial.print(key_array[0]);
        Serial.print(",");  
        Serial.println(key_array[1]);
    }else if (InputString == "STATUS") {
		Serial.println("OK");
	}
	
    (++inCount < INLENGTH);

    inString[inCount] = 0;                     // null terminate the string
    
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

// Scrolls the display left by the number of characters passed in, and waits a given
// number of milliseconds between each step
void scrollLeft(int num, int wait) {
  for(int i=0;i<num;i++) {
    serCommand();
    Serial1.write(0x18);
    delay(wait);
  }
}

// Scrolls the display right by the number of characters passed in, and waits a given
// number of milliseconds between each step
void scrollRight(int num, int wait) {
  for(int i=0;i<num;i++) {
    serCommand();
    Serial1.write(0x1C);
    delay(wait);
  }
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
void SelectItem(char key_char[2]){
  int i=0;
  clearLCD();
  Serial1.print("Select Item:");
  while (i<2)
  {
    char key = keypad.getKey();
    if (key != NO_KEY){
      key_char[i] = key;
      Serial1.print(key_char[i]);
      Serial1.print(" ");
      i ++;
    }
  }
  delay(1000);
  clearLCD();
}

void VendItem(char key_char[2]){
  clearLCD();
  Serial1.print("Vending:");
  Serial1.print(key_char[0]);
  Serial1.print("-");
  Serial1.print(key_char[1]);
  allpinsoff();      
  switch (key_char[0]){
    case '1':
      digitalWrite(pin_red_1, HIGH);
      Serial.println("Powering Red 1");
      break;
    case '2':
      digitalWrite(pin_red_2, HIGH);
      break;
    case '3':
      digitalWrite(pin_red_3, HIGH);
      break;
    case '4':
      digitalWrite(pin_red_4, HIGH);
      break;
    case '5':
      digitalWrite(pin_red_5, HIGH);
      break;
    case '6':
      digitalWrite(pin_red_6, HIGH);
      break;
  }
  switch (key_char[1]){
    case '1':
      digitalWrite(pin_black_1, HIGH);
      Serial.println("Powering Black 1");
      break;
    case '2':
      digitalWrite(pin_black_2, HIGH);
      break;
    case '3':
      digitalWrite(pin_black_3, HIGH);
      break;
    case '4':
      digitalWrite(pin_black_4, HIGH);
      break;
    case '5':
      digitalWrite(pin_black_5, HIGH);
      break;
    case '6':
      digitalWrite(pin_black_6, HIGH);
      break;
    case '7':
      digitalWrite(pin_black_7, HIGH);
      break;
    case '8':
      digitalWrite(pin_black_8, HIGH);
      break;
    }      
    delay(5000);
    allpinsoff();
    clearLCD();
    //Serial1.print("Pins off...");
    //delay(1000);
    //clearLCD();
}
