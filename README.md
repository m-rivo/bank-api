# 🏦 Bank System API

Sistemas para la gestión de cuentas bancarias y transacciones desarrollada con **.NET 8**. Este proyecto implementa inyección de dependencias y pruebas unitarias automatizadas.

## 📋 Requisitos Previos

Asegúrese de tener instalado lo siguiente:
* **SDK de .NET 8.0** o superior.
* **Herramienta de Entity Framework Core**: Se instala mediante el comando
  `dotnet tool install --global dotnet-ef`

## 🚀 Instrucciones de Ejecución

1.  **Clonar el repositorio**
    ```bash
    git clone https://github.com/m-rivo/bank-api.git
    cd bank-api
    ```

2.  **Instalar Dependencias**
    ```bash
    dotnet restore
    ```
    
3.  **Iniciar la API**

    ```bash
    dotnet run --project BankSystem.Api
    ```
       
    Una vez ejecutado, puede acceder a la interfaz de pruebas de Swagger UI en: http://localhost:5242/swagger/index.html

## 🧪 Pruebas Unitarias
1. **Para ejecutar pruebas unitarias simplemente ejecute el siguiente comando:**
    
    ```bash
    dotnet test
    ```
    _Se ejecutan 6 tests que validan la lógica de negocio (creación de cuenta, depósito, retiro, cálculo de intereses, consulta de saldo e historial de transacciones)._
