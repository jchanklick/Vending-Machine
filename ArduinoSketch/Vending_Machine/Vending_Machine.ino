#include <Keypad.h>

int pin_black[] = {22, 24, 26, 28, 30, 32, 34, 36};
int pin_red[] = {23, 25, 27, 29, 31, 33};

int indshelf[] = {52, 50, 48, 46, 44};
int indmotor[] = {53, 51, 49, 47, 45, 43, 41, 39};
boolean homestate = false;
boolean flipstate = false;

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
  backlightOn();
  
  for (int i = 0; i < 8; i++){ 
    pinMode ( pin_black[i], OUTPUT);
    digitalWrite(pin_black[i], LOW);
  }

  for (int i = 0; i < 6; i++){ 
    pinMode ( pin_red[i], OUTPUT);
    digitalWrite(pin_red[i], LOW);
  }
  
  for(int i=0;i<5;i++) {
    pinMode(indshelf[i], OUTPUT);
    delay(10);
    digitalWrite(indshelf[i], LOW);
  }
  for(int i=0;i<8;i++) {
    pinMode(indmotor[i], INPUT);
  }
  delay(2000);
  Serial1.write(0xFE);   //command flag
  Serial1.write(0x01);   //clear command.
  delay(50);
  clearLCD();
  Serial1.println("System Ready");
  Serial.println("System Ready");
  delay(2000);
  Serial1.write(0xFE);   //command flag
  Serial1.write(0x01);   //clear command.

}

char key_array[2];
#define INLENGTH 16
#define INTERMINATOR 13
#define TIME_LIMIT 500000
char inString[INLENGTH+1];
String InputString;
int inCount;

void loop()
{  
  inCount = 0;
  InputString = "";
    do
    {
      
      Serial1.print("Ready");
      delay (500);
      clearLCD();
      while (!Serial.available());             // wait for input
      inString[inCount] = Serial.read();       // get it
      if (inString [inCount] == INTERMINATOR){ // break on esc character
        break;
      }
      InputString += inString;

    }while(true);
	// If input from serial port matches a command, execute that command.  Otherwise, echo input to LCD
    if (InputString == "9999"){
        if (SelectItem(key_array, TIME_LIMIT) < TIME_LIMIT){
			Serial.print("KEYPAD:");
			Serial.print(key_array[0]);
			Serial.print(",");  
			Serial.println(key_array[1]);
		} else {
			Serial.println("KEYPAD:TIMEOUT");
	        	Serial1.println("Timed Out!");
		}
        
    }else if (InputString == "1234"){
        VendItem(key_array);
        Serial.print("VEND:");
	Serial.print(key_array[0]);
        Serial.print(",");  
        Serial.println(key_array[1]);
    }else if (InputString == "1111"){
        backlightOn();
        Serial.println("BACKLIGHT ON");
    }else if (InputString == "0000"){
        backlightOff();
        Serial.println("BACKLIGHT OFF");
    }else if (InputString == "8378"){
        Serial.println("MOTOR TEST");
        digitalWrite (pin_red[0], HIGH);
        digitalWrite (pin_black[0], HIGH);
        digitalWrite (indshelf[0], HIGH);
        delay (500);
        while (!digitalRead(indmotor[0])){
          Serial.println("Motor 1 is reading high");
        }
        digitalWrite (indshelf[0], LOW);
        digitalWrite (pin_red[0], LOW);
        digitalWrite (pin_black[0], LOW);
    }else if (InputString == "STATUS") {
		Serial.println("OK");
    }else{
	clearLCD();
        Serial.print("PRINT:");
	Serial.println(InputString);
	Serial1.print(InputString);
        delay (5000);
        clearLCD();
    }
	
   (++inCount < INLENGTH);

    inString[inCount] = 0;                     // null terminate the string
    
}

void allpinsoff() {
  for (int i = 0; i < 8; i++){ 
    digitalWrite(pin_black[i], LOW);
  }

  for (int i = 0; i < 6; i++){ 
    digitalWrite(pin_red[i], LOW);
  }
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
long SelectItem(char key_char[2], long time_limit){
  int i = 0;
  long time_counter = 0;
  clearLCD(); //clear screen, and prompt for item selection
  Serial1.print("Select Item:"); 
  while ((i<2) && (time_counter < time_limit)) // give the user a timed window to select an item
  {
    char key = keypad.getKey();
    if (key != NO_KEY){
      key_char[i] = key;
      Serial1.print(key_char[i]);
      i ++;
    }
    time_counter++;
  }
  delay(1000);  
  clearLCD();
  return time_counter;
}

void VendItem(char key_char[2]){
  clearLCD();
  Serial1.print("Vending:");
  Serial1.print(key_char[0]);
  Serial1.print(key_char[1]);
  allpinsoff();      
  digitalWrite (pin_red[(key_char[0]-'0')-1], HIGH);
  digitalWrite (pin_black[(key_char[1]-'0')-1], HIGH);      
  delay(5000);
  allpinsoff();
  clearLCD();
}

