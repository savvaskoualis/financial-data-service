# Financial Data Service

## Overview

This solution provides a service to handle live financial instrument prices sourced from a public data provider.

## Components

The solution consists of the following projects:

1. **FinancialDataService.Api**: Provides REST API endpoints for getting available instruments and the latest price of a specific instrument.
2. **FinancialDataService.Jobs**: Fetches available instruments and price updates from an external data provider, updates the data store and publishes updates.
3. **FinancialDataService.Streams**: Exposes the websocket responsible for delivering price updates to subscribed users. 
4. **FinancialDataService.TcpServer**: Manages the TCP server for the simple TCP/IP backplane to distribute price updates. Used during development
5. **FinancialDataService.DemoUI**: A Blazor application for demonstrating the solution, including API calls and WebSocket subscriptions.


## Architecture

### System Components Diagram

![System Components](http://www.plantuml.com/plantuml/proxy?cache=no&src=https://raw.githubusercontent.com/savvaskoualis/financial-data-service/main/docs/SystemComponents.iuml)

### Scalability Design

- **Data Fetching and Storage**:
    - The `FinancialDataService.Jobs` component fetches available instruments and price updates from the external data provider, maintaining a single connection.
    - Maintaining a single connection is crucial for responsible scaling as the external provider may not allow multiple subscriptions.
    - Fetched data is stored in the database for efficient retrieval.

- **Serving API Requests**:
    - The `FinancialDataService.Api` component handles user requests for available instruments and the latest prices by querying the database.

- **Publishing Updates**:
    - In addition to updating the database, `FinancialDataService.Jobs` publishes price updates to the backplane (currently TCP for demonstration purposes).

- **Distributing Updates**:
    - The `FinancialDataService.Streams` component subscribes to the backplane. Upon receiving updates, it pushes them to SignalR.
    - SignalR then distributes these updates to all connected WebSocket clients, ensuring real-time delivery to the end user.

- **Future Scalability with Distributed Backplane**:
    - For production, a clustered distributed backplane like Redis or Kafka should be used to handle thousands of subscribers efficiently.

This workflow ensures efficient data management and real-time update delivery, making the system scalable and capable of handling a large number of subscribers.

### Prerequisites

- .NET 8 SDK
- Visual Studio Code 
- Docker and Docker Compose

### Setup

1. Clone the repository.
   ```sh
   git clone https://github.com/savvaskoualis/financial-data-service.git

2. Navigate to the project directory.
    ```sh
    cd financial-data-service

3. Open the project in Visual Studio Code.
    ```sh
    code .
   
4. Ensure the Docker extension is installed in Visual Studio Code. If not, install it from the Extensions view (Ctrl+Shift+X) by searching for "Docker" and installing the official Docker extension.

5. Open the Command Palette (Ctrl+Shift+P) and select Docker: Add Docker Files to Workspace. Choose .NET: ASP.NET Core when prompted and select the appropriate port.

6. Build and run the Docker Compose services. Open the Command Palette again and select Docker: Compose Up.

7. Alternatively, you can run the Docker Compose services using the terminal:
    ```sh
    docker-compose up --build -d
   
8. To stop the services, use the Command Palette and select Docker: Compose Down or run the following command in the terminal:
    ```sh
    docker-compose down

## Docker Compose Configuration

The Docker Compose file includes the following services:

1. **postgres**: PostgreSQL database to store instrument and price data.
2. **redis**: Redis server to act as the distributed backplane.
3. **api**: REST API service to handle user requests.
4. **jobs**: Service to fetch instruments and price updates, and publish them to the backplane.
5. **streams**: WebSocket server to distribute price updates to subscribers.
6. **demoapp**: Blazor application for demonstrating the solution.

This setup will ensure that your application uses Redis as the backplane, providing a robust and scalable solution for handling price updates and subscriptions. 
