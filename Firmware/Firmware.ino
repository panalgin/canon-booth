/*
Name:    Firmware.ino
Created: 12/5/2017 9:23:02 AM
Author:  Mashadow
*/

#include <FastLED.h>

#define INCREMENT_THRESOLD 2  //milliseconds between two increments, how fast do we want it ?
#define DECREMENT_THRESOLD 100  //

#define LED_FPS 30 //how many updates per second ? 

#define NUM_LEDS 300
#define DATA_PIN 6
#define TRIGGER_PIN 10

CRGB leds[NUM_LEDS];

volatile uint16_t counter = 0;
volatile unsigned long revolutions = 0;

// the setup function runs once when you press reset or power the board
void setup() {
	Serial.begin(115200);

	while (!Serial) {
		;
	}

	pinMode(TRIGGER_PIN, INPUT_PULLUP);

	FastLED.addLeds<WS2812B, DATA_PIN, GRB>(leds, NUM_LEDS);

	FastLED.setCorrection(TypicalSMD5050);
	FastLED.setTemperature(Candle);

	attachInterrupt(digitalPinToInterrupt(2), onHallTriggered, RISING);

	counter = 0;
}

unsigned long lastCommandSent = millis();
unsigned long lastIncrementedAt = millis();

void onHallTriggered() {
	revolutions++;

	if (millis() - lastIncrementedAt > INCREMENT_THRESOLD) {
		lastIncrementedAt = millis();

		if (counter < NUM_LEDS) {
			counter++;

			if (counter >= 290)
				Serial.println("Stop");
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
	checkForSampling();
}

unsigned long lastSampledAt = 0;
unsigned long sampleInterval = 500;
unsigned long lastMillisSample = 0;
unsigned long lastRevSample = 0;

void checkForSampling() {
	if (millis() - lastSampledAt > sampleInterval) {
		uint16_t timeElapsed = millis() - lastSampledAt;
		lastSampledAt = millis();

		uint16_t revsOccurred = revolutions - lastRevSample;
		lastRevSample = revolutions;

		Serial.print("D:");
		Serial.print(revsOccurred);
		Serial.print(",");
		Serial.println(timeElapsed);
	}
}

void updateStrip() {
	for (uint16_t i = 0; i < NUM_LEDS; i++) {
		if (i < counter)
			leds[i] = CRGB::Red;
		else
			leds[i] = CRGB::Black;
	}
}

unsigned long lastDecrementedAt = 0;

void checkForDecrement() {
	if (millis() - lastIncrementedAt > 400) {
		if (millis() - lastDecrementedAt > DECREMENT_THRESOLD) {
			lastDecrementedAt = millis();

			if (counter > 0) {
				counter--;
			}
		}
	}
}

