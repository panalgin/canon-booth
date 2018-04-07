/*
 Name:    Firmware.ino
 Created: 12/5/2017 9:23:02 AM
 Author:  Mashadow
*/

#include <FastLED.h>

#define NUM_LEDS 90
#define DATA_PIN 6

CRGB leds[NUM_LEDS];

uint8_t counter = 0;

// the setup function runs once when you press reset or power the board
void setup() {
  Serial.begin(9600);

  while (!Serial) { ; }

  FastLED.addLeds<WS2812B, DATA_PIN, GRB>(leds, NUM_LEDS);

  FastLED.setCorrection(TypicalSMD5050);
  FastLED.setTemperature(Candle);

  attachInterrupt(digitalPinToInterrupt(2), onHallTriggered, RISING);

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
  if (counter > 0)
    counter--;
}

unsigned long lastCommandSent = millis();

void onHallTriggered() {
  if (counter < NUM_LEDS) {
    counter++;

    if (counter == NUM_LEDS) {
      if (millis() - lastCommandSent > (1000 * 10)) {
        lastCommandSent = millis();

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
  for (uint8_t i = 0; i < NUM_LEDS; i++) {
    if (i < counter)
      leds[i] = CRGB::Magenta;
    else
      leds[i] = CRGB::Black;
  }
}

void playVideo() {
  Serial.println("Play");
}

