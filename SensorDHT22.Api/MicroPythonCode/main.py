import network
import time
import dht
import machine
import urequests
import ntptime
import json

# ================= CONFIGURACIÓN WIFI =================
WIFI_SSID = "SSID_NAME"
WIFI_PASSWORD = "WIFI_PASS"

# ================= CONFIGURACIÓN API EN LA NUBE =================
API_URL = "https://www.fpenate.somee.com/api/sensorreadings" 

# ================= CONFIGURACIÓN DHT22 =================
sensor = dht.DHT22(machine.Pin(4))

def conectar_wifi():
    wlan = network.WLAN(network.STA_IF)
    wlan.active(True)
    if not wlan.isconnected():
        print("Conectando a la red WiFi...")
        wlan.connect(WIFI_SSID, WIFI_PASSWORD)
        while not wlan.isconnected():
            time.sleep(0.5)
    print(f"WiFi Conectada. IP del ESP32: {wlan.ifconfig()[0]}")

def enviar_a_api(temp, hum, fecha_hora):
    anio = fecha_hora[0]
    mes = fecha_hora[1]
    dia = fecha_hora[2]
    hora = fecha_hora[3]
    minuto = fecha_hora[4]
    segundo = fecha_hora[5]
    
    fecha_str = f"{anio}-{mes:02d}-{dia:02d}T{hora:02d}:{minuto:02d}:{segundo:02d}"

    datos = {
        "temperature": temp,   
        "humidity": hum,       
        "fechaHora": fecha_str 
    }

    payload = json.dumps(datos)
    
    headers = {
        "Content-Type": "application/json",
        "Content-Length": str(len(payload))
    }

    try:
        print(f"Enviando a API Nube: {datos}")
        
        # Con el ESP32 actualizado, esto SÍ funcionará
        respuesta = urequests.post(API_URL, data=payload, headers=headers)
        
        print(f"Respuesta de Somee: {respuesta.status_code}")
        
        respuesta.close()
        time.sleep(0.1)
        
    except Exception as e:
        print("Error al enviar datos a la nube:", e)

# ================= BUCLE PRINCIPAL =================
conectar_wifi()

try:
    ntptime.settime()
except:
    pass

while True:
    try:
        sensor.measure()
        temp = sensor.temperature()
        hum = sensor.humidity()
        
        hora_actual = time.localtime(time.time() - (6 * 3600)) 
        
        enviar_a_api(temp, hum, hora_actual)
        
    except OSError as e:
        print("Error leyendo el DHT22:", e)
        
    time.sleep(5)
