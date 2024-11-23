#include <WiFi.h>
#include <ArduinoWebsockets.h>

using namespace websockets;

// Piny przycisków
#define BUTTON_PIN 23           // Przycisk "Not Veto"
#define BUTTON_PIN_VETO 22      // Przycisk "Veto"

// Dane WiFi
const char* ssid = "Propaganda AuthControl"; 
const char* password = "12345678"; 

// Adres serwera WebSocket
const char* serverAddress = "ws://172.20.10.2:80";

WebsocketsClient client;

// Flagi i czas dla debouncingu
unsigned long lastPressTimeNotVeto = 0;
unsigned long lastPressTimeVeto = 0;
unsigned long debounceDelay = 50; // 50 ms
bool selectedChoice = false;

// Funkcje ustawiania stanu LED (do dalszego rozwinięcia)
void setGreen() {
  // Implementacja logiki zapalenia zielonego światła
  digitalWrite(12, LOW);
  digitalWrite(13, LOW);
  digitalWrite(2, HIGH);
}

void setRed() {
  // Implementacja logiki zapalenia czerwonego światła
  digitalWrite(12, HIGH);
  digitalWrite(13, LOW);
  digitalWrite(2, LOW);
}

void setYellow() {
  // Implementacja logiki zapalenia żółtego światła
  digitalWrite(12, LOW);
  digitalWrite(13, HIGH);
  digitalWrite(2, LOW);
}

// Wysłanie głosu do serwera
void sendVote(bool isVeto) {
  String vote = isVeto ? "true" : "false";
  String payload = "sendVote;" + vote;

  client.send(payload);
  setRed(); // Zapalenie czerwonego światła po wysłaniu
  Serial.println("Wysłano: " + payload);
}

// Sprawdzenie przycisku "Not Veto"
void checkNoVeto() {
  int buttonState = digitalRead(BUTTON_PIN);
  if (buttonState == HIGH && !selectedChoice) {
    lastPressTimeNotVeto = millis();
    Serial.println("Not Veto");
    selectedChoice = true; // Ustawiamy wybór na "Not Veto"
    sendVote(false);
  }
}

// Sprawdzenie przycisku "Veto"
void checkVeto() {
  int buttonState = digitalRead(BUTTON_PIN_VETO);
  if (buttonState == HIGH && !selectedChoice) {
    lastPressTimeVeto = millis();
    Serial.println("Veto");
    selectedChoice = true; // Ustawiamy wybór na "Veto"
    sendVote(true);
  }
}

// Próba ponownego połączenia z serwerem
unsigned long lastReconnectAttempt = 0;

void reconnect() {
  if (millis() - lastReconnectAttempt > 5000) { // Próba co 5 sekund
    lastReconnectAttempt = millis();
    if (client.connect(serverAddress)) {
      Serial.println("Ponownie połączono z serwerem WebSocket.");
      setYellow(); // Żółte światło sygnalizujące połączenie
    } else {
      Serial.println("Nie udało się połączyć z serwerem.");
    }
  }
}

void setup() {
  Serial.begin(115200);

  // Inicjalizacja przycisków
  pinMode(BUTTON_PIN, INPUT);
  pinMode(BUTTON_PIN_VETO, INPUT);

  pinMode(13, OUTPUT);
  pinMode(12, OUTPUT);
  pinMode(2, OUTPUT);
  

  // Połączenie z WiFi
  Serial.print("Łączenie z WiFi...");
  WiFi.begin(ssid, password);
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println("\nPołączono z WiFi");
  Serial.print("Adres IP: ");
  Serial.println(WiFi.localIP());

  // Połączenie z serwerem WebSocket
  Serial.println("Łączenie z serwerem WebSocket...");
  if (client.connect(serverAddress)) {
    Serial.println("Połączono z serwerem WebSocket.");
    setYellow(); // Sygnalizacja połączenia
  } else {
    Serial.println("Nie udało się połączyć z serwerem.");
  }

  // Obsługa wiadomości przychodzących
  client.onMessage([](WebsocketsMessage message) {
    Serial.println("Otrzymano wiadomość: " + message.data());

    if(message.data() == "newVoting;")
    {
      setGreen();
      selectedChoice = false;
    }
    
    // Obsługa konkretnych typów wiadomości
    if (message.data().indexOf("type:1") > 0) {
      Serial.println("Otrzymano wiadomość typu 1.");
    }
  });
}

void loop() {
  // Utrzymanie połączenia WebSocket
  if (client.available()) {
    client.poll();
  } else {
    reconnect(); // Próba ponownego połączenia w razie problemów
  }

  // Obsługa przycisków
  checkNoVeto();
  checkVeto();

  delay(50); // Opóźnienie w celu oszczędzania zasobów
}
