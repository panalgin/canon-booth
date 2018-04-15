/*
 Name:    Firmware.ino
 Created: 12/5/2017 9:23:02 AM
 Author:  Mashadow
*/

#include <FastLED.h>

#define INCREMENT_THRESOLD 1000  //milliseconds between two increments, how fast do we want it ?
#define DECREMENT_THRESOLD 3000  //

#define LED_FPS 30 //how many updates per second ? 

#define NUM_LEDS 300
#define DATA_PIN 6

CRGB leds[NUM_LEDS];

uint16_t counter = 0;

// the setup function runs once when you press reset or power the board
void setup() {
	Serial.begin(115200);

	while (!Serial) { ; }

	FastLED.addLeds<WS2812B, DATA_PIN, GRB>(leds, NUM_LEDS);

	FastLED.setCorrection(TypicalSMD5050);
	FastLED.setTemperature(Candle);

	attachInterrupt(digitalPinToInterrupt(2), onHallTriggered, RISING);
}

unsigned long lastCommandSent = millis();
unsigned long lastIncrementedAt = millis();

void onHallTriggered() {
	if (millis() - lastIncrementedAt > INCREMENT_THRESOLD) {
		lastIncrementedAt = millis();

		if (counter < NUM_LEDS) {
			counter++;
		}
	}
}

// the loop function runs over and over again until power down or reset
void loop() {
  show();
}

unsigned long lastSyncedAt = 0;

void show() {
  updateStrip();

  if (millis() - lastSyncedAt > (1000 / LED_FPS)) {
	  FastLED.show();
	  lastSyncedAt = millis();
  }

  checkForDecrement();
}

void updateStrip() {
  for (uint16_t i = 0; i < NUM_LEDS; i++) {
    if (i < counter)
      leds[i] = CRGB::Magenta;
    else
      leds[i] = CRGB::Black;
  }
}

unsigned long lastDecrementedAt = millis();

void checkForDecrement() {
	if (millis() - lastDecrementedAt > DECREMENT_THRESOLD) {
		lastDecrementedAt = millis();

		counter--;
	}
}

