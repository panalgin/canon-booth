/*
 Name:    Firmware.ino
 Created: 12/5/2017 9:23:02 AM
 Author:  Mashadow
*/

#include <FastLED.h>

#define NUM_LEDS_RIGHT 90
#define NUM_LEDS_LEFT 90
#define DATA_RIGHT_PIN 6
#define DATA_LEFT_PIN 7
#define RIGHT_RELAY 8
#define LEFT_RELAY 9

CRGB leds_right[NUM_LEDS_RIGHT];
CRGB leds_left[NUM_LEDS_LEFT];

uint8_t rightCounter = 0;
uint8_t leftCounter = 0;

// the setup function runs once when you press reset or power the board
void setup() {
  Serial.begin(9600);

  while (!Serial) { ; }

  pinMode(RIGHT_RELAY, OUTPUT);
  pinMode(LEFT_RELAY, OUTPUT);

  FastLED.addLeds<WS2812, DATA_RIGHT_PIN, GRB>(leds_right, NUM_LEDS_RIGHT);
  FastLED.addLeds<WS2812, DATA_LEFT_PIN, GRB>(leds_left, NUM_LEDS_LEFT);

  FastLED.setCorrection(TypicalSMD5050);
  FastLED.setTemperature(Candle);

  attachInterrupt(digitalPinToInterrupt(2), onRightHallTriggered, RISING);
  attachInterrupt(digitalPinToInterrupt(3), onLeftHallTriggered, RISING);

  cli(); // stop interrupts
  TCCR1A = 0; // set entire TCCR1A register to 0
  TCCR1B = 0; // same for TCCR1B
  TCNT1 = 0; // initialize counter value to 0
         // set compare match register for 1 Hz increments
  OCR1A = 62499; // = 16000000 / (256 * 1) - 1 (must be <65536)
           // turn on CTC mode
  TCCR1B |= (1 << WGM12);
  // Set CS12, CS11 and CS10 bits for 256 prescaler
  TCCR1B |= (1 << CS12) | (0 << CS11) | (0 << CS10);
  // enable timer compare interrupt
  TIMSK1 |= (1 << OCIE1A);
  sei(); // allow interrupts
}

ISR(TIMER1_COMPA_vect) {
  if (rightCounter > 0)
    rightCounter--;

  if (leftCounter > 0)
    leftCounter--;
}

unsigned long lastCommandSent = millis();

void onRightHallTriggered() {
  if (rightCounter < NUM_LEDS_RIGHT) {
    rightCounter++;

    if (rightCounter == NUM_LEDS_RIGHT) {
      if (millis() - lastCommandSent > (1000 * 10)) {
        lastCommandSent = millis();

        digitalWrite(LEFT_RELAY, LOW);
        digitalWrite(RIGHT_RELAY, HIGH);

        playVideo();
      }
    }
  }
}

void onLeftHallTriggered() {
  if (leftCounter < NUM_LEDS_LEFT) {
    leftCounter++;

    if (leftCounter == NUM_LEDS_LEFT) {
      if (millis() - lastCommandSent > (1000 * 10)) {
        lastCommandSent = millis();

        digitalWrite(RIGHT_RELAY, LOW);
        digitalWrite(LEFT_RELAY, HIGH);

        playVideo();
      }
    }
  }
}

// the loop function runs over and over again until power down or reset
void loop() {
  show();
}

void show() {
  updateStrip();
  FastLED.show();
  FastLED.delay(50);
}

void updateStrip() {
  for (uint8_t i = 0; i < NUM_LEDS_RIGHT; i++) {
    if (i < rightCounter)
      leds_right[i] = CRGB::Magenta;
    else
      leds_right[i] = CRGB::Black;
  }
  for (uint8_t i = 0; i < NUM_LEDS_LEFT; i++) {
    if (i < leftCounter)
      leds_left[i] = CRGB::Orange;
    else
      leds_left[i] = CRGB::Black;
  }
}

void playVideo() {
  Serial.println("Play");
}

